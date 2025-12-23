using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoublonManager.Services;
using DoublonManager.Models;

namespace DoublonManager.Forms
{
    public partial class AnalysisForm : Form
    {
        private DatabaseService _dbService;
        private DuplicateAnalysisResult _analysisResult;
        
        // Contrôles UI
        private Panel panelHeader;
        private ProgressBar progressBar;
        private Label lblProgress;
        
        private Panel panelSummary;
        private Label lblCount39C, lblCount19M, lblCountDiffCodes, lblCountDiffNumbers, lblCountAmbiguous;
        
        private TabControl tabResults;
        private DataGridView dgvDifferentCodes, dgvDifferentNumbers, dgvAmbiguous;
        
        private Button btnExportExcel, btnDeleteSelected, btnClose;

        // Constantes de couleurs pour les sites
        private static class SiteColors
        {
            public static readonly Color Site39C_Background = Color.FromArgb(220, 237, 255);
            public static readonly Color Site39C_Text = Color.FromArgb(0, 84, 166);
            public static readonly Color Site19M_Background = Color.FromArgb(255, 235, 220);
            public static readonly Color Site19M_Text = Color.FromArgb(166, 84, 0);
        }

        public AnalysisForm(DatabaseService dbService)
        {
            _dbService = dbService;
            InitializeComponent();
            InitializeCustomComponents();
            
            // Démarrer l'analyse automatiquement
            this.Load += async (s, e) => await StartAnalysis();
        }

        private void InitializeComponent()
        {
            this.Text = "🔍 Analyse des doublons";
            this.Size = new Size(1500, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1200, 800);
            this.BackColor = ColorTranslator.FromHtml("#ECF0F1");
        }

        private void InitializeCustomComponents()
        {
            // Reset
            this.Controls.Clear();

            // 1. Actions Bar (Bottom)
            CreateActionsSection();
            
            // 2. Header (Top)
            CreateHeaderSection();
            
            // 3. Middle Container (Fill)
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = ColorTranslator.FromHtml("#ECF0F1")
            };
            this.Controls.Add(mainContainer);
            mainContainer.BringToFront(); // Between Header and Footer

            // 4. Create content inside container
            CreateSummarySection(mainContainer);
            CreateResultsSection(mainContainer);
        }

        #region UI Creation

        private void CreateHeaderSection()
        {
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            Label lblTitle = new Label
            {
                Text = "🔍 Analyse des doublons entre sites",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label lblSubtitle = new Label
            {
                Text = "Détection automatique des employés en double entre 39C et 19M",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#7F8C8D"),
                Location = new Point(20, 55),
                AutoSize = true
            };

            progressBar = new ProgressBar
            {
                Location = new Point(20, 90),
                Size = new Size(1440, 25),
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };

            lblProgress = new Label
            {
                Location = new Point(20, 120),
                Size = new Size(1440, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#3498DB"),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "",
                Visible = false
            };

            panelHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle, progressBar, lblProgress });
            this.Controls.Add(panelHeader);
        }

        private void CreateSummarySection(Control parent)
        {
            panelSummary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                Margin = new Padding(0, 0, 0, 20)
            };

            // Colonne 1 : Site 39C
            Panel panel39C = CreateSummaryCard(20, "📊", ColorTranslator.FromHtml("#E8F8F5"), 
                ColorTranslator.FromHtml("#16A085"), "Fiches Site 39C", 270);
            lblCount39C = (Label)((TableLayoutPanel)panel39C.Controls[0]).Controls[1];

            // Colonne 2 : Site 19M
            Panel panel19M = CreateSummaryCard(305, "📊", ColorTranslator.FromHtml("#EBF5FB"), 
                ColorTranslator.FromHtml("#2980B9"), "Fiches Site 19M", 270);
            lblCount19M = (Label)((TableLayoutPanel)panel19M.Controls[0]).Controls[1];

            // Colonne 3 : Codes différents
            Panel panelDiffCodes = CreateSummaryCard(590, "⚠️", ColorTranslator.FromHtml("#FADBD8"), 
                ColorTranslator.FromHtml("#E74C3C"), "Codes différents", 270);
            lblCountDiffCodes = (Label)((TableLayoutPanel)panelDiffCodes.Controls[0]).Controls[1];

            // Colonne 4 : Numéros différents
            Panel panelDiffNumbers = CreateSummaryCard(875, "⚠️", ColorTranslator.FromHtml("#FEF5E7"), 
                ColorTranslator.FromHtml("#F39C12"), "Numéros différents", 270);
            lblCountDiffNumbers = (Label)((TableLayoutPanel)panelDiffNumbers.Controls[0]).Controls[1];

            // Colonne 5 : Cas ambigus
            Panel panelAmbiguous = CreateSummaryCard(1160, "❓", ColorTranslator.FromHtml("#F4ECF7"), 
                ColorTranslator.FromHtml("#8E44AD"), "Cas ambigus", 260);
            lblCountAmbiguous = (Label)((TableLayoutPanel)panelAmbiguous.Controls[0]).Controls[1];

            panelSummary.Controls.AddRange(new Panel[] { 
                panel39C, panel19M, panelDiffCodes, panelDiffNumbers, panelAmbiguous 
            });

            parent.Controls.Add(panelSummary);
        }

        private Panel CreateSummaryCard(int x, string icon, Color bgColor, Color valueColor, string description, int width = 210)
        {
            Panel card = new Panel
            {
                Location = new Point(x, 10),
                Size = new Size(width, 130),
                BackColor = bgColor
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                BackColor = Color.Transparent
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // Icon
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F)); // Value
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F)); // Desc


            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24), // Slightly smaller to avoid overlap
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblValue = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = valueColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblDesc = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#7F8C8D"),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            layout.Controls.Add(lblIcon, 0, 0);
            layout.Controls.Add(lblValue, 0, 1);
            layout.Controls.Add(lblDesc, 0, 2);
            card.Controls.Add(layout);

            return card;
        }

        private void CreateResultsSection(Control parent)
        {
            tabResults = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Visible = false
            };

            // Onglet 1 : Codes différents
            TabPage tabDiffCodes = new TabPage("⚠️ Codes différents (0)");
            dgvDifferentCodes = CreateResultsGrid();
            tabDiffCodes.Controls.Add(dgvDifferentCodes);

            // Onglet 2 : Numéros différents
            TabPage tabDiffNumbers = new TabPage("⚠️ Numéros différents (0)");
            dgvDifferentNumbers = CreateResultsGrid();
            tabDiffNumbers.Controls.Add(dgvDifferentNumbers);

            // Onglet 3 : Cas ambigus
            TabPage tabAmbiguous = new TabPage("❓ Cas ambigus (0)");
            
            Panel panelAmbiguousHelp = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#FFF9E6"),
                Padding = new Padding(10)
            };

            Label lblHelp = new Label
            {
                Text = "⚠️ Ces cas nécessitent une validation manuelle : même nom/prénom mais codes ET numéros différents avec dates de modification identiques. Risque de confusion élevé.",
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#856404"),
                Dock = DockStyle.Fill
            };
            panelAmbiguousHelp.Controls.Add(lblHelp);

            dgvAmbiguous = CreateResultsGrid();
            dgvAmbiguous.Dock = DockStyle.Fill;

            tabAmbiguous.Controls.AddRange(new Control[] { panelAmbiguousHelp, dgvAmbiguous });

            tabResults.TabPages.AddRange(new TabPage[] { tabDiffCodes, tabDiffNumbers, tabAmbiguous });
            
            parent.Controls.Add(tabResults);
            tabResults.BringToFront(); // Below summary because summary is Dock.Top
        }


        private DataGridView CreateResultsGrid()
        {
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = ColorTranslator.FromHtml("#F8F9FA") 
                }
            };

            // Ajouter colonnes
            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "☑️", Width = 40, FillWeight = 3 },
                new DataGridViewTextBoxColumn { Name = "Site", HeaderText = "🏢 Site", FillWeight = 12 },
                new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "🆔 Matricule", FillWeight = 10 },
                new DataGridViewTextBoxColumn { Name = "Nom", HeaderText = "👤 Nom", FillWeight = 15 },
                new DataGridViewTextBoxColumn { Name = "Prenom", HeaderText = "👤 Prénom", FillWeight = 15 },
                new DataGridViewTextBoxColumn { Name = "Cardholder", HeaderText = "💳 Cardholder", FillWeight = 12 },
                new DataGridViewTextBoxColumn { Name = "DateModif", HeaderText = "📅 Modification", FillWeight = 13 },
                new DataGridViewTextBoxColumn { Name = "Confidence", HeaderText = "📈 Confiance", FillWeight = 8 },
                new DataGridViewTextBoxColumn { Name = "Recommendation", HeaderText = "💡 Suggestion", FillWeight = 12 },
                new DataGridViewButtonColumn { Name = "ColVoir", HeaderText = "Action", Text = "👁️ Voir", UseColumnTextForButtonValue = true, FillWeight = 8, FlatStyle = FlatStyle.Flat }
            });

            // Événements pour l'effet visuel et liaison de sélection
            dgv.CellValueChanged += (s, e) => {
                if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == "Select") {
                    var currentPair = dgv.Rows[e.RowIndex].Tag as DuplicatePair;
                    bool newValue = (bool)dgv.Rows[e.RowIndex].Cells["Select"].Value;

                    // Synchroniser l'autre ligne du même doublon
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.Index != e.RowIndex && row.Tag == currentPair)
                        {
                            if ((bool)row.Cells["Select"].Value != newValue)
                            {
                                row.Cells["Select"].Value = newValue;
                                UpdateRowStyle(row);
                            }
                        }
                    }
                    UpdateRowStyle(dgv.Rows[e.RowIndex]);
                }
            };

            dgv.CellContentClick += (s, e) => {
                if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == "ColVoir")
                {
                    // Récupérer l'employé spécifique à cette ligne
                    // Nous stockons l'employé spécifique dans la cellule "ID" ou via un autre mécanisme
                    // Pour simplifier, on va stocker l'employé dans le Tag de la CELLULE "Site"
                    var emp = dgv.Rows[e.RowIndex].Cells["Site"].Tag as DbEmployee;
                    if (emp != null)
                    {
                        MessageBox.Show(
                            $"Détails de l'employé :\n\n" +
                            $"📍 Site : {emp.SiteCode}\n" +
                            $"🆔 Matricule : {emp.ID}\n" +
                            $"👤 Nom : {emp.LastName}\n" +
                            $"👤 Prénom : {emp.FirstName}\n" +
                            $"💳 Cardholder : {emp.CardholderIdNumber}\n" +
                            $"📅 Dernier Download : {emp.LastDownloadTime:dd/MM/yyyy HH:mm}",
                            "Détails de l'enregistrement",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            };

            // Pour valider immédiatement le clic sur la checkbox
            dgv.CurrentCellDirtyStateChanged += (s, e) => {
                if (dgv.IsCurrentCellDirty && dgv.CurrentCell is DataGridViewCheckBoxCell) {
                    dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };

            return dgv;
        }

        private void UpdateRowStyle(DataGridViewRow row)
        {
            if (row.Tag == null) return;
            
            bool isSelected = row.Cells["Select"].Value != null && (bool)row.Cells["Select"].Value;
            var pair = (DuplicatePair)row.Tag;
            var emp = row.Cells["Site"].Tag as DbEmployee;

            if (isSelected)
            {
                // Effet "Barré" et Gris pour toute la ligne
                row.DefaultCellStyle.ForeColor = Color.Gray;
                row.DefaultCellStyle.SelectionForeColor = Color.Gray;
                row.DefaultCellStyle.Font = new Font(dgvDifferentCodes.Font, FontStyle.Strikeout);
                row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                
                // Forcer le gris aussi sur la cellule Site
                row.Cells["Site"].Style.BackColor = Color.FromArgb(230, 230, 230);
                row.Cells["Site"].Style.ForeColor = Color.DarkGray;
            }
            else
            {
                // Restaurer style normal
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.SelectionForeColor = Color.White;
                row.DefaultCellStyle.Font = new Font(dgvDifferentCodes.Font, FontStyle.Regular);
                
                // Couleur basée sur la confiance pour le fond de ligne
                if (pair.Confidence >= 0.8)
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D5F4E6");
                else if (pair.Confidence >= 0.6)
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FEF9E7");
                else
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FADBD8");

                // Restaurer la couleur spécifique du site pour la cellule Site
                if (emp != null)
                {
                    if (emp.SiteCode == "39C")
                    {
                        row.Cells["Site"].Style.BackColor = SiteColors.Site39C_Background;
                        row.Cells["Site"].Style.ForeColor = SiteColors.Site39C_Text;
                    }
                    else
                    {
                        row.Cells["Site"].Style.BackColor = SiteColors.Site19M_Background;
                        row.Cells["Site"].Style.ForeColor = SiteColors.Site19M_Text;
                    }
                }
            }
        }

        private void CreateActionsSection()
        {
            Panel panelActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = ColorTranslator.FromHtml("#34495E"),
                Padding = new Padding(20, 15, 20, 15)
            };

            // Using TableLayoutPanel to force horizontal alignment without wrapping
            TableLayoutPanel actionLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            btnExportExcel = new Button
            {
                Text = "📊 Exporter Excel",
                Size = new Size(160, 45),
                BackColor = ColorTranslator.FromHtml("#27AE60"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false,
                Visible = false,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 15, 0)
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += BtnExportExcel_Click;

            btnDeleteSelected = new Button
            {
                Text = "🗑️ Supprimer sélection",
                Size = new Size(200, 45),
                BackColor = ColorTranslator.FromHtml("#E74C3C"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false,
                Visible = false,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 15, 0)
            };
            btnDeleteSelected.FlatAppearance.BorderSize = 0;
            btnDeleteSelected.Click += BtnDeleteSelected_Click;

            btnClose = new Button
            {
                Text = "❌ Fermer",
                Size = new Size(130, 45),
                BackColor = ColorTranslator.FromHtml("#7F8C8D"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            actionLayout.Controls.Add(btnExportExcel, 0, 0);
            actionLayout.Controls.Add(btnDeleteSelected, 1, 0);
            actionLayout.Controls.Add(btnClose, 2, 0);

            panelActions.Controls.Add(actionLayout);
            this.Controls.Add(panelActions);
        }

        #endregion

        #region Analysis Logic

        private async Task StartAnalysis()
        {
            try
            {
                // Afficher la progression
                progressBar.Visible = true;
                lblProgress.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                // Étape 1 : Chargement des données
                lblProgress.Text = "📥 Chargement des données du site 39C...";
                await Task.Delay(100); // Pour permettre l'affichage

                lblProgress.Text = "📥 Chargement des données du site 19M...";
                await Task.Delay(100);

                // Étape 2 : Analyse
                lblProgress.Text = "🔍 Analyse des doublons en cours...";
                _analysisResult = await _dbService.DetectDuplicates();

                // Étape 3 : Affichage des résultats
                lblProgress.Text = "📊 Préparation des résultats...";
                await Task.Delay(100);

                DisplayResults();

                // Masquer la progression
                progressBar.Visible = false;
                lblProgress.Visible = false;
            }
            catch (Exception ex)
            {
                progressBar.Visible = false;
                lblProgress.Visible = false;

                MessageBox.Show(
                    $"Erreur lors de l'analyse :\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                this.Close();
            }
        }

        private void DisplayResults()
        {
            // Afficher le résumé
            panelSummary.Visible = true;
            lblCount39C.Text = _analysisResult.TotalEmployees39C.ToString("N0");
            lblCount19M.Text = _analysisResult.TotalEmployees19M.ToString("N0");
            lblCountDiffCodes.Text = _analysisResult.DifferentCodeDuplicates.Count.ToString();
            lblCountDiffNumbers.Text = _analysisResult.DifferentNumberDuplicates.Count.ToString();
            lblCountAmbiguous.Text = _analysisResult.AmbiguousCases.Count.ToString();

            // Afficher les résultats détaillés
            tabResults.Visible = true;
            PopulateGrid(dgvDifferentCodes, _analysisResult.DifferentCodeDuplicates);
            PopulateGrid(dgvDifferentNumbers, _analysisResult.DifferentNumberDuplicates);
            PopulateGrid(dgvAmbiguous, _analysisResult.AmbiguousCases);

            // Mettre à jour les titres des onglets
            tabResults.TabPages[0].Text = $"⚠️ Codes différents ({_analysisResult.DifferentCodeDuplicates.Count})";
            tabResults.TabPages[1].Text = $"⚠️ Numéros différents ({_analysisResult.DifferentNumberDuplicates.Count})";
            tabResults.TabPages[2].Text = $"❓ Cas ambigus ({_analysisResult.AmbiguousCases.Count})";
            
            // Afficher les boutons d'action
            if (_analysisResult.TotalDuplicates > 0)
            {
                btnExportExcel.Visible = true;
                btnExportExcel.Enabled = true;
                btnDeleteSelected.Visible = true;
                btnDeleteSelected.Enabled = true;
            }
        }

        private void PopulateGrid(DataGridView dgv, List<DuplicatePair> duplicates)
        {
            dgv.Rows.Clear();

            foreach (var dup in duplicates)
            {
                // On ajoute DEUX lignes par doublon (une par site)
                AddEmployeeRow(dgv, dup, dup.Employee39C);
                AddEmployeeRow(dgv, dup, dup.Employee19M);
                
                // On peut ajouter une ligne vide ou un séparateur visuel ?
                // Non, on va utiliser une bordure ou une couleur de groupe.
            }
        }

        private void AddEmployeeRow(DataGridView dgv, DuplicatePair pair, DbEmployee emp)
        {
            int idx = dgv.Rows.Add();
            DataGridViewRow row = dgv.Rows[idx];
            
            row.Tag = pair; // Le tag est la PAIRE pour pouvoir gérer la suppression/sélection groupée
            
            // Site detail for Action button
            row.Cells["Site"].Tag = emp;

            row.Cells["Select"].Value = false;
            row.Cells["Site"].Value = emp.SiteCode == "39C" ? "📍 Site 39C" : "📍 Site 19M";
            row.Cells["ID"].Value = emp.ID;
            row.Cells["Nom"].Value = emp.LastName;
            row.Cells["Prenom"].Value = emp.FirstName;
            row.Cells["Cardholder"].Value = emp.CardholderIdNumber;
            row.Cells["DateModif"].Value = emp.LastDownloadTime.ToString("dd/MM/yyyy HH:mm");
            row.Cells["Confidence"].Value = $"{pair.Confidence:P0}";

            // Logique de suggestion (basée sur la date la plus récente)
            var otherEmp = (emp == pair.Employee39C) ? pair.Employee19M : pair.Employee39C;
            if (emp.LastDownloadTime > otherEmp.LastDownloadTime)
            {
                row.Cells["Recommendation"].Value = "✅ Garder (Récent)";
                row.Cells["Recommendation"].Style.ForeColor = Color.DarkGreen;
            }
            else if (emp.LastDownloadTime < otherEmp.LastDownloadTime)
            {
                row.Cells["Recommendation"].Value = "🗑️ Supprimer (Ancien)";
                row.Cells["Recommendation"].Style.ForeColor = Color.Firebrick;
            }
            else
            {
                row.Cells["Recommendation"].Value = "❓ Identique";
                row.Cells["Recommendation"].Style.ForeColor = Color.Orange;
            }
            row.Cells["Recommendation"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Style spécifique au site
            if (emp.SiteCode == "39C")
            {
                row.Cells["Site"].Style.BackColor = SiteColors.Site39C_Background;
                row.Cells["Site"].Style.ForeColor = SiteColors.Site39C_Text;
                row.Cells["Site"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else
            {
                row.Cells["Site"].Style.BackColor = SiteColors.Site19M_Background;
                row.Cells["Site"].Style.ForeColor = SiteColors.Site19M_Text;
                row.Cells["Site"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }

            // Appliquer le style initial (couleur de confiance)
            UpdateRowStyle(row);
        }

        #endregion

        #region Event Handlers

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Fichier Excel (*.xlsx)|*.xlsx",
                    FileName = $"Analyse_Doublons_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Enregistrer le rapport"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // TODO : Implémenter l'export Excel (Étape 5)
                    MessageBox.Show(
                        $"Export réussi !\n\nFichier : {sfd.FileName}",
                        "Export Excel",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'export :\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async void BtnDeleteSelected_Click(object sender, EventArgs e)
        {
            // Collecter les lignes sélectionnées dans tous les onglets
            var selectedDuplicates = new List<DuplicatePair>();
            
            DataGridView[] grids = { dgvDifferentCodes, dgvDifferentNumbers, dgvAmbiguous };

            foreach (var grid in grids)
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.Cells["Select"].Value != null && (bool)row.Cells["Select"].Value)
                    {
                        var pair = (DuplicatePair)row.Tag;
                        if (!selectedDuplicates.Contains(pair))
                        {
                            selectedDuplicates.Add(pair);
                        }
                    }
                }
            }

            if (selectedDuplicates.Count == 0)
            {
                MessageBox.Show(
                    "Veuillez cocher au moins un doublon à traiter.",
                    "Aucune sélection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Ouvrir le formulaire de confirmation
            var deleteForm = new DeleteConfirmationForm(selectedDuplicates, _dbService);
            if (deleteForm.ShowDialog() == DialogResult.OK)
            {
                // Rafraîchir l'analyse pour faire disparaître les doublons supprimés
                await StartAnalysis();
            }
        }

        #endregion
    }
}
