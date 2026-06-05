namespace EDU_HUB_AI.Config.Component.Data
{
    partial class Pagination
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
            lblInfo = new Label();
            flowPages = new FlowLayoutPanel();
            btnPrev = new Button();
            btnPage1 = new Button();
            btnPage2 = new Button();
            btnPage3 = new Button();
            lblEllipsis = new Label();
            btnPageLast = new Button();
            btnNext = new Button();
            flowPages.SuspendLayout();
            SuspendLayout();
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(12, 16);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(141, 15);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "총 128건 · 1 / 13 페이지";
            // 
            // flowPages
            // 
            flowPages.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            flowPages.AutoSize = true;
            flowPages.Controls.Add(btnPrev);
            flowPages.Controls.Add(btnPage1);
            flowPages.Controls.Add(btnPage2);
            flowPages.Controls.Add(btnPage3);
            flowPages.Controls.Add(lblEllipsis);
            flowPages.Controls.Add(btnPageLast);
            flowPages.Controls.Add(btnNext);
            flowPages.Location = new Point(520, 10);
            flowPages.Name = "flowPages";
            flowPages.Size = new Size(348, 30);
            flowPages.TabIndex = 1;
            flowPages.WrapContents = false;
            // 
            // btnPrev
            // 
            btnPrev.Location = new Point(3, 3);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(52, 24);
            btnPrev.TabIndex = 0;
            btnPrev.Text = "이전";
            btnPrev.UseVisualStyleBackColor = true;
            btnPrev.Click += btnPrev_Click;
            // 
            // btnPage1
            // 
            btnPage1.Location = new Point(61, 3);
            btnPage1.Name = "btnPage1";
            btnPage1.Size = new Size(32, 24);
            btnPage1.TabIndex = 1;
            btnPage1.Text = "1";
            btnPage1.UseVisualStyleBackColor = true;
            // 
            // btnPage2
            // 
            btnPage2.Location = new Point(99, 3);
            btnPage2.Name = "btnPage2";
            btnPage2.Size = new Size(32, 24);
            btnPage2.TabIndex = 2;
            btnPage2.Text = "2";
            btnPage2.UseVisualStyleBackColor = true;
            // 
            // btnPage3
            // 
            btnPage3.Location = new Point(137, 3);
            btnPage3.Name = "btnPage3";
            btnPage3.Size = new Size(32, 24);
            btnPage3.TabIndex = 3;
            btnPage3.Text = "3";
            btnPage3.UseVisualStyleBackColor = true;
            // 
            // lblEllipsis
            // 
            lblEllipsis.AutoSize = true;
            lblEllipsis.Location = new Point(175, 8);
            lblEllipsis.Margin = new Padding(3, 8, 3, 0);
            lblEllipsis.Name = "lblEllipsis";
            lblEllipsis.Size = new Size(16, 15);
            lblEllipsis.TabIndex = 4;
            lblEllipsis.Text = "…";
            // 
            // btnPageLast
            // 
            btnPageLast.Location = new Point(197, 3);
            btnPageLast.Name = "btnPageLast";
            btnPageLast.Size = new Size(32, 24);
            btnPageLast.TabIndex = 5;
            btnPageLast.Text = "13";
            btnPageLast.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(235, 3);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(52, 24);
            btnNext.TabIndex = 6;
            btnNext.Text = "다음";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // Pagination
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(flowPages);
            Controls.Add(lblInfo);
            Name = "Pagination";
            Size = new Size(880, 48);
            Resize += Pagination_Resize;
            flowPages.ResumeLayout(false);
            flowPages.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblInfo;
        private FlowLayoutPanel flowPages;
        private Button btnPrev;
        private Button btnPage1;
        private Button btnPage2;
        private Button btnPage3;
        private Label lblEllipsis;
        private Button btnPageLast;
        private Button btnNext;
    }
}
