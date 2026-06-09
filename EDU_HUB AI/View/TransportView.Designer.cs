using EDU_HUB_AI.Config.Component.Layout;

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
            cardsPanel = new TableLayoutPanel();
            scrollPanel = new Panel();
            pageHeader1 = new PageHeader();
            bodyPanel.SuspendLayout();
            scrollPanel.SuspendLayout();
            SuspendLayout();
            // bodyPanel
            bodyPanel.Controls.Add(scrollPanel);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Location = new Point(0, 64);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Padding = new Padding(24);
            bodyPanel.Size = new Size(840, 616);
            bodyPanel.TabIndex = 1;
            // scrollPanel
            scrollPanel.AutoScroll = true;
            scrollPanel.Controls.Add(cardsPanel);
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.Location = new Point(24, 24);
            scrollPanel.Name = "scrollPanel";
            scrollPanel.Size = new Size(792, 568);
            scrollPanel.TabIndex = 0;
            // cardsPanel
            cardsPanel.AutoSize = true;
            cardsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cardsPanel.ColumnCount = 2;
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            cardsPanel.Dock = DockStyle.Top;
            cardsPanel.Location = new Point(0, 0);
            cardsPanel.Name = "cardsPanel";
            cardsPanel.Padding = new Padding(0, 0, 0, 8);
            cardsPanel.RowCount = 2;
            cardsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            cardsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            cardsPanel.Size = new Size(775, 560);
            cardsPanel.TabIndex = 0;
            // pageHeader1
            pageHeader1.BackColor = Color.FromArgb(228, 231, 240);
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Location = new Point(0, 0);
            pageHeader1.Name = "pageHeader1";
            pageHeader1.Size = new Size(840, 64);
            pageHeader1.TabIndex = 0;
            pageHeader1.Title = "교통 정보 관리";
            // TransportView
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Name = "TransportView";
            Size = new Size(840, 680);
            bodyPanel.ResumeLayout(false);
            scrollPanel.ResumeLayout(false);
            scrollPanel.PerformLayout();
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private Panel scrollPanel;
        private TableLayoutPanel cardsPanel;
        private PageHeader pageHeader1;
    }
}
