using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;

namespace EDU_HUB_AI.View
{
    partial class StudentView
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            bodyPanel = new Panel();
            grid = new AppDataGrid();
            actionPanel = new ActionBar();
            btnCreate = new AppButton();
            btnExcel = new AppButton();
            txtSearch = new TextField();
            cmbEdu = new ComboBox();
            cmbBatch = new ComboBox();
            pagination1 = new Pagination();
            pageHeader1 = new PageHeader();
            bodyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            actionPanel.SuspendLayout();
            SuspendLayout();
            // 
            // bodyPanel
            // 
            bodyPanel.Controls.Add(grid);
            bodyPanel.Controls.Add(actionPanel);
            bodyPanel.Controls.Add(pagination1);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Location = new Point(0, 107);
            bodyPanel.Margin = new Padding(4, 5, 4, 5);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Padding = new Padding(34, 40, 34, 40);
            bodyPanel.Size = new Size(1479, 781);
            bodyPanel.TabIndex = 1;
            // 
            // grid
            // 
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(250, 251, 252);
            grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(241, 245, 249);
            dataGridViewCellStyle2.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle2.Padding = new Padding(8, 0, 8, 0);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(241, 245, 249);
            grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            grid.ColumnHeadersHeight = 36;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("맑은 고딕", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle3.Padding = new Padding(8, 0, 8, 0);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(248, 250, 252);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            grid.DefaultCellStyle = dataGridViewCellStyle3;
            grid.Dock = DockStyle.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.Font = new Font("맑은 고딕", 9F);
            grid.GridColor = Color.FromArgb(226, 232, 240);
            grid.Location = new Point(34, 130);
            grid.Margin = new Padding(4, 5, 4, 5);
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.RowHeadersWidth = 62;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(1411, 531);
            grid.TabIndex = 0;
            // 
            // actionPanel
            // 
            actionPanel.BackColor = Color.FromArgb(244, 246, 249);
            actionPanel.Controls.Add(btnCreate);
            actionPanel.Controls.Add(btnExcel);
            actionPanel.Controls.Add(txtSearch);
            actionPanel.Controls.Add(cmbEdu);
            actionPanel.Controls.Add(cmbBatch);
            actionPanel.Dock = DockStyle.Top;
            actionPanel.FlowDirection = FlowDirection.RightToLeft;
            actionPanel.Location = new Point(34, 40);
            actionPanel.Margin = new Padding(0);
            actionPanel.Name = "actionPanel";
            actionPanel.Padding = new Padding(0, 7, 0, 13);
            actionPanel.Size = new Size(1411, 90);
            actionPanel.TabIndex = 2;
            actionPanel.WrapContents = false;
            // 
            // btnCreate
            // 
            btnCreate.AutoSize = true;
            btnCreate.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCreate.BackColor = Color.FromArgb(37, 99, 235);
            btnCreate.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnCreate.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.Font = new Font("맑은 고딕", 9F);
            btnCreate.ForeColor = Color.White;
            btnCreate.Location = new Point(1257, 7);
            btnCreate.Margin = new Padding(0);
            btnCreate.Name = "btnCreate";
            btnCreate.Padding = new Padding(17, 10, 17, 10);
            btnCreate.Size = new Size(154, 57);
            btnCreate.TabIndex = 0;
            btnCreate.Tag = ButtonVariant.Primary;
            btnCreate.Text = "교육생 추가";
            btnCreate.UseVisualStyleBackColor = false;
            // 
            // btnExcel
            // 
            btnExcel.AutoSize = true;
            btnExcel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnExcel.BackColor = Color.White;
            btnExcel.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnExcel.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
            btnExcel.FlatStyle = FlatStyle.Flat;
            btnExcel.Font = new Font("맑은 고딕", 9F);
            btnExcel.ForeColor = Color.FromArgb(37, 99, 235);
            btnExcel.Location = new Point(1078, 7);
            btnExcel.Margin = new Padding(0, 0, 11, 0);
            btnExcel.Name = "btnExcel";
            btnExcel.Padding = new Padding(12, 6, 12, 6);
            btnExcel.Size = new Size(168, 49);
            btnExcel.TabIndex = 1;
            btnExcel.Tag = ButtonVariant.Secondary;
            btnExcel.Text = "엑셀 일괄 등록";
            btnExcel.UseVisualStyleBackColor = false;
            btnExcel.Variant = ButtonVariant.Secondary;
            btnExcel.Click += btnExcel_Click;
            // 
            // txtSearch
            // 
            txtSearch.BackColor = Color.White;
            txtSearch.Location = new Point(706, 10);
            txtSearch.MinimumSize = new Size(120, 54);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(369, 81);
            txtSearch.TabIndex = 2;
            // 
            // cmbEdu
            // 
            cmbEdu.FormattingEnabled = true;
            cmbEdu.Location = new Point(518, 10);
            cmbEdu.Name = "cmbEdu";
            cmbEdu.Size = new Size(182, 33);
            cmbEdu.TabIndex = 3;
            // 
            // cmbBatch
            // 
            cmbBatch.FormattingEnabled = true;
            cmbBatch.Location = new Point(330, 10);
            cmbBatch.Name = "cmbBatch";
            cmbBatch.Size = new Size(182, 33);
            cmbBatch.TabIndex = 4;
            // 
            // pagination1
            // 
            pagination1.BackColor = Color.FromArgb(244, 246, 249);
            pagination1.Dock = DockStyle.Bottom;
            pagination1.Location = new Point(34, 661);
            pagination1.Margin = new Padding(6, 8, 6, 8);
            pagination1.Name = "pagination1";
            pagination1.PageIndex = 1;
            pagination1.PageSize = 10;
            pagination1.Size = new Size(1411, 80);
            pagination1.TabIndex = 1;
            pagination1.TotalCount = 0;
            // 
            // pageHeader1
            // 
            pageHeader1.BackColor = Color.FromArgb(228, 231, 240);
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Location = new Point(0, 0);
            pageHeader1.Margin = new Padding(6, 8, 6, 8);
            pageHeader1.Name = "pageHeader1";
            pageHeader1.Size = new Size(1479, 107);
            pageHeader1.TabIndex = 0;
            pageHeader1.Title = "교육생 관리";
            // 
            // StudentView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Name = "StudentView";
            Size = new Size(1479, 888);
            bodyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            actionPanel.ResumeLayout(false);
            actionPanel.PerformLayout();
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private PageHeader pageHeader1;
        private AppDataGrid grid;
        private Pagination pagination1;
        private ActionBar actionPanel;
        private AppButton btnExcel;
        private AppButton btnCreate;
        private TextField txtSearch;
        private ComboBox cmbEdu;
        private ComboBox cmbBatch;
    }
}
