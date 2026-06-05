namespace EDU_HUB_AI.Config.Component.Layout
{
    partial class PageHeader
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _clock.Stop();
                _clock.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblClock = new Label();
            btnSync = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(24, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(71, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "페이지 제목";
            // 
            // lblClock
            // 
            lblClock.AutoSize = true;
            lblClock.Location = new Point(600, 24);
            lblClock.Name = "lblClock";
            lblClock.Size = new Size(122, 15);
            lblClock.TabIndex = 1;
            lblClock.Text = "2026. 1. 1.  00:00:00";
            // 
            // btnSync
            // 
            btnSync.AutoSize = true;
            btnSync.Location = new Point(740, 16);
            btnSync.Name = "btnSync";
            btnSync.Size = new Size(68, 28);
            btnSync.TabIndex = 2;
            btnSync.Text = "동기화";
            btnSync.UseVisualStyleBackColor = true;
            // 
            // PageHeader
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnSync);
            Controls.Add(lblClock);
            Controls.Add(lblTitle);
            Name = "PageHeader";
            Size = new Size(840, 64);
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle;
        private Label lblClock;
        private Button btnSync;
    }
}
