using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using DoublonManager.Models;
using DoublonManager.Helpers;

namespace DoublonManager.Forms
{
    public partial class AnalysisResults : Form
    {
        private readonly AnalysisResult _result;
        private DataGridView dgvDuplicates;
        private Label lblSummary;
        private Button btnExport;
        private Button btnClose;

        public AnalysisResults(AnalysisResult result)
        {
            _result = result;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Résultats de l'analyse";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorHelper.Neutral;

            // Layout
            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(20);
            layout.RowCount = 3;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Header
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Grid
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Actions
            this.Controls.Add(layout);

            // Header
            lblSummary = new Label();
            lblSummary.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSummary.ForeColor = ColorHelper.Primary;
            lblSummary.AutoSize = true;
            layout.Controls.Add(lblSummary, 0, 0);

            // Grid
            dgvDuplicates = new DataGridView();
            dgvDuplicates.Dock = DockStyle.Fill;
            dgvDuplicates.BackgroundColor = Color.White;
            dgvDuplicates.BorderStyle = BorderStyle.None;
            dgvDuplicates.RowHeadersVisible = false;
            dgvDuplicates.AllowUserToAddRows = false;
            dgvDuplicates.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDuplicates.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Columns
            dgvDuplicates.Columns.Add("Nom", "Nom Prénom");
            dgvDuplicates.Columns.Add("39C_Code", "39C Code");
            dgvDuplicates.Columns.Add("19M_Code", "19M Code");
            dgvDuplicates.Columns.Add("39C_Num", "39C Num");
            dgvDuplicates.Columns.Add("19M_Num", "19M Num");
            dgvDuplicates.Columns.Add("Status", "Statut");
            dgvDuplicates.Columns.Add("Recommendation", "Recommandation");

            layout.Controls.Add(dgvDuplicates, 0, 1);

            // Actions
            var pnlActions = new FlowLayoutPanel();
            pnlActions.Dock = DockStyle.Fill;
            pnlActions.FlowDirection = FlowDirection.RightToLeft;
            
            btnClose = new Button();
            btnClose.Text = "Fermer";
            btnClose.Size = new Size(100, 35);
            btnClose.BackColor = ColorHelper.Gray;
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += (s, e) => this.Close();

            btnExport = new Button();
            btnExport.Text = "Exporter PDF";
            btnExport.Size = new Size(120, 35);
            btnExport.BackColor = ColorHelper.Blue;
            btnExport.ForeColor = Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            
            pnlActions.Controls.Add(btnClose);
            pnlActions.Controls.Add(btnExport);

            layout.Controls.Add(pnlActions, 0, 2);
        }

        private void LoadData()
        {
            lblSummary.Text = $"Analyse du {_result.AnalysisDate:g} | {_result.Duplicates.Count} doublons potentiels trouvés";

            foreach (var dup in _result.Duplicates)
            {
                var rowIndex = dgvDuplicates.Rows.Add(
                    dup.SourceEmployee.FullName,
                    dup.SourceEmployee.Code,
                    dup.DestEmployee.Code,
                    dup.SourceEmployee.Num,
                    dup.DestEmployee.Num,
                    dup.Status,
                    dup.Recommendation
                );

                // Styling row based on severity
                if (dup.Type == DuplicateType.CodeDifferent)
                {
                    dgvDuplicates.Rows[rowIndex].Cells["Status"].Style.ForeColor = ColorHelper.Danger;
                }
                else if (dup.Type == DuplicateType.NumeroDifferent)
                {
                    dgvDuplicates.Rows[rowIndex].Cells["Status"].Style.ForeColor = ColorHelper.Warning;
                }
            }
        }
    }
}
