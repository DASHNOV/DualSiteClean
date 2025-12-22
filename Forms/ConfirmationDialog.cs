using System;
using System.Drawing;
using System.Windows.Forms;
using DoublonManager.Helpers;

namespace DoublonManager.Forms
{
    public partial class ConfirmationDialog : Form
    {
        public bool IsConfirmed { get; private set; }

        public ConfirmationDialog(string message, string title = "Confirmation")
        {
            InitializeComponent(message, title);
        }

        private void InitializeComponent(string message, string title)
        {
            this.Text = title;
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(20);
            layout.RowCount = 2;
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            this.Controls.Add(layout);

            var lblMessage = new Label();
            lblMessage.Text = message;
            lblMessage.Font = new Font("Segoe UI", 11F);
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            lblMessage.Dock = DockStyle.Fill;
            layout.Controls.Add(lblMessage, 0, 0);

            var pnlButtons = new FlowLayoutPanel();
            pnlButtons.Dock = DockStyle.Fill;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            
            var btnCancel = new Button();
            btnCancel.Text = "Annuler";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Click += (s, e) => { IsConfirmed = false; this.Close(); };
            
            var btnOk = new Button();
            btnOk.Text = "Confirmer";
            btnOk.BackColor = ColorHelper.Danger;
            btnOk.ForeColor = Color.White;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Click += (s, e) => { IsConfirmed = true; this.Close(); };

            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Controls.Add(btnOk);
            layout.Controls.Add(pnlButtons, 0, 1);
        }
    }
}
