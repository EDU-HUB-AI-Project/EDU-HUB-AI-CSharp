namespace EDU_HUB_AI.View
{
    partial class MainShellForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            navigation1 = new EDU_HUB_AI.Config.Component.Layout.Navigation();
            contentPanel = new Panel();
            SuspendLayout();
            // 
            // navigation1
            // 
            navigation1.ActiveMenu = Config.Component.Common.MenuKey.Dashboard;
            navigation1.BackColor = Color.FromArgb(43, 50, 66);
            navigation1.Dock = DockStyle.Left;
            navigation1.Location = new Point(0, 0);
            navigation1.Name = "navigation1";
            navigation1.TabIndex = 0;
            // 
            // contentPanel
            // 
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(280, 0);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(1508, 1024);
            contentPanel.TabIndex = 2;
            // 
            // MainShellForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1898, 1024);
            Controls.Add(contentPanel);
            Controls.Add(navigation1);
            Name = "MainShellForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainShellForm";
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
        }

        #endregion

        private Config.Component.Layout.Navigation navigation1;
        private Panel contentPanel;
    }
}