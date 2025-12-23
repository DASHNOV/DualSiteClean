using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoublonManager.Models;
using DoublonManager.Services;
using System.Data.SqlClient;
using DoublonManager.Helpers;

namespace DoublonManager.Forms
{
    public partial class FormDoublonManager : Form
    {
        private DoublonDetectionService _detectionService;
        private DoublonActionService _actionService;
        private List<EmployeDoublonFedere> _doublonsDetectes;
        private BindingSource _bindingSource;

        public FormDoublonManager()
        {
            InitializeComponent();
            InitialiserFormulaire();
        }

        private void InitialiserFormulaire()
        {
            _bindingSource = new BindingSource();
            dgvDoublons.DataSource = _bindingSource;

            // Chargement des chaînes de connexion depuis la configuration
            var (settings39C, settings19M) = ConnectionHelper.LoadConnectionSettings();

            if (settings39C != null)
            {
                txtServer39C.Text = settings39C.Server;
                txtDatabase39C.Text = settings39C.Database;
                rbWindowsAuth39C.Checked = settings39C.UseWindowsAuth;
                rbSqlAuth39C.Checked = !settings39C.UseWindowsAuth;
                txtUser39C.Text = settings39C.Username;
                txtPassword39C.Text = settings39C.Password;
            }

            if (settings19M != null)
            {
                txtServer19M.Text = settings19M.Server;
                txtDatabase19M.Text = settings19M.Database;
                rbWindowsAuth19M.Checked = settings19M.UseWindowsAuth;
                rbSqlAuth19M.Checked = !settings19M.UseWindowsAuth;
                txtUser19M.Text = settings19M.Username;
                txtPassword19M.Text = settings19M.Password;
            }

            ToggleAuthFields(null, null);
            ConfigurerColonnesDataGridView();

            // États initiaux
            btnTraiterAutomatique.Enabled = false;
            btnModeSimulation.Enabled = false;
            btnExporterRapport.Enabled = false;
            progressBar.Visible = false;

            AjouterLog("✅ Application initialisée");
        }

        private void ToggleAuthFields(object sender, EventArgs e)
        {
            txtUser39C.Enabled = rbSqlAuth39C.Checked;
            txtPassword39C.Enabled = rbSqlAuth39C.Checked;
            
            txtUser19M.Enabled = rbSqlAuth19M.Checked;
            txtPassword19M.Enabled = rbSqlAuth19M.Checked;

            lblStatus39C.Text = "⚪ Non testé";
            lblStatus39C.ForeColor = Color.Gray;
            lblStatus19M.Text = "⚪ Non testé";
            lblStatus19M.ForeColor = Color.Gray;
        }

        private void ConfigurerColonnesDataGridView()
        {
            dgvDoublons.AutoGenerateColumns = false;
            dgvDoublons.Columns.Clear();

            dgvDoublons.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNom",
                DataPropertyName = "Nom",
                HeaderText = "Nom",
                Width = 120
            });

            dgvDoublons.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrenom",
                DataPropertyName = "Prenom",
                HeaderText = "Prénom",
                Width = 100
            });

            dgvDoublons.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSiteSource",
                DataPropertyName = "CodeSource",
                HeaderText = "Site 39C",
                Width = 80
            });

            dgvDoublons.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSiteDest",
                DataPropertyName = "CodeDest",
                HeaderText = "Site 19M",
                Width = 80
            });

            var colScoreSource = new DataGridViewTextBoxColumn
            {
                Name = "colScoreSource",
                DataPropertyName = "CompletudeSource",
                HeaderText = "Score 39C",
                Width = 70
            };
            colScoreSource.DefaultCellStyle.Format = "0\\%";
            dgvDoublons.Columns.Add(colScoreSource);

            var colScoreDest = new DataGridViewTextBoxColumn
            {
                Name = "colScoreDest",
                DataPropertyName = "CompletudeDest",
                HeaderText = "Score 19M",
                Width = 70
            };
            colScoreDest.DefaultCellStyle.Format = "0\\%";
            dgvDoublons.Columns.Add(colScoreDest);

            dgvDoublons.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPhotoSource",
                DataPropertyName = "HasPhotoSource",
                HeaderText = "Photo 39C",
                Width = 70
            });

            dgvDoublons.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPhotoDest",
                DataPropertyName = "HasPhotoDest",
                HeaderText = "Photo 19M",
                Width = 70
            });

            dgvDoublons.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDecision",
                DataPropertyName = "FicheAConserver",
                HeaderText = "Décision",
                Width = 100
            });

            dgvDoublons.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRaison",
                DataPropertyName = "RaisonDecision",
                HeaderText = "Raison",
                Width = 250
            });

            // Formatage personnalisé pour les colonnes booléennes (Photo) dans l'événement CellFormatting
            dgvDoublons.CellFormatting += DgvDoublons_CellFormatting;
        }

        private void DgvDoublons_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvDoublons.Columns[e.ColumnIndex].Name;
            if (colName == "colPhotoSource" || colName == "colPhotoDest")
            {
                if (e.Value is bool val)
                {
                    e.Value = val ? "✅ Oui" : "❌ Non";
                    e.FormattingApplied = true;
                }
            }
        }

        private async void btnDetecterDoublons_Click(object sender, EventArgs e)
        {
            try
            {
                DesactiverControles();
                AjouterLog("🔍 Lancement de la détection des doublons...");
                lblStatut.Text = "Détection en cours...";
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                string connStr39C = ConnectionHelper.BuildConnectionString(txtServer39C.Text, txtDatabase39C.Text, rbWindowsAuth39C.Checked, txtUser39C.Text, txtPassword39C.Text);
                string connStr19M = ConnectionHelper.BuildConnectionString(txtServer19M.Text, txtDatabase19M.Text, rbWindowsAuth19M.Checked, txtUser19M.Text, txtPassword19M.Text);

                _detectionService = new DoublonDetectionService(connStr39C, connStr19M);
                _detectionService.OnLog += AjouterLog;

                // Sauvegarder les configurations
                SauvegarderConfigurations();

                _doublonsDetectes = await _detectionService.DetecterDoublonsAsync();

                _bindingSource.DataSource = _doublonsDetectes;
                lblNombreDoublons.Text = $"Doublons détectés ({_doublonsDetectes.Count})";
                grpResults.Text = $"📊 Doublons détectés ({_doublonsDetectes.Count})";

                AjouterLog($"✅ {_doublonsDetectes.Count} doublons détectés");

                btnTraiterAutomatique.Enabled = _doublonsDetectes.Count > 0;
                btnModeSimulation.Enabled = _doublonsDetectes.Count > 0;
                btnExporterRapport.Enabled = _doublonsDetectes.Count > 0;

                AppliquerFormatageLignes();
                lblStatut.Text = "Détection terminée";
                progressBar.Visible = false;
            }
            catch (Exception ex)
            {
                AjouterLog($"❌ Erreur lors de la détection: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ReactiverControles();
            }
        }

        private void SauvegarderConfigurations()
        {
            var settings39C = new ConnectionSettings
            {
                Server = txtServer39C.Text,
                Database = txtDatabase39C.Text,
                UseWindowsAuth = rbWindowsAuth39C.Checked,
                Username = txtUser39C.Text,
                Password = txtPassword39C.Text
            };

            var settings19M = new ConnectionSettings
            {
                Server = txtServer19M.Text,
                Database = txtDatabase19M.Text,
                UseWindowsAuth = rbWindowsAuth19M.Checked,
                Username = txtUser19M.Text,
                Password = txtPassword19M.Text
            };

            ConnectionHelper.SaveConnectionSettings(settings39C, settings19M);
        }

        private void DgvDoublons_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDoublons.SelectedRows.Count == 0) return;

            var doublon = (EmployeDoublonFedere)dgvDoublons.SelectedRows[0].DataBoundItem;
            if (doublon == null) return;

            // Site Source
            lblBadgeSource.Text = $"Badge: {doublon.CodeSource}-{doublon.NumSource}";
            lblCompletudeSource.Text = $"Complétude: {doublon.CompletudeSource}%";
            lblPhotoSource.Text = $"Photo: {(doublon.HasPhotoSource ? "✅ Oui" : "❌ Non")}";
            lblDateSource.Text = $"Modif: {doublon.DateSource?.ToString("dd/MM/yyyy") ?? "N/A"}";

            // Site Dest
            lblBadgeDest.Text = $"Badge: {doublon.CodeDest}-{doublon.NumDest}";
            lblCompletudeDest.Text = $"Complétude: {doublon.CompletudeDest}%";
            lblPhotoDest.Text = $"Photo: {(doublon.HasPhotoDest ? "✅ Oui" : "❌ Non")}";
            lblDateDest.Text = $"Modif: {doublon.DateDest?.ToString("dd/MM/yyyy") ?? "N/A"}";

            // Décision
            switch (doublon.FicheAConserver)
            {
                case FicheReference.Source39C:
                    lblDecisionConserver.Text = "✅ Conserver: Site 39C";
                    lblDecisionSupprimer.Text = "🗑️ Supprimer: Site 19M";
                    break;
                case FicheReference.Destination19M:
                    lblDecisionConserver.Text = "✅ Conserver: Site 19M";
                    lblDecisionSupprimer.Text = "🗑️ Supprimer: Site 39C";
                    break;
                case FicheReference.EgaliteTemporelle:
                    lblDecisionConserver.Text = "⚠️ Égalité parfaite";
                    lblDecisionSupprimer.Text = "👤 Intervention manuelle requise";
                    break;
            }
            lblDecisionRaison.Text = $"📝 {doublon.RaisonDecision}";
        }

        private async void btnTraiterAutomatique_Click(object sender, EventArgs e)
        {
            var msg = $"⚠️ Vous êtes sur le point de traiter {_doublonsDetectes.Count} doublons.\n\n" +
                      "Cette opération va :\n" +
                      "• Supprimer les fiches en doublon\n" +
                      "• Supprimer les badges associés\n" +
                      "• Activer le double site sur les fiches conservées\n" +
                      "• Créer des sauvegardes JSON\n\n" +
                      "⚠️ Cette action est IRRÉVERSIBLE !\n\n" +
                      "Voulez-vous continuer ?";

            if (MessageBox.Show(msg, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            await LancerTraitement(false);
        }

        private async void btnModeSimulation_Click(object sender, EventArgs e)
        {
            AjouterLog("🧪 Lancement du mode SIMULATION...");
            AjouterLog("⚠️ Aucune modification ne sera effectuée en base");
            await LancerTraitement(true);
        }

        private async Task LancerTraitement(bool modeSimulation)
        {
            try
            {
                DesactiverControles();
                string connStr39C = ConnectionHelper.BuildConnectionString(txtServer39C.Text, txtDatabase39C.Text, rbWindowsAuth39C.Checked, txtUser39C.Text, txtPassword39C.Text);
                string connStr19M = ConnectionHelper.BuildConnectionString(txtServer19M.Text, txtDatabase19M.Text, rbWindowsAuth19M.Checked, txtUser19M.Text, txtPassword19M.Text);

                _actionService = new DoublonActionService(connStr39C, connStr19M);
                _actionService.OnLog += AjouterLog;
                _actionService.OnProgress += MettreAJourProgression;

                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Maximum = _doublonsDetectes.Count;
                progressBar.Value = 0;

                lblStatut.Text = modeSimulation ? "Simulation en cours..." : "Traitement en cours...";
                
                var resultat = await _actionService.TraiterDoublonsAsync(_doublonsDetectes, modeSimulation);
                AfficherResultatTraitement(resultat);

                if (!modeSimulation)
                {
                    await RefreshDoublonsAsync();
                }
            }
            catch (Exception ex)
            {
                AjouterLog($"❌ Erreur critique: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ReactiverControles();
                progressBar.Visible = false;
            }
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            bool is39C = btn == btnTestConnection39C;
            string site = is39C ? "39C" : "19M";
            Label lblStatus = is39C ? lblStatus39C : lblStatus19M;

            string connStr = is39C 
                ? ConnectionHelper.BuildConnectionString(txtServer39C.Text, txtDatabase39C.Text, rbWindowsAuth39C.Checked, txtUser39C.Text, txtPassword39C.Text)
                : ConnectionHelper.BuildConnectionString(txtServer19M.Text, txtDatabase19M.Text, rbWindowsAuth19M.Checked, txtUser19M.Text, txtPassword19M.Text);

            try
            {
                lblStatus.Text = "⏳ Test...";
                lblStatus.ForeColor = Color.Blue;
                
                bool result = await ConnectionHelper.TestConnection(connStr);
                if (result)
                {
                    lblStatus.Text = "✅ Réussi";
                    lblStatus.ForeColor = Color.Green;
                    AjouterLog($"✅ Test connexion {site} réussi");
                }
                else
                {
                    lblStatus.Text = "❌ Échec";
                    lblStatus.ForeColor = Color.Red;
                    AjouterLog($"❌ Échec test connexion {site}");
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Erreur";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"❌ Échec de la connexion au site {site} :\n{ex.Message}", "Erreur Connexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AjouterLog($"❌ Échec test connexion {site}: {ex.Message}");
            }
        }

        private async void btnRafraichir_Click(object sender, EventArgs e) => await RefreshDoublonsAsync();

        private void AjouterLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AjouterLog), message);
                return;
            }
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtLogs.AppendText($"[{timestamp}] {message}\n");
            txtLogs.SelectionStart = txtLogs.Text.Length;
            txtLogs.ScrollToCaret();
        }

        private void MettreAJourProgression(int actuel, int total)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, int>(MettreAJourProgression), actuel, total);
                return;
            }
            progressBar.Maximum = total;
            progressBar.Value = actuel;
            int pourcentage = total > 0 ? (int)((double)actuel / total * 100) : 0;
            lblStatut.Text = $"Traitement: {actuel}/{total} ({pourcentage}%)";
        }

        private void AfficherResultatTraitement(ResultatTraitement resultat)
        {
            string message = (resultat.Succes ? "✅" : "⚠️") + " Traitement terminé " + (resultat.Succes ? "avec succès" : "avec erreurs") + " !\n\n" +
                             $"📊 Résumé :\n" +
                             $"✓ {resultat.NombreFichesTraitees} fiches traitées\n" +
                             $"✓ {resultat.NombreFichesConservees} fiches conservées\n" +
                             $"✓ {resultat.NombreFichesSupprimees} fiches supprimées\n" +
                             $"✓ {resultat.NombreErreurs} erreurs\n\n" +
                             $"📁 {resultat.FichiersSauvegardes.Count} sauvegardes créées.";

            AjouterLog(message);
            MessageBox.Show(message, "Résultat du traitement", MessageBoxButtons.OK, resultat.Succes ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void AppliquerFormatageLignes()
        {
            foreach (DataGridViewRow row in dgvDoublons.Rows)
            {
                var doublon = (EmployeDoublonFedere)row.DataBoundItem;
                if (doublon.FicheAConserver == FicheReference.EgaliteTemporelle)
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                else
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
            }
        }

        private void DesactiverControles()
        {
            grpConfig.Enabled = false;
            btnTraiterAutomatique.Enabled = false;
            btnModeSimulation.Enabled = false;
            btnExporterRapport.Enabled = false;
            btnRafraichir.Enabled = false;
        }

        private void ReactiverControles()
        {
            grpConfig.Enabled = true;
            btnTraiterAutomatique.Enabled = _doublonsDetectes?.Count > 0;
            btnModeSimulation.Enabled = _doublonsDetectes?.Count > 0;
            btnExporterRapport.Enabled = _doublonsDetectes?.Count > 0;
            btnRafraichir.Enabled = true;
        }

        private async Task RefreshDoublonsAsync()
        {
            AjouterLog("🔄 Rafraîchissement de la liste...");
            btnDetecterDoublons_Click(null, null);
            await Task.CompletedTask;
        }
    }
}
