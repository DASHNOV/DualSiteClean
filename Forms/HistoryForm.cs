using System;
using System.Drawing;
using System.Windows.Forms;
using DoublonManager.Helpers;

namespace DoublonManager.Forms
{
    public partial class HistoryForm : Form
    {
        private ListView lvHistory;

        public HistoryForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Historique des opérations";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorHelper.Neutral;

            // Layout
            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(20);
            layout.RowCount = 2;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Header
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // List
            this.Controls.Add(layout);

            // Header
            Label lblTitle = new Label();
            lblTitle.Text = "Historique";
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = ColorHelper.Primary;
            lblTitle.AutoSize = true;
            layout.Controls.Add(lblTitle, 0, 0);

            // List
            lvHistory = new ListView();
            lvHistory.View = View.Details;
            lvHistory.Dock = DockStyle.Fill;
            lvHistory.FullRowSelect = true;
            lvHistory.GridLines = true;
            lvHistory.Font = new Font("Segoe UI", 10F);
            
            lvHistory.Columns.Add("Date", 150);
            lvHistory.Columns.Add("Action", 150);
            lvHistory.Columns.Add("Détails", 400);

            // Mock Data
            var item1 = new ListViewItem(DateTime.Now.AddDays(-1).ToString("g"));
            item1.SubItems.Add("Analyse");
            item1.SubItems.Add("Analyse lancée - 12 doublons trouvés");

            var item2 = new ListViewItem(DateTime.Now.AddHours(-2).ToString("g"));
            item2.SubItems.Add("Suppression");
            item2.SubItems.Add("Suppression doublon ID 102 (Site 19M)");

            lvHistory.Items.AddRange(new ListViewItem[] { item1, item2 });

            layout.Controls.Add(lvHistory, 0, 1);
        }
    }
}
