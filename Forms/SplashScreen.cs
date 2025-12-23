using System;
using System.Drawing;
using System.Windows.Forms;
using DoublonManager.Helpers;

namespace DoublonManager.Forms
{
    public partial class SplashScreen : Form
    {
        private System.Windows.Forms.Timer _timer;
        
        public SplashScreen()
        {
            InitializeComponent();
            
            // Configuration du formulaire
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(600, 400);
            this.BackColor = Color.FromArgb(52, 73, 94);
            
            // Ajouter les contrôles
            ConfigurerInterface();
            
            // Timer pour fermer automatiquement
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = ConfigManager.GetDureeSplashScreen();
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Name = "SplashScreen";
            this.ResumeLayout(false);
        }
        
        private void ConfigurerInterface()
        {
            // Logo / Titre
            var lblTitre = new Label
            {
                Text = "DoublonManager",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(this.Width, 80),
                Location = new Point(0, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitre);
            
            // Sous-titre
            var lblSousTitre = new Label
            {
                Text = "Gestionnaire de doublons Amadeus",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(189, 195, 199),
                AutoSize = false,
                Size = new Size(this.Width, 40),
                Location = new Point(0, 160),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblSousTitre);
            
            // Version
            var lblVersion = new Label
            {
                Text = "Version 1.0.0",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(149, 165, 166),
                AutoSize = false,
                Size = new Size(this.Width, 30),
                Location = new Point(0, 210),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblVersion);
            
            // Barre de progression
            var progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Size = new Size(400, 30),
                Location = new Point(100, 280)
            };
            this.Controls.Add(progressBar);
            
            // Statut
            var lblStatut = new Label
            {
                Text = "Chargement en cours...",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(189, 195, 199),
                AutoSize = false,
                Size = new Size(this.Width, 30),
                Location = new Point(0, 330),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblStatut);
        }
        
        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();
            this.Close();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Dessiner une bordure
            using (Pen pen = new Pen(Color.FromArgb(41, 128, 185), 3))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }
    }
}
