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
            btnKLGet = new Button();
            btnKLDel = new Button();
            btnKLPatch = new Button();
            btnKLPost = new Button();
            SuspendLayout();
            // 
            // btnTest
            // 
            btnTest.Location = new Point(86, 55);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(159, 62);
            btnTest.TabIndex = 0;
            btnTest.Text = "GET테스트";
            btnTest.UseVisualStyleBackColor = true;
            // 
            // btnTestPost
            // 
            btnTestPost.Location = new Point(86, 137);
            btnTestPost.Name = "btnTestPost";
            btnTestPost.Size = new Size(159, 56);
            btnTestPost.TabIndex = 1;
            btnTestPost.Text = "POST테스트";
            btnTestPost.UseVisualStyleBackColor = true;
            // 
            // btnTestPatch
            // 
            btnTestPatch.Location = new Point(86, 223);
            btnTestPatch.Name = "btnTestPatch";
            btnTestPatch.Size = new Size(159, 56);
            btnTestPatch.TabIndex = 2;
            btnTestPatch.Text = "PATCH테스트";
            btnTestPatch.UseVisualStyleBackColor = true;
            // 
            // btnTestDelete
            // 
            btnTestDelete.Location = new Point(86, 307);
            btnTestDelete.Name = "btnTestDelete";
            btnTestDelete.Size = new Size(159, 56);
            btnTestDelete.TabIndex = 3;
            btnTestDelete.Text = "DELETE테스트";
            btnTestDelete.UseVisualStyleBackColor = true;
            // 
            // btnKLGet
            // 
            btnKLGet.Location = new Point(389, 55);
            btnKLGet.Name = "btnKLGet";
            btnKLGet.Size = new Size(150, 62);
            btnKLGet.TabIndex = 4;
            btnKLGet.Text = "KL GET 테스트";
            btnKLGet.UseVisualStyleBackColor = true;
            // 
            // btnKLDel
            // 
            btnKLDel.Location = new Point(389, 307);
            btnKLDel.Name = "btnKLDel";
            btnKLDel.Size = new Size(150, 56);
            btnKLDel.TabIndex = 5;
            btnKLDel.Text = "KL DEL 테스트";
            btnKLDel.UseVisualStyleBackColor = true;
            // 
            // btnKLPatch
            // 
            btnKLPatch.Location = new Point(389, 223);
            btnKLPatch.Name = "btnKLPatch";
            btnKLPatch.Size = new Size(150, 56);
            btnKLPatch.TabIndex = 6;
            btnKLPatch.Text = "KL PATCH 테스트";
            btnKLPatch.UseVisualStyleBackColor = true;
            // 
            // btnKLPost
            // 
            btnKLPost.Location = new Point(389, 137);
            btnKLPost.Name = "btnKLPost";
            btnKLPost.Size = new Size(150, 56);
            btnKLPost.TabIndex = 7;
            btnKLPost.Text = "KL POST 테스트";
            btnKLPost.UseVisualStyleBackColor = true;
            // 
            // testForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnKLPost);
            Controls.Add(btnKLPatch);
            Controls.Add(btnKLDel);
            Controls.Add(btnKLGet);
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
        private Button btnKLGet;
        private Button btnKLDel;
        private Button btnKLPatch;
        private Button btnKLPost;
    }
}