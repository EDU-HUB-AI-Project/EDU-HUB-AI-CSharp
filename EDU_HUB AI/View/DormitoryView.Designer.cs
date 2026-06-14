using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.View
{
    partial class DormitoryView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            bodyPanel = new Panel();
            pageHeader1 = new PageHeader();
            bodyPanel.SuspendLayout();
            SuspendLayout();

            // bodyPanel
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.BackColor = ThemeColors.Background;
            bodyPanel.Padding = new Padding(0);

            // pageHeader1
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Size = new Size(1479, 60);
            pageHeader1.BackColor = ThemeColors.HeaderBg;
            pageHeader1.Title = "생활관 배정 현황";

            // DormitoryView
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            BackColor = ThemeColors.Background;
            Size = new Size(1479, 888);
            bodyPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private PageHeader pageHeader1;
    }
}