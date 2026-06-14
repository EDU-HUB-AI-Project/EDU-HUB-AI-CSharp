namespace EDU_HUB_AI.Config.Component.Layout
{
    partial class TabStrip
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
            components = new System.ComponentModel.Container();
            flowTabBar = new FlowLayoutPanel();
            btnTabMeal = new Button();
            btnTabFacility = new Button();
            btnTabTraffic = new Button();
            panelContent = new Panel();
            lblTabPreview = new Label();
            flowTabBar.SuspendLayout();
            panelContent.SuspendLayout();
            SuspendLayout();
            // 
            // flowTabBar
            // 
            flowTabBar.Controls.Add(btnTabMeal);
            flowTabBar.Controls.Add(btnTabFacility);
            flowTabBar.Controls.Add(btnTabTraffic);
            flowTabBar.Dock = DockStyle.Top;
            flowTabBar.Location = new Point(0, 0);
            flowTabBar.Name = "flowTabBar";
            flowTabBar.Padding = new Padding(8, 10, 8, 10);
            flowTabBar.Size = new Size(480, 52);
            flowTabBar.TabIndex = 0;
            flowTabBar.WrapContents = false;
            // 
            // btnTabMeal
            // 
            btnTabMeal.Location = new Point(3, 3);
            btnTabMeal.Margin = new Padding(0, 0, 4, 0);
            btnTabMeal.Name = "btnTabMeal";
            btnTabMeal.Size = new Size(80, 28);
            btnTabMeal.TabIndex = 0;
            btnTabMeal.Text = "구내식당";
            btnTabMeal.UseVisualStyleBackColor = true;
            btnTabMeal.Click += TabDesignButton_Click;
            // 
            // btnTabFacility
            // 
            btnTabFacility.Location = new Point(87, 3);
            btnTabFacility.Margin = new Padding(0, 0, 4, 0);
            btnTabFacility.Name = "btnTabFacility";
            btnTabFacility.Size = new Size(80, 28);
            btnTabFacility.TabIndex = 1;
            btnTabFacility.Text = "시설 위치";
            btnTabFacility.UseVisualStyleBackColor = true;
            btnTabFacility.Click += TabDesignButton_Click;
            // 
            // btnTabTraffic
            // 
            btnTabTraffic.Location = new Point(171, 3);
            btnTabTraffic.Margin = new Padding(0, 0, 4, 0);
            btnTabTraffic.Name = "btnTabTraffic";
            btnTabTraffic.Size = new Size(80, 28);
            btnTabTraffic.TabIndex = 2;
            btnTabTraffic.Text = "교통";
            btnTabTraffic.UseVisualStyleBackColor = true;
            btnTabTraffic.Click += TabDesignButton_Click;
            // 
            // panelContent
            // 
            panelContent.Controls.Add(lblTabPreview);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(0, 40);
            panelContent.Name = "panelContent";
            panelContent.Padding = new Padding(8);
            panelContent.Size = new Size(480, 120);
            panelContent.TabIndex = 1;
            // 
            // lblTabPreview
            // 
            lblTabPreview.AutoSize = true;
            lblTabPreview.Location = new Point(8, 8);
            lblTabPreview.Name = "lblTabPreview";
            lblTabPreview.Size = new Size(139, 15);
            lblTabPreview.TabIndex = 0;
            lblTabPreview.Text = "구내식당 · 식단표 탭 내용";
            // 
            // TabStrip
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panelContent);
            Controls.Add(flowTabBar);
            Name = "TabStrip";
            Size = new Size(480, 160);
            flowTabBar.ResumeLayout(false);
            panelContent.ResumeLayout(false);
            panelContent.PerformLayout();
            ResumeLayout(false);
        }

        private FlowLayoutPanel flowTabBar;
        private Button btnTabMeal;
        private Button btnTabFacility;
        private Button btnTabTraffic;
        private Panel panelContent;
        private Label lblTabPreview;
    }
}
