using System;
using System.Drawing;
using System.Windows.Forms;
using DoublonManager.Helpers;

namespace DoublonManager.Forms
{
    public partial class SettingsForm : Form
    {
        private TextBox txtConn39C;
        private TextBox txtConn19M;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Paramètres";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorHelper.Neutral;

            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(20);
            layout.RowCount = 3;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.Controls.Add(layout);

            // Conn 39C
            var pnl39C = new Panel { Dock = DockStyle.Top, Height = 60 };
            pnl39C.Controls.Add(new Label { Text = "Connection String 39C:", Top = 5, AutoSize = true });
            txtConn39C = new TextBox { Top = 30, Width = 400, Text = "Data Source=SRV39C;Initial Catalog=DB;Integrated Security=True" };
            pnl39C.Controls.Add(txtConn39C);
            layout.Controls.Add(pnl39C, 0, 0);

            // Conn 19M
            var pnl19M = new Panel { Dock = DockStyle.Top, Height = 60 };
            pnl19M.Controls.Add(new Label { Text = "Connection String 19M:", Top = 5, AutoSize = true });
            txtConn19M = new TextBox { Top = 30, Width = 400, Text = "Data Source=SRV19M;Initial Catalog=DB;Integrated Security=True" };
            pnl19M.Controls.Add(txtConn19M);
            layout.Controls.Add(pnl19M, 0, 1);

            // Save
            var btnSave = new Button();
            btnSave.Text = "Enregistrer";
            btnSave.BackColor = ColorHelper.Success;
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += (s, e) => { MessageBox.Show("Paramètres enregistrés"); this.Close(); };
            layout.Controls.Add(btnSave, 0, 2);
        }
    }
}
