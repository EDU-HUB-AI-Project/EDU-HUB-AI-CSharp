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
            btnTestPost = new Button();
            btnTestPatch = new Button();
            btnTestDelete = new Button();
            SuspendLayout();
            // 
            // btnTest
            // 
            btnTest.Location = new Point(312, 46);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(159, 62);
            btnTest.TabIndex = 0;
            btnTest.Text = "GET테스트";
            btnTest.UseVisualStyleBackColor = true;
            // 
            // btnTestPost
            // 
            btnTestPost.Location = new Point(312, 128);
            btnTestPost.Name = "btnTestPost";
            btnTestPost.Size = new Size(159, 56);
            btnTestPost.TabIndex = 1;
            btnTestPost.Text = "POST테스트";
            btnTestPost.UseVisualStyleBackColor = true;
            // 
            // btnTestPatch
            // 
            btnTestPatch.Location = new Point(312, 214);
            btnTestPatch.Name = "btnTestPatch";
            btnTestPatch.Size = new Size(159, 56);
            btnTestPatch.TabIndex = 2;
            btnTestPatch.Text = "PATCH테스트";
            btnTestPatch.UseVisualStyleBackColor = true;
            // 
            // btnTestDelete
            // 
            btnTestDelete.Location = new Point(312, 298);
            btnTestDelete.Name = "btnTestDelete";
            btnTestDelete.Size = new Size(159, 56);
            btnTestDelete.TabIndex = 3;
            btnTestDelete.Text = "DELETE테스트";
            btnTestDelete.UseVisualStyleBackColor = true;
            // 
            // testForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnTestDelete);
            Controls.Add(btnTestPatch);
            Controls.Add(btnTestPost);
            Controls.Add(btnTest);
            Name = "testForm";
            Text = "testForm";
            ResumeLayout(false);
        }

        #endregion

        private Button btnTest;
        private Button btnTestPost;
        private Button btnTestPatch;
        private Button btnTestDelete;
    }
}