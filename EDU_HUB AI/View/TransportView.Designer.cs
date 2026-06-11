using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.View
{
    partial class TransportView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            bodyPanel = new Panel();
            scrollPanel = new Panel();
            cardsPanel = new TableLayoutPanel();
            pageHeader1 = new PageHeader();

            bodyPanel.SuspendLayout();
            scrollPanel.SuspendLayout();
            SuspendLayout();

            // ── bodyPanel ──────────────────────────────────────
            bodyPanel.Controls.Add(scrollPanel);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Padding = new Padding(34, 20, 34, 24);
            bodyPanel.BackColor = ThemeColors.Background;

            // ── scrollPanel ────────────────────────────────────
            scrollPanel.Controls.Add(cardsPanel);
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.AutoScroll = true;
            scrollPanel.BackColor = ThemeColors.Background;

            // ── cardsPanel ─────────────────────────────────────
            cardsPanel.ColumnCount = 2;
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            cardsPanel.Dock = DockStyle.Top;
            cardsPanel.AutoSize = true;
            cardsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cardsPanel.RowCount = 2;
            cardsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            cardsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            cardsPanel.BackColor = ThemeColors.Background;
            cardsPanel.Padding = new Padding(0, 0, 0, 8);

            // ── pageHeader1 ────────────────────────────────────
            pageHeader1.BackColor = ThemeColors.HeaderBg;
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Size = new Size(1479, 100);
            pageHeader1.Title = "교통 정보 관리";

            // ── TransportView ──────────────────────────────────
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = ThemeColors.Background;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Size = new Size(1479, 888);

            scrollPanel.ResumeLayout(false);
            scrollPanel.PerformLayout();
            bodyPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private Panel scrollPanel;
        private TableLayoutPanel cardsPanel;
        private PageHeader pageHeader1;
    }
}