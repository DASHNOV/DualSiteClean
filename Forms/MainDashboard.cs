using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using DoublonManager.Helpers;
using DoublonManager.Models;

namespace DoublonManager.Forms
{
    public partial class MainDashboard : Form
    {
        private Panel pnlSite39C;
        private Panel pnlSite19M;
        private Panel pnlLastAnalysis;
        private Button btnAnalyze;
        private Button btnHistory;
        private ListView lvDuplicateStats;

        public MainDashboard()
        {
            InitializeCustomComponents();
            LoadDashboardData();
        }

        private void InitializeCustomComponents()
        {
            // 1. Taille de la fenêtre
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(1200, 700); // Empêcher le redimensionnement trop petit
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Gestionnaire de Doublons Multi-Sites";
            this.BackColor = ColorHelper.Neutral;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 2. Overview Panels
            CreateOverviewPanels();

            // 3. Duplicates Section
            CreateDuplicatesSection();

            // 4. Action Buttons
            CreateActionButtons();
        }

        private void CreateOverviewPanels()
        {
            // Panel Site 39C
            pnlSite39C = CreateOverviewPanel("📍", "Site 39C", "1,245 fiches", 30);
            
            // Panel Site 19M
            pnlSite19M = CreateOverviewPanel("📍", "Site 19M", "892 fiches", 390);
            
            // Panel Dernière analyse
            pnlLastAnalysis = CreateOverviewPanel("🔄", "Dernière analyse", "Aujourd'hui 09:00", 750);

            this.Controls.Add(pnlSite39C);
            this.Controls.Add(pnlSite19M);
            this.Controls.Add(pnlLastAnalysis);
        }

        private Panel CreateOverviewPanel(string iconText, string titleText, string valueText, int xPosition)
        {
            Panel panel = new Panel
            {
                Size = new Size(340, 100),
                Location = new Point(xPosition, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Icône en haut à gauche
            Label lblIcon = new Label
            {
                Text = iconText, // "📍" ou "🔄"
                Font = new Font("Segoe UI", 24f, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            // Titre à côté de l'icône - Décalé pour éviter le chevauchement
            Label lblTitle = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = ColorHelper.Primary,
                AutoSize = false,
                Size = new Size(235, 30),
                Location = new Point(90, 20), // X augmenté de 60 à 90
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Valeur en dessous - Alignée avec le titre
            Label lblValue = new Label
            {
                Text = valueText,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = ColorHelper.Primary,
                AutoSize = false,
                Size = new Size(235, 35),
                Location = new Point(90, 55), // X augmenté de 15 à 90 pour alignement propre
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.Add(lblIcon);
            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblValue);

            return panel;
        }

        private void CreateDuplicatesSection()
        {
            // Panel conteneur
            Panel pnlDuplicatesContainer = new Panel
            {
                Size = new Size(1140, 280), // Plus large et plus haut
                Location = new Point(30, 220),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Titre de la section
            Label lblTitle = new Label
            {
                Text = "⚠️ Doublons détectés",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = ColorHelper.Primary,
                AutoSize = false,
                Size = new Size(300, 30),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ListView pour les statistiques
            lvDuplicateStats = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                HeaderStyle = ColumnHeaderStyle.None,
                Size = new Size(1100, 210),
                Location = new Point(20, 55),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11f)
            };

            // Colonnes
            lvDuplicateStats.Columns.Add("Type", 600);
            lvDuplicateStats.Columns.Add("Nombre", 100, HorizontalAlignment.Right);

            // Items avec icônes colorées
            ListViewItem item1 = new ListViewItem("🔴  Codes différents");
            item1.SubItems.Add("12");
            item1.ForeColor = ColorHelper.Danger;
            item1.Font = new Font("Segoe UI", 11f, FontStyle.Regular);

            ListViewItem item2 = new ListViewItem("🟡  Numéros différents");
            item2.SubItems.Add("5");
            item2.ForeColor = ColorHelper.Warning;
            item2.Font = new Font("Segoe UI", 11f, FontStyle.Regular);

            ListViewItem item3 = new ListViewItem("🟠  Cas ambigus");
            item3.SubItems.Add("3");
            item3.ForeColor = Color.FromArgb(230, 126, 34); // Orange foncé
            item3.Font = new Font("Segoe UI", 11f, FontStyle.Regular);

            lvDuplicateStats.Items.AddRange(new[] { item1, item2, item3 });

            pnlDuplicatesContainer.Controls.Add(lblTitle);
            pnlDuplicatesContainer.Controls.Add(lvDuplicateStats);
            
            this.Controls.Add(pnlDuplicatesContainer);
        }

        private void CreateActionButtons()
        {
            // Bouton "Lancer une analyse"
            btnAnalyze = new Button
            {
                Text = "🔍  Lancer une analyse",
                Size = new Size(220, 50),
                Location = new Point(30, 520),
                BackColor = ColorHelper.Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAnalyze.FlatAppearance.BorderSize = 0;
            btnAnalyze.Click += btnAnalyze_Click;

            // Bouton "Voir l'historique"
            btnHistory = new Button
            {
                Text = "📁  Voir l'historique",
                Size = new Size(220, 50),
                Location = new Point(270, 520),
                BackColor = ColorHelper.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnHistory.FlatAppearance.BorderSize = 0;
            btnHistory.Click += btnHistory_Click;

            // Add Hover Effects
            AddHoverEffect(btnAnalyze, ColorHelper.Blue);
            AddHoverEffect(btnHistory, ColorHelper.Gray);

            this.Controls.Add(btnAnalyze);
            this.Controls.Add(btnHistory);
        }

        private void AddHoverEffect(Button btn, Color normalColor)
        {
            Color hoverColor = ControlPaint.Dark(normalColor, 0.1f);
            
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;
        }

        private void LoadDashboardData()
        {
            // Mock data loading
        }

        private async void btnAnalyze_Click(object? sender, EventArgs e)
        {
            btnAnalyze.Text = "Analyse en cours...";
            btnAnalyze.Enabled = false;

            try 
            {
                // Instantiate Services (Dependency Injection would be better in larger app)
                var dbService = new DoublonManager.Services.DatabaseService("conn39C_placeholder", "conn19M_placeholder");
                var analysisService = new DoublonManager.Services.AnalysisService(dbService);

                var results = await analysisService.AnalyzeDuplicates();

                // Reset button
                btnAnalyze.Text = "🔍  Lancer une analyse";
                btnAnalyze.Enabled = true;

                // Open results
                var resultForm = new AnalysisResults(results);
                resultForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'analyse : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnAnalyze.Text = "🔍  Lancer une analyse";
                btnAnalyze.Enabled = true;
            }
        }

        private void btnHistory_Click(object? sender, EventArgs e)
        {
             var historyForm = new HistoryForm();
             historyForm.ShowDialog();
        }
    }
}
