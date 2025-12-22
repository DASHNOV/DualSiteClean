using System;
using System.Windows.Forms;
using System.Drawing;
using DoublonManager.Helpers;

namespace DoublonManager.Forms
{
    public partial class SettingsForm : Form
    {
        private bool _connection39CTested = false;
        private bool _connection19MTested = false;

        // Déclaration des contrôles
        private TextBox txtServer39C, txtDatabase39C, txtUser39C, txtPassword39C;
        private TextBox txtServer19M, txtDatabase19M, txtUser19M, txtPassword19M;
        private RadioButton rbWindowsAuth39C, rbSqlAuth39C;
        private RadioButton rbWindowsAuth19M, rbSqlAuth19M;
        private Button btnTest39C, btnTest19M, btnSave, btnCancel;
        private Label lblStatus39C, lblStatus19M;

        public SettingsForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadSavedSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "⚙️ Configuration des connexions";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = ColorHelper.Neutral;
        }

        private void InitializeCustomComponents()
        {
            // En-tête
            Label lblTitle = new Label
            {
                Text = "⚙️ Configuration des connexions aux bases de données",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = ColorHelper.Primary,
                Location = new Point(30, 20),
                AutoSize = true
            };

            Label lblSubtitle = new Label
            {
                Text = "Configurez les paramètres de connexion pour les deux sites Amadeus8",
                Font = new Font("Segoe UI", 10f),
                ForeColor = ColorHelper.Gray,
                Location = new Point(30, 55),
                AutoSize = true
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);

            // GroupBox Site 39C
            CreateSite39CGroupBox();

            // GroupBox Site 19M
            CreateSite19MGroupBox();

            // Boutons d'action
            CreateActionButtons();
        }

        private void CreateSite39CGroupBox()
        {
            GroupBox grp39C = new GroupBox
            {
                Text = "📍 Site 39C - Base Amadeus5_39C",
                Location = new Point(30, 110),
                Size = new Size(920, 240),
                BackColor = Color.White,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold)
            };

            // Serveur
            grp39C.Controls.Add(new Label 
            { 
                Text = "Serveur :", 
                Location = new Point(20, 40),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            txtServer39C = new TextBox
            {
                Location = new Point(200, 37),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10f)
            };
            grp39C.Controls.Add(txtServer39C);

            // Base de données
            grp39C.Controls.Add(new Label 
            { 
                Text = "Base de données :", 
                Location = new Point(20, 75),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            txtDatabase39C = new TextBox
            {
                Location = new Point(200, 72),
                Size = new Size(400, 25),
                Text = "Amadeus5_39C",
                Font = new Font("Segoe UI", 10f)
            };
            grp39C.Controls.Add(txtDatabase39C);

            // Authentification
            grp39C.Controls.Add(new Label 
            { 
                Text = "Authentification :", 
                Location = new Point(20, 110),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            rbWindowsAuth39C = new RadioButton
            {
                Text = "Authentification Windows",
                Location = new Point(200, 108),
                Checked = true,
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            };
            rbWindowsAuth39C.CheckedChanged += (s, e) => ToggleAuthFields39C();
            grp39C.Controls.Add(rbWindowsAuth39C);

            rbSqlAuth39C = new RadioButton
            {
                Text = "Authentification SQL Server",
                Location = new Point(450, 108),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            };
            rbSqlAuth39C.CheckedChanged += (s, e) => ToggleAuthFields39C();
            grp39C.Controls.Add(rbSqlAuth39C);

            // Utilisateur
            grp39C.Controls.Add(new Label 
            { 
                Text = "Utilisateur :", 
                Location = new Point(20, 145),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            txtUser39C = new TextBox
            {
                Location = new Point(200, 142),
                Size = new Size(200, 25),
                Enabled = false,
                Font = new Font("Segoe UI", 10f)
            };
            grp39C.Controls.Add(txtUser39C);

            // Mot de passe
            grp39C.Controls.Add(new Label 
            { 
                Text = "Mot de passe :", 
                Location = new Point(430, 145),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            txtPassword39C = new TextBox
            {
                Location = new Point(560, 142),
                Size = new Size(200, 25),
                PasswordChar = '●',
                Enabled = false,
                Font = new Font("Segoe UI", 10f)
            };
            grp39C.Controls.Add(txtPassword39C);

            // Bouton test
            btnTest39C = new Button
            {
                Text = "🔍 Tester la connexion",
                Location = new Point(200, 180),
                Size = new Size(180, 35),
                BackColor = ColorHelper.Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTest39C.FlatAppearance.BorderSize = 0;
            btnTest39C.Click += BtnTest39C_Click;
            grp39C.Controls.Add(btnTest39C);

            // Statut
            lblStatus39C = new Label
            {
                Text = "⚪ Non testé",
                Location = new Point(400, 187),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = ColorHelper.Gray
            };
            grp39C.Controls.Add(lblStatus39C);

            this.Controls.Add(grp39C);
        }

        private void CreateSite19MGroupBox()
        {
            GroupBox grp19M = new GroupBox
            {
                Text = "📍 Site 19M - Base Amadeus5_19M",
                Location = new Point(30, 380),
                Size = new Size(920, 240),
                BackColor = Color.White,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold)
            };

            // Serveur
            grp19M.Controls.Add(new Label 
            { 
                Text = "Serveur :", 
                Location = new Point(20, 40),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            txtServer19M = new TextBox
            {
                Location = new Point(200, 37),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10f)
            };
            grp19M.Controls.Add(txtServer19M);

            // Base de données
            grp19M.Controls.Add(new Label 
            { 
                Text = "Base de données :", 
                Location = new Point(20, 75),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            txtDatabase19M = new TextBox
            {
                Location = new Point(200, 72),
                Size = new Size(400, 25),
                Text = "Amadeus5_19M",
                Font = new Font("Segoe UI", 10f)
            };
            grp19M.Controls.Add(txtDatabase19M);

            // Authentification
            grp19M.Controls.Add(new Label 
            { 
                Text = "Authentification :", 
                Location = new Point(20, 110),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            rbWindowsAuth19M = new RadioButton
            {
                Text = "Authentification Windows",
                Location = new Point(200, 108),
                Checked = true,
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            };
            rbWindowsAuth19M.CheckedChanged += (s, e) => ToggleAuthFields19M();
            grp19M.Controls.Add(rbWindowsAuth19M);

            rbSqlAuth19M = new RadioButton
            {
                Text = "Authentification SQL Server",
                Location = new Point(450, 108),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            };
            rbSqlAuth19M.CheckedChanged += (s, e) => ToggleAuthFields19M();
            grp19M.Controls.Add(rbSqlAuth19M);

            // Utilisateur
            grp19M.Controls.Add(new Label 
            { 
                Text = "Utilisateur :", 
                Location = new Point(20, 145),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            txtUser19M = new TextBox
            {
                Location = new Point(200, 142),
                Size = new Size(200, 25),
                Enabled = false,
                Font = new Font("Segoe UI", 10f)
            };
            grp19M.Controls.Add(txtUser19M);

            // Mot de passe
            grp19M.Controls.Add(new Label 
            { 
                Text = "Mot de passe :", 
                Location = new Point(430, 145),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });

            txtPassword19M = new TextBox
            {
                Location = new Point(560, 142),
                Size = new Size(200, 25),
                PasswordChar = '●',
                Enabled = false,
                Font = new Font("Segoe UI", 10f)
            };
            grp19M.Controls.Add(txtPassword19M);

            // Bouton test
            btnTest19M = new Button
            {
                Text = "🔍 Tester la connexion",
                Location = new Point(200, 180),
                Size = new Size(180, 35),
                BackColor = ColorHelper.Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTest19M.FlatAppearance.BorderSize = 0;
            btnTest19M.Click += BtnTest19M_Click;
            grp19M.Controls.Add(btnTest19M);

            // Statut
            lblStatus19M = new Label
            {
                Text = "⚪ Non testé",
                Location = new Point(400, 187),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = ColorHelper.Gray
            };
            grp19M.Controls.Add(lblStatus19M);

            this.Controls.Add(grp19M);
        }

        private void CreateActionButtons()
        {
            btnSave = new Button
            {
                Text = "💾 Enregistrer",
                Location = new Point(30, 650),
                Size = new Size(200, 45),
                BackColor = ColorHelper.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "❌ Annuler",
                Location = new Point(250, 650),
                Size = new Size(200, 45),
                BackColor = ColorHelper.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            Button btnViewLogs = new Button
            {
                Text = "📋 Voir les logs",
                Location = new Point(750, 650),
                Size = new Size(200, 45),
                BackColor = Color.White,
                ForeColor = ColorHelper.Primary,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f),
                Cursor = Cursors.Hand
            };
            btnViewLogs.FlatAppearance.BorderColor = ColorHelper.Primary;
            btnViewLogs.Click += (s, e) => LogHelper.OpenLogDirectory();

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnViewLogs);
        }

        // Événements
        private void ToggleAuthFields39C()
        {
            bool useSql = rbSqlAuth39C.Checked;
            txtUser39C.Enabled = useSql;
            txtPassword39C.Enabled = useSql;
            _connection39CTested = false;
            lblStatus39C.Text = "⚪ Non testé";
            lblStatus39C.ForeColor = ColorHelper.Gray;
            UpdateSaveButton();
        }

        private void ToggleAuthFields19M()
        {
            bool useSql = rbSqlAuth19M.Checked;
            txtUser19M.Enabled = useSql;
            txtPassword19M.Enabled = useSql;
            _connection19MTested = false;
            lblStatus19M.Text = "⚪ Non testé";
            lblStatus19M.ForeColor = ColorHelper.Gray;
            UpdateSaveButton();
        }

        private async void BtnTest39C_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServer39C.Text))
            {
                MessageBox.Show("Veuillez saisir le nom du serveur.", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTest39C.Enabled = false;
            lblStatus39C.Text = "⏳ Test en cours...";
            lblStatus39C.ForeColor = ColorHelper.Blue;

            string connStr = ConnectionHelper.BuildConnectionString(
                txtServer39C.Text,
                txtDatabase39C.Text,
                rbWindowsAuth39C.Checked,
                txtUser39C.Text,
                txtPassword39C.Text
            );

            bool success = await ConnectionHelper.TestConnection(connStr);

            if (success)
            {
                lblStatus39C.Text = "✅ Connexion réussie";
                lblStatus39C.ForeColor = ColorHelper.Success;
                _connection39CTested = true;
            }
            else
            {
                lblStatus39C.Text = "❌ Échec de connexion";
                lblStatus39C.ForeColor = ColorHelper.Danger;
                _connection39CTested = false;
            }

            btnTest39C.Enabled = true;
            UpdateSaveButton();
        }

        private async void BtnTest19M_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServer19M.Text))
            {
                MessageBox.Show("Veuillez saisir le nom du serveur.", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTest19M.Enabled = false;
            lblStatus19M.Text = "⏳ Test en cours...";
            lblStatus19M.ForeColor = ColorHelper.Blue;

            string connStr = ConnectionHelper.BuildConnectionString(
                txtServer19M.Text,
                txtDatabase19M.Text,
                rbWindowsAuth19M.Checked,
                txtUser19M.Text,
                txtPassword19M.Text
            );

            bool success = await ConnectionHelper.TestConnection(connStr);

            if (success)
            {
                lblStatus19M.Text = "✅ Connexion réussie";
                lblStatus19M.ForeColor = ColorHelper.Success;
                _connection19MTested = true;
            }
            else
            {
                lblStatus19M.Text = "❌ Échec de connexion";
                lblStatus19M.ForeColor = ColorHelper.Danger;
                _connection19MTested = false;
            }

            btnTest19M.Enabled = true;
            UpdateSaveButton();
        }

        private void BtnSave_Click(object sender, EventArgs e)
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

            try
            {
                ConnectionHelper.SaveConnectionSettings(settings39C, settings19M);
                
                MessageBox.Show("✅ Paramètres enregistrés avec succès !", 
                    "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement :\n{ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSaveButton()
        {
            btnSave.Enabled = _connection39CTested && _connection19MTested;
        }

        private void LoadSavedSettings()
        {
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
        }
    }
}
