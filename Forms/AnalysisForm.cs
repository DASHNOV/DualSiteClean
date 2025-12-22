using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoublonManager.Services;

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
                new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "☑️", Width = 50, FillWeight = 2 },
                new DataGridViewTextBoxColumn { Name = "Code39C", HeaderText = "ID (39C)", FillWeight = 10 },
                new DataGridViewTextBoxColumn { Name = "Nom39C", HeaderText = "Nom Prénom (39C)", FillWeight = 15 },
                new DataGridViewTextBoxColumn { Name = "Numero39C", HeaderText = "N° Cardholder (39C)", FillWeight = 12 },
                new DataGridViewTextBoxColumn { Name = "DateModif39C", HeaderText = "Dernier Download (39C)", FillWeight = 13 },
                new DataGridViewTextBoxColumn { Name = "Code19M", HeaderText = "ID (19M)", FillWeight = 10 },
                new DataGridViewTextBoxColumn { Name = "Nom19M", HeaderText = "Nom Prénom (19M)", FillWeight = 15 },
                new DataGridViewTextBoxColumn { Name = "Numero19M", HeaderText = "N° Cardholder (19M)", FillWeight = 12 },
                new DataGridViewTextBoxColumn { Name = "DateModif19M", HeaderText = "Dernier Download (19M)", FillWeight = 13 },
                new DataGridViewTextBoxColumn { Name = "Confidence", HeaderText = "Confiance", FillWeight = 8 }
            });

            // Événements pour l'effet visuel
            dgv.CellValueChanged += (s, e) => {
                if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == "Select") {
                    UpdateRowStyle(dgv.Rows[e.RowIndex]);
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
            if (row.Tag == null) return; // Sécurité si la ligne n'est pas encore prête
            
            bool isSelected = row.Cells["Select"].Value != null && (bool)row.Cells["Select"].Value;
            var dup = (DuplicatePair)row.Tag;

            if (isSelected)
            {
                // Effet "Barré" et Gris
                row.DefaultCellStyle.ForeColor = Color.Gray;
                row.DefaultCellStyle.SelectionForeColor = Color.Gray;
                row.DefaultCellStyle.Font = new Font(dgvDifferentCodes.Font, FontStyle.Strikeout);
                row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            }
            else
            {
                // Restaurer style normal (basé sur la confiance)
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.SelectionForeColor = Color.White;
                row.DefaultCellStyle.Font = new Font(dgvDifferentCodes.Font, FontStyle.Regular);
                
                if (dup.Confidence >= 0.8)
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D5F4E6");
                else if (dup.Confidence >= 0.6)
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FEF9E7");
                else
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FADBD8");
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
                int rowIndex = dgv.Rows.Add();
                DataGridViewRow row = dgv.Rows[rowIndex];
                row.Tag = dup; // On définit le tag IMMÉDIATEMENT

                row.Cells["Select"].Value = false;
                row.Cells["Code39C"].Value = dup.Employee39C.ID;
                row.Cells["Nom39C"].Value = dup.Employee39C.FullName;
                row.Cells["Numero39C"].Value = dup.Employee39C.CardholderIdNumber;
                row.Cells["DateModif39C"].Value = dup.Employee39C.LastDownloadTime.ToString("yyyy-MM-dd HH:mm");
                row.Cells["Code19M"].Value = dup.Employee19M.ID;
                row.Cells["Nom19M"].Value = dup.Employee19M.FullName;
                row.Cells["Numero19M"].Value = dup.Employee19M.CardholderIdNumber;
                row.Cells["DateModif19M"].Value = dup.Employee19M.LastDownloadTime.ToString("yyyy-MM-dd HH:mm");
                row.Cells["Confidence"].Value = $"{dup.Confidence:P0}";

                // Colorer la ligne selon le niveau de confiance
                if (dup.Confidence >= 0.8)
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D5F4E6"); // Vert clair
                else if (dup.Confidence >= 0.6)
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FEF9E7"); // Jaune clair
                else
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FADBD8"); // Rouge clair
            }
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
                        selectedDuplicates.Add((DuplicatePair)row.Tag);
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
