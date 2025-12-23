namespace DoublonManager.Forms
{
    partial class FormDoublonManager
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpConfig = new System.Windows.Forms.GroupBox();
            this.btnDetecterDoublons = new System.Windows.Forms.Button();
            this.grp19M = new System.Windows.Forms.GroupBox();
            this.lblStatus19M = new System.Windows.Forms.Label();
            this.btnTestConnection19M = new System.Windows.Forms.Button();
            this.txtPassword19M = new System.Windows.Forms.TextBox();
            this.lblPassword19M = new System.Windows.Forms.Label();
            this.txtUser19M = new System.Windows.Forms.TextBox();
            this.lblUser19M = new System.Windows.Forms.Label();
            this.rbSqlAuth19M = new System.Windows.Forms.RadioButton();
            this.rbWindowsAuth19M = new System.Windows.Forms.RadioButton();
            this.txtDatabase19M = new System.Windows.Forms.TextBox();
            this.lblDatabase19M = new System.Windows.Forms.Label();
            this.txtServer19M = new System.Windows.Forms.TextBox();
            this.lblServer19M = new System.Windows.Forms.Label();
            this.grp39C = new System.Windows.Forms.GroupBox();
            this.lblStatus39C = new System.Windows.Forms.Label();
            this.btnTestConnection39C = new System.Windows.Forms.Button();
            this.txtPassword39C = new System.Windows.Forms.TextBox();
            this.lblPassword39C = new System.Windows.Forms.Label();
            this.txtUser39C = new System.Windows.Forms.TextBox();
            this.lblUser39C = new System.Windows.Forms.Label();
            this.rbSqlAuth39C = new System.Windows.Forms.RadioButton();
            this.rbWindowsAuth39C = new System.Windows.Forms.RadioButton();
            this.txtDatabase39C = new System.Windows.Forms.TextBox();
            this.lblDatabase39C = new System.Windows.Forms.Label();
            this.txtServer39C = new System.Windows.Forms.TextBox();
            this.lblServer39C = new System.Windows.Forms.Label();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.lblNombreDoublons = new System.Windows.Forms.Label();
            this.dgvDoublons = new System.Windows.Forms.DataGridView();
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.lblDecisionRaison = new System.Windows.Forms.Label();
            this.lblDecisionSupprimer = new System.Windows.Forms.Label();
            this.lblDecisionConserver = new System.Windows.Forms.Label();
            this.lblDecisionTitre = new System.Windows.Forms.Label();
            this.pnlDest = new System.Windows.Forms.Panel();
            this.lblDateDest = new System.Windows.Forms.Label();
            this.lblPhotoDest = new System.Windows.Forms.Label();
            this.lblCompletudeDest = new System.Windows.Forms.Label();
            this.lblBadgeDest = new System.Windows.Forms.Label();
            this.lblTitreDest = new System.Windows.Forms.Label();
            this.pnlSource = new System.Windows.Forms.Panel();
            this.lblDateSource = new System.Windows.Forms.Label();
            this.lblPhotoSource = new System.Windows.Forms.Label();
            this.lblCompletudeSource = new System.Windows.Forms.Label();
            this.lblBadgeSource = new System.Windows.Forms.Label();
            this.lblTitreSource = new System.Windows.Forms.Label();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnRafraichir = new System.Windows.Forms.Button();
            this.btnExporterRapport = new System.Windows.Forms.Button();
            this.btnModeSimulation = new System.Windows.Forms.Button();
            this.btnTraiterAutomatique = new System.Windows.Forms.Button();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.lblStatut = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.grpLogs = new System.Windows.Forms.GroupBox();
            this.txtLogs = new System.Windows.Forms.RichTextBox();
            this.grpConfig.SuspendLayout();
            this.grp19M.SuspendLayout();
            this.grp39C.SuspendLayout();
            this.grpResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoublons)).BeginInit();
            this.grpDetails.SuspendLayout();
            this.pnlDest.SuspendLayout();
            this.pnlSource.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.pnlProgress.SuspendLayout();
            this.grpLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConfig
            // 
            this.grpConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConfig.Controls.Add(this.btnDetecterDoublons);
            this.grpConfig.Controls.Add(this.grp19M);
            this.grpConfig.Controls.Add(this.grp39C);
            this.grpConfig.Location = new System.Drawing.Point(12, 12);
            this.grpConfig.Name = "grpConfig";
            this.grpConfig.Size = new System.Drawing.Size(1160, 270);
            this.grpConfig.TabIndex = 0;
            this.grpConfig.TabStop = false;
            this.grpConfig.Text = "🔧 Configuration des connexions";
            // 
            // btnDetecterDoublons
            // 
            this.btnDetecterDoublons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDetecterDoublons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnDetecterDoublons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetecterDoublons.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnDetecterDoublons.ForeColor = System.Drawing.Color.White;
            this.btnDetecterDoublons.Location = new System.Drawing.Point(950, 215);
            this.btnDetecterDoublons.Name = "btnDetecterDoublons";
            this.btnDetecterDoublons.Size = new System.Drawing.Size(200, 45);
            this.btnDetecterDoublons.TabIndex = 2;
            this.btnDetecterDoublons.Text = "🔍 Détecter doublons";
            this.btnDetecterDoublons.UseVisualStyleBackColor = false;
            this.btnDetecterDoublons.Click += new System.EventHandler(this.btnDetecterDoublons_Click);
            // 
            // btnTestConnection19M
            // 
            // 
            // grp19M
            // 
            this.grp19M.Controls.Add(this.lblStatus19M);
            this.grp19M.Controls.Add(this.btnTestConnection19M);
            this.grp19M.Controls.Add(this.txtPassword19M);
            this.grp19M.Controls.Add(this.lblPassword19M);
            this.grp19M.Controls.Add(this.txtUser19M);
            this.grp19M.Controls.Add(this.lblUser19M);
            this.grp19M.Controls.Add(this.rbSqlAuth19M);
            this.grp19M.Controls.Add(this.rbWindowsAuth19M);
            this.grp19M.Controls.Add(this.txtDatabase19M);
            this.grp19M.Controls.Add(this.lblDatabase19M);
            this.grp19M.Controls.Add(this.txtServer19M);
            this.grp19M.Controls.Add(this.lblServer19M);
            this.grp19M.Location = new System.Drawing.Point(485, 22);
            this.grp19M.Name = "grp19M";
            this.grp19M.Size = new System.Drawing.Size(460, 238);
            this.grp19M.TabIndex = 1;
            this.grp19M.TabStop = false;
            this.grp19M.Text = "📍 Site 19M - Destination";
            // 
            // lblStatus19M
            // 
            this.lblStatus19M.AutoSize = true;
            this.lblStatus19M.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus19M.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus19M.Location = new System.Drawing.Point(210, 201);
            this.lblStatus19M.Name = "lblStatus19M";
            this.lblStatus19M.Size = new System.Drawing.Size(73, 15);
            this.lblStatus19M.TabIndex = 11;
            this.lblStatus19M.Text = "⚪ Non testé";
            // 
            // btnTestConnection19M
            // 
            this.btnTestConnection19M.Location = new System.Drawing.Point(100, 194);
            this.btnTestConnection19M.Name = "btnTestConnection19M";
            this.btnTestConnection19M.Size = new System.Drawing.Size(100, 28);
            this.btnTestConnection19M.TabIndex = 10;
            this.btnTestConnection19M.Text = "Tester";
            this.btnTestConnection19M.UseVisualStyleBackColor = true;
            this.btnTestConnection19M.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // txtPassword19M
            // 
            this.txtPassword19M.Enabled = false;
            this.txtPassword19M.Location = new System.Drawing.Point(100, 158);
            this.txtPassword19M.Name = "txtPassword19M";
            this.txtPassword19M.PasswordChar = '●';
            this.txtPassword19M.Size = new System.Drawing.Size(345, 23);
            this.txtPassword19M.TabIndex = 9;
            // 
            // lblPassword19M
            // 
            this.lblPassword19M.AutoSize = true;
            this.lblPassword19M.Location = new System.Drawing.Point(15, 161);
            this.lblPassword19M.Name = "lblPassword19M";
            this.lblPassword19M.Size = new System.Drawing.Size(80, 15);
            this.lblPassword19M.TabIndex = 8;
            this.lblPassword19M.Text = "Mot de passe:";
            // 
            // txtUser19M
            // 
            this.txtUser19M.Enabled = false;
            this.txtUser19M.Location = new System.Drawing.Point(100, 129);
            this.txtUser19M.Name = "txtUser19M";
            this.txtUser19M.Size = new System.Drawing.Size(345, 23);
            this.txtUser19M.TabIndex = 7;
            // 
            // lblUser19M
            // 
            this.lblUser19M.AutoSize = true;
            this.lblUser19M.Location = new System.Drawing.Point(15, 132);
            this.lblUser19M.Name = "lblUser19M";
            this.lblUser19M.Size = new System.Drawing.Size(63, 15);
            this.lblUser19M.TabIndex = 6;
            this.lblUser19M.Text = "Utilisateur:";
            // 
            // rbSqlAuth19M
            // 
            this.rbSqlAuth19M.AutoSize = true;
            this.rbSqlAuth19M.Location = new System.Drawing.Point(215, 100);
            this.rbSqlAuth19M.Name = "rbSqlAuth19M";
            this.rbSqlAuth19M.Size = new System.Drawing.Size(81, 19);
            this.rbSqlAuth19M.TabIndex = 5;
            this.rbSqlAuth19M.Text = "SQL Server";
            this.rbSqlAuth19M.UseVisualStyleBackColor = true;
            this.rbSqlAuth19M.CheckedChanged += new System.EventHandler(this.ToggleAuthFields);
            // 
            // rbWindowsAuth19M
            // 
            this.rbWindowsAuth19M.AutoSize = true;
            this.rbWindowsAuth19M.Checked = true;
            this.rbWindowsAuth19M.Location = new System.Drawing.Point(100, 100);
            this.rbWindowsAuth19M.Name = "rbWindowsAuth19M";
            this.rbWindowsAuth19M.Size = new System.Drawing.Size(74, 19);
            this.rbWindowsAuth19M.TabIndex = 4;
            this.rbWindowsAuth19M.TabStop = true;
            this.rbWindowsAuth19M.Text = "Windows";
            this.rbWindowsAuth19M.UseVisualStyleBackColor = true;
            this.rbWindowsAuth19M.CheckedChanged += new System.EventHandler(this.ToggleAuthFields);
            // 
            // txtDatabase19M
            // 
            this.txtDatabase19M.Location = new System.Drawing.Point(100, 61);
            this.txtDatabase19M.Name = "txtDatabase19M";
            this.txtDatabase19M.Size = new System.Drawing.Size(345, 23);
            this.txtDatabase19M.TabIndex = 3;
            this.txtDatabase19M.Text = "Amadeus";
            // 
            // lblDatabase19M
            // 
            this.lblDatabase19M.AutoSize = true;
            this.lblDatabase19M.Location = new System.Drawing.Point(15, 64);
            this.lblDatabase19M.Name = "lblDatabase19M";
            this.lblDatabase19M.Size = new System.Drawing.Size(34, 15);
            this.lblDatabase19M.TabIndex = 2;
            this.lblDatabase19M.Text = "Base:";
            // 
            // txtServer19M
            // 
            this.txtServer19M.Location = new System.Drawing.Point(100, 32);
            this.txtServer19M.Name = "txtServer19M";
            this.txtServer19M.Size = new System.Drawing.Size(345, 23);
            this.txtServer19M.TabIndex = 1;
            // 
            // lblServer19M
            // 
            this.lblServer19M.AutoSize = true;
            this.lblServer19M.Location = new System.Drawing.Point(15, 35);
            this.lblServer19M.Name = "lblServer19M";
            this.lblServer19M.Size = new System.Drawing.Size(49, 15);
            this.lblServer19M.TabIndex = 0;
            this.lblServer19M.Text = "Serveur:";
            // 
            // grp39C
            // 
            this.grp39C.Controls.Add(this.lblStatus39C);
            this.grp39C.Controls.Add(this.btnTestConnection39C);
            this.grp39C.Controls.Add(this.txtPassword39C);
            this.grp39C.Controls.Add(this.lblPassword39C);
            this.grp39C.Controls.Add(this.txtUser39C);
            this.grp39C.Controls.Add(this.lblUser39C);
            this.grp39C.Controls.Add(this.rbSqlAuth39C);
            this.grp39C.Controls.Add(this.rbWindowsAuth39C);
            this.grp39C.Controls.Add(this.txtDatabase39C);
            this.grp39C.Controls.Add(this.lblDatabase39C);
            this.grp39C.Controls.Add(this.txtServer39C);
            this.grp39C.Controls.Add(this.lblServer39C);
            this.grp39C.Location = new System.Drawing.Point(15, 22);
            this.grp39C.Name = "grp39C";
            this.grp39C.Size = new System.Drawing.Size(460, 238);
            this.grp39C.TabIndex = 0;
            this.grp39C.TabStop = false;
            this.grp39C.Text = "📍 Site 39C - Source";
            // 
            // lblStatus39C
            // 
            this.lblStatus39C.AutoSize = true;
            this.lblStatus39C.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus39C.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus39C.Location = new System.Drawing.Point(210, 201);
            this.lblStatus39C.Name = "lblStatus39C";
            this.lblStatus39C.Size = new System.Drawing.Size(73, 15);
            this.lblStatus39C.TabIndex = 11;
            this.lblStatus39C.Text = "⚪ Non testé";
            // 
            // btnTestConnection39C
            // 
            this.btnTestConnection39C.Location = new System.Drawing.Point(100, 194);
            this.btnTestConnection39C.Name = "btnTestConnection39C";
            this.btnTestConnection39C.Size = new System.Drawing.Size(100, 28);
            this.btnTestConnection39C.TabIndex = 10;
            this.btnTestConnection39C.Text = "Tester";
            this.btnTestConnection39C.UseVisualStyleBackColor = true;
            this.btnTestConnection39C.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // txtPassword39C
            // 
            this.txtPassword39C.Enabled = false;
            this.txtPassword39C.Location = new System.Drawing.Point(100, 158);
            this.txtPassword39C.Name = "txtPassword39C";
            this.txtPassword39C.PasswordChar = '●';
            this.txtPassword39C.Size = new System.Drawing.Size(345, 23);
            this.txtPassword39C.TabIndex = 9;
            // 
            // lblPassword39C
            // 
            this.lblPassword39C.AutoSize = true;
            this.lblPassword39C.Location = new System.Drawing.Point(15, 161);
            this.lblPassword39C.Name = "lblPassword39C";
            this.lblPassword39C.Size = new System.Drawing.Size(80, 15);
            this.lblPassword39C.TabIndex = 8;
            this.lblPassword39C.Text = "Mot de passe:";
            // 
            // txtUser39C
            // 
            this.txtUser39C.Enabled = false;
            this.txtUser39C.Location = new System.Drawing.Point(100, 129);
            this.txtUser39C.Name = "txtUser39C";
            this.txtUser39C.Size = new System.Drawing.Size(345, 23);
            this.txtUser39C.TabIndex = 7;
            // 
            // lblUser39C
            // 
            this.lblUser39C.AutoSize = true;
            this.lblUser39C.Location = new System.Drawing.Point(15, 132);
            this.lblUser39C.Name = "lblUser39C";
            this.lblUser39C.Size = new System.Drawing.Size(63, 15);
            this.lblUser39C.TabIndex = 6;
            this.lblUser39C.Text = "Utilisateur:";
            // 
            // rbSqlAuth39C
            // 
            this.rbSqlAuth39C.AutoSize = true;
            this.rbSqlAuth39C.Location = new System.Drawing.Point(215, 100);
            this.rbSqlAuth39C.Name = "rbSqlAuth39C";
            this.rbSqlAuth39C.Size = new System.Drawing.Size(81, 19);
            this.rbSqlAuth39C.TabIndex = 5;
            this.rbSqlAuth39C.Text = "SQL Server";
            this.rbSqlAuth39C.UseVisualStyleBackColor = true;
            this.rbSqlAuth39C.CheckedChanged += new System.EventHandler(this.ToggleAuthFields);
            // 
            // rbWindowsAuth39C
            // 
            this.rbWindowsAuth39C.AutoSize = true;
            this.rbWindowsAuth39C.Checked = true;
            this.rbWindowsAuth39C.Location = new System.Drawing.Point(100, 100);
            this.rbWindowsAuth39C.Name = "rbWindowsAuth39C";
            this.rbWindowsAuth39C.Size = new System.Drawing.Size(74, 19);
            this.rbWindowsAuth39C.TabIndex = 4;
            this.rbWindowsAuth39C.TabStop = true;
            this.rbWindowsAuth39C.Text = "Windows";
            this.rbWindowsAuth39C.UseVisualStyleBackColor = true;
            this.rbWindowsAuth39C.CheckedChanged += new System.EventHandler(this.ToggleAuthFields);
            // 
            // txtDatabase39C
            // 
            this.txtDatabase39C.Location = new System.Drawing.Point(100, 61);
            this.txtDatabase39C.Name = "txtDatabase39C";
            this.txtDatabase39C.Size = new System.Drawing.Size(345, 23);
            this.txtDatabase39C.TabIndex = 3;
            this.txtDatabase39C.Text = "Amadeus";
            // 
            // lblDatabase39C
            // 
            this.lblDatabase39C.AutoSize = true;
            this.lblDatabase39C.Location = new System.Drawing.Point(15, 64);
            this.lblDatabase39C.Name = "lblDatabase39C";
            this.lblDatabase39C.Size = new System.Drawing.Size(34, 15);
            this.lblDatabase39C.TabIndex = 2;
            this.lblDatabase39C.Text = "Base:";
            // 
            // txtServer39C
            // 
            this.txtServer39C.Location = new System.Drawing.Point(100, 32);
            this.txtServer39C.Name = "txtServer39C";
            this.txtServer39C.Size = new System.Drawing.Size(345, 23);
            this.txtServer39C.TabIndex = 1;
            // 
            // lblServer39C
            // 
            this.lblServer39C.AutoSize = true;
            this.lblServer39C.Location = new System.Drawing.Point(15, 35);
            this.lblServer39C.Name = "lblServer39C";
            this.lblServer39C.Size = new System.Drawing.Size(49, 15);
            this.lblServer39C.TabIndex = 0;
            this.lblServer39C.Text = "Serveur:";
            // 
            // grpResults
            // 
            this.grpResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResults.Controls.Add(this.lblNombreDoublons);
            this.grpResults.Controls.Add(this.dgvDoublons);
            this.grpResults.Location = new System.Drawing.Point(12, 118);
            this.grpResults.Name = "grpResults";
            this.grpResults.Size = new System.Drawing.Size(960, 250);
            this.grpResults.TabIndex = 1;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "📊 Doublons détectés (0)";
            // 
            // lblNombreDoublons
            // 
            this.lblNombreDoublons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNombreDoublons.AutoSize = true;
            this.lblNombreDoublons.Location = new System.Drawing.Point(825, 0);
            this.lblNombreDoublons.Name = "lblNombreDoublons";
            this.lblNombreDoublons.Size = new System.Drawing.Size(126, 15);
            this.lblNombreDoublons.TabIndex = 1;
            this.lblNombreDoublons.Text = "Doublons détectés (0)";
            // 
            // dgvDoublons
            // 
            this.dgvDoublons.AllowUserToAddRows = false;
            this.dgvDoublons.AllowUserToDeleteRows = false;
            this.dgvDoublons.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDoublons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDoublons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDoublons.Location = new System.Drawing.Point(3, 19);
            this.dgvDoublons.MultiSelect = false;
            this.dgvDoublons.Name = "dgvDoublons";
            this.dgvDoublons.ReadOnly = true;
            this.dgvDoublons.RowHeadersVisible = false;
            this.dgvDoublons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDoublons.Size = new System.Drawing.Size(954, 228);
            this.dgvDoublons.TabIndex = 0;
            this.dgvDoublons.SelectionChanged += new System.EventHandler(this.DgvDoublons_SelectionChanged);
            // 
            // grpDetails
            // 
            this.grpDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetails.Controls.Add(this.lblDecisionRaison);
            this.grpDetails.Controls.Add(this.lblDecisionSupprimer);
            this.grpDetails.Controls.Add(this.lblDecisionConserver);
            this.grpDetails.Controls.Add(this.lblDecisionTitre);
            this.grpDetails.Controls.Add(this.pnlDest);
            this.grpDetails.Controls.Add(this.pnlSource);
            this.grpDetails.Location = new System.Drawing.Point(12, 374);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(960, 160);
            this.grpDetails.TabIndex = 2;
            this.grpDetails.TabStop = false;
            this.grpDetails.Text = "📋 Détails du doublon sélectionné";
            // 
            // lblDecisionRaison
            // 
            this.lblDecisionRaison.AutoSize = true;
            this.lblDecisionRaison.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblDecisionRaison.Location = new System.Drawing.Point(520, 125);
            this.lblDecisionRaison.Name = "lblDecisionRaison";
            this.lblDecisionRaison.Size = new System.Drawing.Size(56, 15);
            this.lblDecisionRaison.TabIndex = 5;
            this.lblDecisionRaison.Text = "📝 Raison";
            // 
            // lblDecisionSupprimer
            // 
            this.lblDecisionSupprimer.AutoSize = true;
            this.lblDecisionSupprimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblDecisionSupprimer.Location = new System.Drawing.Point(520, 100);
            this.lblDecisionSupprimer.Name = "lblDecisionSupprimer";
            this.lblDecisionSupprimer.Size = new System.Drawing.Size(100, 15);
            this.lblDecisionSupprimer.TabIndex = 4;
            this.lblDecisionSupprimer.Text = "🗑️ Supprimer: ...";
            // 
            // lblDecisionConserver
            // 
            this.lblDecisionConserver.AutoSize = true;
            this.lblDecisionConserver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.lblDecisionConserver.Location = new System.Drawing.Point(520, 75);
            this.lblDecisionConserver.Name = "lblDecisionConserver";
            this.lblDecisionConserver.Size = new System.Drawing.Size(98, 15);
            this.lblDecisionConserver.TabIndex = 3;
            this.lblDecisionConserver.Text = "✅ Conserver: ...";
            // 
            // lblDecisionTitre
            // 
            this.lblDecisionTitre.AutoSize = true;
            this.lblDecisionTitre.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDecisionTitre.Location = new System.Drawing.Point(520, 50);
            this.lblDecisionTitre.Name = "lblDecisionTitre";
            this.lblDecisionTitre.Size = new System.Drawing.Size(128, 15);
            this.lblDecisionTitre.TabIndex = 2;
            this.lblDecisionTitre.Text = "Décision automatique:";
            // 
            // pnlDest
            // 
            this.pnlDest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDest.Controls.Add(this.lblDateDest);
            this.pnlDest.Controls.Add(this.lblPhotoDest);
            this.pnlDest.Controls.Add(this.lblCompletudeDest);
            this.pnlDest.Controls.Add(this.lblBadgeDest);
            this.pnlDest.Controls.Add(this.lblTitreDest);
            this.pnlDest.Location = new System.Drawing.Point(265, 25);
            this.pnlDest.Name = "pnlDest";
            this.pnlDest.Size = new System.Drawing.Size(235, 120);
            this.pnlDest.TabIndex = 1;
            // 
            // lblDateDest
            // 
            this.lblDateDest.AutoSize = true;
            this.lblDateDest.Location = new System.Drawing.Point(10, 95);
            this.lblDateDest.Name = "lblDateDest";
            this.lblDateDest.Size = new System.Drawing.Size(59, 15);
            this.lblDateDest.TabIndex = 4;
            this.lblDateDest.Text = "Modif: ...";
            // 
            // lblPhotoDest
            // 
            this.lblPhotoDest.AutoSize = true;
            this.lblPhotoDest.Location = new System.Drawing.Point(10, 73);
            this.lblPhotoDest.Name = "lblPhotoDest";
            this.lblPhotoDest.Size = new System.Drawing.Size(59, 15);
            this.lblPhotoDest.TabIndex = 3;
            this.lblPhotoDest.Text = "Photo: ...";
            // 
            // lblCompletudeDest
            // 
            this.lblCompletudeDest.AutoSize = true;
            this.lblCompletudeDest.Location = new System.Drawing.Point(10, 51);
            this.lblCompletudeDest.Name = "lblCompletudeDest";
            this.lblCompletudeDest.Size = new System.Drawing.Size(95, 15);
            this.lblCompletudeDest.TabIndex = 2;
            this.lblCompletudeDest.Text = "Complétude: ...";
            // 
            // lblBadgeDest
            // 
            this.lblBadgeDest.AutoSize = true;
            this.lblBadgeDest.Location = new System.Drawing.Point(10, 29);
            this.lblBadgeDest.Name = "lblBadgeDest";
            this.lblBadgeDest.Size = new System.Drawing.Size(61, 15);
            this.lblBadgeDest.TabIndex = 1;
            this.lblBadgeDest.Text = "Badge: ...";
            // 
            // lblTitreDest
            // 
            this.lblTitreDest.AutoSize = true;
            this.lblTitreDest.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTitreDest.Location = new System.Drawing.Point(10, 5);
            this.lblTitreDest.Name = "lblTitreDest";
            this.lblTitreDest.Size = new System.Drawing.Size(59, 15);
            this.lblTitreDest.TabIndex = 0;
            this.lblTitreDest.Text = "Site 19M";
            // 
            // pnlSource
            // 
            this.pnlSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSource.Controls.Add(this.lblDateSource);
            this.pnlSource.Controls.Add(this.lblPhotoSource);
            this.pnlSource.Controls.Add(this.lblCompletudeSource);
            this.pnlSource.Controls.Add(this.lblBadgeSource);
            this.pnlSource.Controls.Add(this.lblTitreSource);
            this.pnlSource.Location = new System.Drawing.Point(15, 25);
            this.pnlSource.Name = "pnlSource";
            this.pnlSource.Size = new System.Drawing.Size(235, 120);
            this.pnlSource.TabIndex = 0;
            // 
            // lblDateSource
            // 
            this.lblDateSource.AutoSize = true;
            this.lblDateSource.Location = new System.Drawing.Point(10, 95);
            this.lblDateSource.Name = "lblDateSource";
            this.lblDateSource.Size = new System.Drawing.Size(59, 15);
            this.lblDateSource.TabIndex = 4;
            this.lblDateSource.Text = "Modif: ...";
            // 
            // lblPhotoSource
            // 
            this.lblPhotoSource.AutoSize = true;
            this.lblPhotoSource.Location = new System.Drawing.Point(10, 73);
            this.lblPhotoSource.Name = "lblPhotoSource";
            this.lblPhotoSource.Size = new System.Drawing.Size(59, 15);
            this.lblPhotoSource.TabIndex = 3;
            this.lblPhotoSource.Text = "Photo: ...";
            // 
            // lblCompletudeSource
            // 
            this.lblCompletudeSource.AutoSize = true;
            this.lblCompletudeSource.Location = new System.Drawing.Point(10, 51);
            this.lblCompletudeSource.Name = "lblCompletudeSource";
            this.lblCompletudeSource.Size = new System.Drawing.Size(95, 15);
            this.lblCompletudeSource.TabIndex = 2;
            this.lblCompletudeSource.Text = "Complétude: ...";
            // 
            // lblBadgeSource
            // 
            this.lblBadgeSource.AutoSize = true;
            this.lblBadgeSource.Location = new System.Drawing.Point(10, 29);
            this.lblBadgeSource.Name = "lblBadgeSource";
            this.lblBadgeSource.Size = new System.Drawing.Size(61, 15);
            this.lblBadgeSource.TabIndex = 1;
            this.lblBadgeSource.Text = "Badge: ...";
            // 
            // lblTitreSource
            // 
            this.lblTitreSource.AutoSize = true;
            this.lblTitreSource.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTitreSource.Location = new System.Drawing.Point(10, 5);
            this.lblTitreSource.Name = "lblTitreSource";
            this.lblTitreSource.Size = new System.Drawing.Size(57, 15);
            this.lblTitreSource.TabIndex = 0;
            this.lblTitreSource.Text = "Site 39C";
            // 
            // pnlActions
            // 
            this.pnlActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlActions.Controls.Add(this.btnRafraichir);
            this.pnlActions.Controls.Add(this.btnExporterRapport);
            this.pnlActions.Controls.Add(this.btnModeSimulation);
            this.pnlActions.Controls.Add(this.btnTraiterAutomatique);
            this.pnlActions.Location = new System.Drawing.Point(12, 540);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(960, 50);
            this.pnlActions.TabIndex = 3;
            // 
            // btnRafraichir
            // 
            this.btnRafraichir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRafraichir.Location = new System.Drawing.Point(825, 10);
            this.btnRafraichir.Name = "btnRafraichir";
            this.btnRafraichir.Size = new System.Drawing.Size(120, 30);
            this.btnRafraichir.TabIndex = 3;
            this.btnRafraichir.Text = "🔄 Rafraîchir";
            this.btnRafraichir.UseVisualStyleBackColor = true;
            this.btnRafraichir.Click += new System.EventHandler(this.btnRafraichir_Click);
            // 
            // btnExporterRapport
            // 
            this.btnExporterRapport.Location = new System.Drawing.Point(400, 10);
            this.btnExporterRapport.Name = "btnExporterRapport";
            this.btnExporterRapport.Size = new System.Drawing.Size(150, 30);
            this.btnExporterRapport.TabIndex = 2;
            this.btnExporterRapport.Text = "📄 Exporter rapport";
            this.btnExporterRapport.UseVisualStyleBackColor = true;
            // 
            // btnModeSimulation
            // 
            this.btnModeSimulation.Location = new System.Drawing.Point(220, 10);
            this.btnModeSimulation.Name = "btnModeSimulation";
            this.btnModeSimulation.Size = new System.Drawing.Size(170, 30);
            this.btnModeSimulation.TabIndex = 1;
            this.btnModeSimulation.Text = "🧪 Mode simulation";
            this.btnModeSimulation.UseVisualStyleBackColor = true;
            this.btnModeSimulation.Click += new System.EventHandler(this.btnModeSimulation_Click);
            // 
            // btnTraiterAutomatique
            // 
            this.btnTraiterAutomatique.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.btnTraiterAutomatique.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTraiterAutomatique.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTraiterAutomatique.ForeColor = System.Drawing.Color.White;
            this.btnTraiterAutomatique.Location = new System.Drawing.Point(5, 10);
            this.btnTraiterAutomatique.Name = "btnTraiterAutomatique";
            this.btnTraiterAutomatique.Size = new System.Drawing.Size(200, 30);
            this.btnTraiterAutomatique.TabIndex = 0;
            this.btnTraiterAutomatique.Text = "🎯 Traiter automatiquement";
            this.btnTraiterAutomatique.UseVisualStyleBackColor = false;
            this.btnTraiterAutomatique.Click += new System.EventHandler(this.btnTraiterAutomatique_Click);
            // 
            // pnlProgress
            // 
            this.pnlProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlProgress.Controls.Add(this.lblStatut);
            this.pnlProgress.Controls.Add(this.progressBar);
            this.pnlProgress.Location = new System.Drawing.Point(12, 595);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Size = new System.Drawing.Size(960, 45);
            this.pnlProgress.TabIndex = 4;
            // 
            // lblStatut
            // 
            this.lblStatut.AutoSize = true;
            this.lblStatut.Location = new System.Drawing.Point(5, 25);
            this.lblStatut.Name = "lblStatut";
            this.lblStatut.Size = new System.Drawing.Size(53, 15);
            this.lblStatut.TabIndex = 1;
            this.lblStatut.Text = "Statut: ...";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(5, 5);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(950, 15);
            this.progressBar.TabIndex = 0;
            // 
            // grpLogs
            // 
            this.grpLogs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLogs.Controls.Add(this.txtLogs);
            this.grpLogs.Location = new System.Drawing.Point(12, 645);
            this.grpLogs.Name = "grpLogs";
            this.grpLogs.Size = new System.Drawing.Size(960, 120);
            this.grpLogs.TabIndex = 5;
            this.grpLogs.TabStop = false;
            this.grpLogs.Text = "📜 Logs";
            // 
            // txtLogs
            // 
            this.txtLogs.BackColor = System.Drawing.Color.Black;
            this.txtLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogs.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLogs.ForeColor = System.Drawing.Color.LightGreen;
            this.txtLogs.Location = new System.Drawing.Point(3, 19);
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.Size = new System.Drawing.Size(954, 98);
            this.txtLogs.TabIndex = 0;
            this.txtLogs.Text = "";
            // 
            // FormDoublonManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 781);
            this.Controls.Add(this.grpLogs);
            this.Controls.Add(this.pnlProgress);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.grpDetails);
            this.Controls.Add(this.grpResults);
            this.Controls.Add(this.grpConfig);
            this.MinimumSize = new System.Drawing.Size(1000, 800);
            this.Name = "FormDoublonManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DoublonManager - Gestion des doublons Amadeus";
            this.grpConfig.ResumeLayout(false);
            this.grpConfig.PerformLayout();
            this.grpResults.ResumeLayout(false);
            this.grpResults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoublons)).EndInit();
            this.grpDetails.ResumeLayout(false);
            this.grpDetails.PerformLayout();
            this.pnlDest.ResumeLayout(false);
            this.pnlDest.PerformLayout();
            this.pnlSource.ResumeLayout(false);
            this.pnlSource.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.pnlProgress.ResumeLayout(false);
            this.pnlProgress.PerformLayout();
            this.grpLogs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox grpConfig;
        private System.Windows.Forms.Button btnDetecterDoublons;
        private System.Windows.Forms.GroupBox grp39C;
        private System.Windows.Forms.Label lblServer39C;
        private System.Windows.Forms.TextBox txtServer39C;
        private System.Windows.Forms.Label lblDatabase39C;
        private System.Windows.Forms.TextBox txtDatabase39C;
        private System.Windows.Forms.RadioButton rbWindowsAuth39C;
        private System.Windows.Forms.RadioButton rbSqlAuth39C;
        private System.Windows.Forms.Label lblUser39C;
        private System.Windows.Forms.TextBox txtUser39C;
        private System.Windows.Forms.Label lblPassword39C;
        private System.Windows.Forms.TextBox txtPassword39C;
        private System.Windows.Forms.Button btnTestConnection39C;
        private System.Windows.Forms.Label lblStatus39C;
        private System.Windows.Forms.GroupBox grp19M;
        private System.Windows.Forms.Label lblServer19M;
        private System.Windows.Forms.TextBox txtServer19M;
        private System.Windows.Forms.Label lblDatabase19M;
        private System.Windows.Forms.TextBox txtDatabase19M;
        private System.Windows.Forms.RadioButton rbWindowsAuth19M;
        private System.Windows.Forms.RadioButton rbSqlAuth19M;
        private System.Windows.Forms.Label lblUser19M;
        private System.Windows.Forms.TextBox txtUser19M;
        private System.Windows.Forms.Label lblPassword19M;
        private System.Windows.Forms.TextBox txtPassword19M;
        private System.Windows.Forms.Button btnTestConnection19M;
        private System.Windows.Forms.Label lblStatus19M;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.DataGridView dgvDoublons;
        private System.Windows.Forms.Label lblNombreDoublons;
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.Panel pnlSource;
        private System.Windows.Forms.Label lblTitreSource;
        private System.Windows.Forms.Label lblBadgeSource;
        private System.Windows.Forms.Label lblCompletudeSource;
        private System.Windows.Forms.Label lblPhotoSource;
        private System.Windows.Forms.Label lblDateSource;
        private System.Windows.Forms.Panel pnlDest;
        private System.Windows.Forms.Label lblTitreDest;
        private System.Windows.Forms.Label lblBadgeDest;
        private System.Windows.Forms.Label lblCompletudeDest;
        private System.Windows.Forms.Label lblPhotoDest;
        private System.Windows.Forms.Label lblDateDest;
        private System.Windows.Forms.Label lblDecisionTitre;
        private System.Windows.Forms.Label lblDecisionConserver;
        private System.Windows.Forms.Label lblDecisionSupprimer;
        private System.Windows.Forms.Label lblDecisionRaison;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnTraiterAutomatique;
        private System.Windows.Forms.Button btnModeSimulation;
        private System.Windows.Forms.Button btnExporterRapport;
        private System.Windows.Forms.Button btnRafraichir;
        private System.Windows.Forms.Panel pnlProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatut;
        private System.Windows.Forms.GroupBox grpLogs;
        private System.Windows.Forms.RichTextBox txtLogs;
    }
}
