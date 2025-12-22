using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DoublonManager.Services;

namespace DoublonManager.Forms
{
    public partial class DeleteConfirmationForm : Form
    {
        private List<DuplicatePair> _selectedDuplicates;
        private DatabaseService _dbService;
        private DataGridView dgvConfirmation;
        private Button btnConfirm, btnCancel;

        public DeleteConfirmationForm(List<DuplicatePair> duplicates, DatabaseService dbService)
        {
            _selectedDuplicates = duplicates;
            _dbService = dbService;
            InitializeComponent();
            PopulateGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "⚠️ Confirmation de suppression";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.FromArgb(231, 76, 60), Padding = new Padding(20) };
            Label lblTitle = new Label { 
                Text = $"Vous allez traiter {_selectedDuplicates.Count} doublon(s). Choisissez le site à CONSERVER :", 
                ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true 
            };
            pnlHeader.Controls.Add(lblTitle);

            dgvConfirmation = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowTemplate = { Height = 40 }
            };

            // Colonnes
            dgvConfirmation.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Employé", FillWeight = 30, ReadOnly = true },
                new DataGridViewTextBoxColumn { Name = "Details39C", HeaderText = "Détails Site 39C", FillWeight = 30, ReadOnly = true },
                new DataGridViewTextBoxColumn { Name = "Details19M", HeaderText = "Détails Site 19M", FillWeight = 30, ReadOnly = true },
                new DataGridViewComboBoxColumn { 
                    Name = "Action", 
                    HeaderText = "Action (Site à GARDER)", 
                    FillWeight = 30 
                }
            });

            Panel pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.WhiteSmoke };
            btnConfirm = new Button { 
                Text = "✅ Valider les suppressions", 
                Size = new Size(200, 40), Location = new Point(770, 10), 
                BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat 
            };
            btnCancel = new Button { 
                Text = "Annuler", Size = new Size(100, 40), Location = new Point(660, 10), 
                BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat 
            };

            btnConfirm.Click += BtnConfirm_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            pnlActions.Controls.AddRange(new Control[] { btnConfirm, btnCancel });

            this.Controls.Add(dgvConfirmation);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlActions);
        }

        private void PopulateGrid()
        {
            foreach (var dup in _selectedDuplicates)
            {
                int rowIndex = dgvConfirmation.Rows.Add(
                    dup.Employee39C.FullName,
                    $"ID: {dup.Employee39C.ID} | N°: {dup.Employee39C.CardholderIdNumber}",
                    $"ID: {dup.Employee19M.ID} | N°: {dup.Employee19M.CardholderIdNumber}"
                );

                var comboCell = (DataGridViewComboBoxCell)dgvConfirmation.Rows[rowIndex].Cells["Action"];
                comboCell.Items.Add("GARDER Site 39C (Suppr 19M)");
                comboCell.Items.Add("GARDER Site 19M (Suppr 39C)");
                comboCell.Value = comboCell.Items[0]; // Par défaut garder 39C
                
                dgvConfirmation.Rows[rowIndex].Tag = dup;
            }
        }

        private async void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cette action est irréversible (suppression logique). Continuer ?", "Confirmation", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

            this.Enabled = false;
            btnConfirm.Text = "Traitement en cours...";

            int success = 0;
            string currentUser = Environment.UserName;

            foreach (DataGridViewRow row in dgvConfirmation.Rows)
            {
                var dup = (DuplicatePair)row.Tag;
                string action = row.Cells["Action"].Value.ToString();
                
                try {
                    if (action.Contains("GARDER Site 39C")) {
                        // On garde 39C, donc on supprime sur 19M
                        await _dbService.DeleteEmployee("19M", dup.Employee19M.ID, currentUser);
                    } else {
                        // On garde 19M, donc on supprime sur 39C
                        await _dbService.DeleteEmployee("39C", dup.Employee39C.ID, currentUser);
                    }
                    success++;
                } catch (Exception ex) {
                    MessageBox.Show($"Erreur pour {dup.Employee39C.FullName} : {ex.Message}");
                }
            }

            MessageBox.Show($"{success} doublon(s) traité(s) avec succès.", "Terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
