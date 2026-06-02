namespace EDU_HUB_AI.View
{
    partial class testForm
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
            btnTest = new Button();
            SuspendLayout();
            // 
            // btnTest
            // 
            btnTest.Location = new Point(312, 161);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(159, 62);
            btnTest.TabIndex = 0;
            btnTest.Text = "테스트";
            btnTest.UseVisualStyleBackColor = true;
            // 
            // testForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnTest);
            Name = "testForm";
            Text = "testForm";
            ResumeLayout(false);
        }

        #endregion

        private Button btnTest;
    }
}