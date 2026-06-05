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
            components = new System.ComponentModel.Container();
            btnBack = new Button();
            dgvAttend = new DataGridView();
            bsAttend = new BindingSource(components);
            btnExport = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvAttend).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsAttend).BeginInit();
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
            btnBack.Click += BtnBack_Click;
            // 
            // dgvAttend
            // 
            dgvAttend.BackgroundColor = SystemColors.ButtonFace;
            dgvAttend.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAttend.GridColor = SystemColors.Info;
            dgvAttend.Location = new Point(100, 104);
            dgvAttend.Name = "dgvAttend";
            dgvAttend.RowHeadersWidth = 51;
            dgvAttend.Size = new Size(1100, 418);
            dgvAttend.TabIndex = 1;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(100, 537);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(142, 48);
            btnExport.TabIndex = 2;
            btnExport.Text = "엑셀로 내보내기";
            btnExport.UseVisualStyleBackColor = true;
            // 
            // Form_Attend
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1243, 643);
            Controls.Add(btnExport);
            Controls.Add(dgvAttend);
            Controls.Add(btnBack);
            Name = "Form_Attend";
            Text = "testAttendForm";
            ((System.ComponentModel.ISupportInitialize)dgvAttend).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsAttend).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnBack;
        private DataGridView dgvAttend;
        private BindingSource bsAttend;
        private Button btnExport;
    }
}