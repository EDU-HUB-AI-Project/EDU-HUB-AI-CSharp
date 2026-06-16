using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.View
{
    partial class DashboardView
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
            pageHeader1 = new PageHeader();

            bodyPanel.SuspendLayout();
            SuspendLayout();

            // ── bodyPanel ──────────────────────────────────────
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.BackColor = ThemeColors.Background;
            bodyPanel.Padding = new Padding(20);
            bodyPanel.AutoScroll = true;

            // ── pageHeader1 ────────────────────────────────────
            pageHeader1.BackColor = ThemeColors.HeaderBg;
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Size = new Size(1479, 100);
            pageHeader1.Title = "대시보드";

            // ── DashboardView ──────────────────────────────────
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = ThemeColors.Background;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Size = new Size(1479, 888);

            bodyPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private PageHeader pageHeader1;
    }
}