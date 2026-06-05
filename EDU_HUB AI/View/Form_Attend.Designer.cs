namespace EDU_HUB_AI.View
{
    partial class Form_Attend
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
            btnBack = new Button();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Location = new Point(54, 12);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(94, 29);
            btnBack.TabIndex = 0;
            btnBack.Text = "뒤로가기";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += this.BtnBack_Click;
            // 
            // Form_Attend
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnBack);
            Name = "Form_Attend";
            Text = "testAttendForm";
            ResumeLayout(false);
        }

        #endregion

        private Button btnBack;
    }
}