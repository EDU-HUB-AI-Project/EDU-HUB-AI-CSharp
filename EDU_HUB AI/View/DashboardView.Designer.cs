using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;

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
            SuspendLayout();
            // 
            // bodyPanel
            // 
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Location = new Point(0, 85);
            bodyPanel.Margin = new Padding(4);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Padding = new Padding(31, 32, 31, 32);
            bodyPanel.Size = new Size(1331, 625);
            bodyPanel.TabIndex = 1;
            // 
            // pageHeader1
            // 
            pageHeader1.BackColor = Color.FromArgb(228, 231, 240);
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Location = new Point(0, 0);
            pageHeader1.Margin = new Padding(5);
            pageHeader1.Name = "pageHeader1";
            pageHeader1.Size = new Size(1331, 85);
            pageHeader1.TabIndex = 0;
            pageHeader1.Title = "관리자 대시보드";
            // 
            // DashboardView
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Name = "DashboardView";
            Size = new Size(1479, 888);
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private PageHeader pageHeader1;
    }
}