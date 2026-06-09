using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;

namespace EDU_HUB_AI.View
{
    partial class AttendanceView
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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            bodyPanel = new Panel();
            grid = new AppDataGrid();
            actionPanel = new ActionBar();
            btnCreate = new AppButton();
            btnImport = new AppButton();
            btnExport = new AppButton();
            btnSearch = new AppButton();
            cmbStatus = new ComboBox();
            cmbEdu = new ComboBox();
            cmbStudent = new ComboBox();
            dtpDate = new DateTimePicker();
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
            bodyPanel.Location = new Point(0, 64);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Padding = new Padding(24, 24, 24, 24);
            bodyPanel.Size = new Size(1150, 602);
            bodyPanel.TabIndex = 1;
            // 
            // grid
            // 
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(250, 251, 252);
            grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(241, 245, 249);
            dataGridViewCellStyle5.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle5.Padding = new Padding(8, 0, 8, 0);
            dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(241, 245, 249);
            grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            grid.ColumnHeadersHeight = 36;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.White;
            dataGridViewCellStyle6.Font = new Font("맑은 고딕", 9F);
            dataGridViewCellStyle6.ForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle6.Padding = new Padding(8, 0, 8, 0);
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(248, 250, 252);
            dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            grid.DefaultCellStyle = dataGridViewCellStyle6;
            grid.Dock = DockStyle.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.Font = new Font("맑은 고딕", 9F);
            grid.GridColor = Color.FromArgb(226, 232, 240);
            grid.Location = new Point(24, 92);
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.RowHeadersWidth = 51;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(1102, 438);
            grid.TabIndex = 0;
            // 
            // actionPanel
            // 
            actionPanel.BackColor = Color.FromArgb(244, 246, 249);
            actionPanel.Controls.Add(btnCreate);
            actionPanel.Controls.Add(btnImport);
            actionPanel.Controls.Add(btnExport);
            actionPanel.Controls.Add(btnSearch);
            actionPanel.Controls.Add(cmbStatus);
            actionPanel.Controls.Add(cmbEdu);
            actionPanel.Controls.Add(cmbStudent);
            actionPanel.Controls.Add(dtpDate);
            actionPanel.Dock = DockStyle.Top;
            actionPanel.FlowDirection = FlowDirection.RightToLeft;
            actionPanel.Location = new Point(24, 24);
            actionPanel.Margin = new Padding(0);
            actionPanel.Name = "actionPanel";
            actionPanel.Padding = new Padding(0, 4, 0, 8);
            actionPanel.Size = new Size(1102, 68);
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
            btnCreate.Location = new Point(979, 4);
            btnCreate.Margin = new Padding(0);
            btnCreate.Name = "btnCreate";
            btnCreate.Padding = new Padding(12, 6, 12, 6);
            btnCreate.Size = new Size(123, 39);
            btnCreate.TabIndex = 0;
            btnCreate.Tag = ButtonVariant.Primary;
            btnCreate.Text = "출석 현황 추가";
            btnCreate.UseVisualStyleBackColor = false;
            // 
            // btnImport
            // 
            btnImport.AutoSize = true;
            btnImport.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnImport.BackColor = Color.White;
            btnImport.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnImport.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.Font = new Font("맑은 고딕", 9F);
            btnImport.ForeColor = Color.FromArgb(37, 99, 235);
            btnImport.Location = new Point(854, 4);
            btnImport.Margin = new Padding(0, 0, 8, 0);
            btnImport.Name = "btnImport";
            btnImport.Padding = new Padding(9, 4, 9, 4);
            btnImport.Size = new Size(117, 35);
            btnImport.TabIndex = 1;
            btnImport.Tag = ButtonVariant.Secondary;
            btnImport.Text = "엑셀 일괄 등록";
            btnImport.UseVisualStyleBackColor = false;
            btnImport.Variant = ButtonVariant.Secondary;
            // 
            // btnExport
            // 
            btnExport.AutoSize = true;
            btnExport.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnExport.BackColor = Color.White;
            btnExport.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnExport.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Font = new Font("맑은 고딕", 9F);
            btnExport.ForeColor = Color.FromArgb(37, 99, 235);
            btnExport.Location = new Point(727, 6);
            btnExport.Margin = new Padding(2, 2, 2, 2);
            btnExport.Name = "btnExport";
            btnExport.Padding = new Padding(9, 4, 9, 4);
            btnExport.Size = new Size(125, 35);
            btnExport.TabIndex = 2;
            btnExport.Tag = ButtonVariant.Secondary;
            btnExport.Text = "엑셀로 내보내기";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Variant = ButtonVariant.Secondary;
            // 
            // btnSearch
            // 
            btnSearch.AutoSize = true;
            btnSearch.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSearch.BackColor = Color.White;
            btnSearch.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnSearch.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Font = new Font("맑은 고딕", 9F);
            btnSearch.ForeColor = Color.FromArgb(37, 99, 235);
            btnSearch.Location = new Point(638, 6);
            btnSearch.Margin = new Padding(2, 2, 2, 2);
            btnSearch.Name = "btnSearch";
            btnSearch.Padding = new Padding(9, 4, 9, 4);
            btnSearch.Size = new Size(85, 35);
            btnSearch.TabIndex = 3;
            btnSearch.Tag = ButtonVariant.Secondary;
            btnSearch.Text = "조회하기";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Variant = ButtonVariant.Secondary;
            // 
            // cmbStatus
            // 
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Location = new Point(559, 6);
            cmbStatus.Margin = new Padding(2, 2, 2, 2);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(75, 23);
            cmbStatus.TabIndex = 4;
            // 
            // cmbEdu
            // 
            cmbEdu.FormattingEnabled = true;
            cmbEdu.Location = new Point(480, 6);
            cmbEdu.Margin = new Padding(2, 2, 2, 2);
            cmbEdu.Name = "cmbEdu";
            cmbEdu.Size = new Size(75, 23);
            cmbEdu.TabIndex = 5;
            // 
            // cmbStudent
            // 
            cmbStudent.FormattingEnabled = true;
            cmbStudent.Location = new Point(401, 6);
            cmbStudent.Margin = new Padding(2, 2, 2, 2);
            cmbStudent.Name = "cmbStudent";
            cmbStudent.Size = new Size(75, 23);
            cmbStudent.TabIndex = 6;
            // 
            // dtpDate
            // 
            dtpDate.Location = new Point(318, 6);
            dtpDate.Margin = new Padding(2, 2, 2, 2);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(79, 23);
            dtpDate.TabIndex = 7;
            // 
            // pagination1
            // 
            pagination1.BackColor = Color.FromArgb(244, 246, 249);
            pagination1.Dock = DockStyle.Bottom;
            pagination1.Location = new Point(24, 530);
            pagination1.Margin = new Padding(4, 4, 4, 4);
            pagination1.Name = "pagination1";
            pagination1.PageIndex = 1;
            pagination1.PageSize = 10;
            pagination1.Size = new Size(1102, 48);
            pagination1.TabIndex = 1;
            pagination1.TotalCount = 0;
            // 
            // pageHeader1
            // 
            pageHeader1.BackColor = Color.FromArgb(228, 231, 240);
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Location = new Point(0, 0);
            pageHeader1.Margin = new Padding(4, 4, 4, 4);
            pageHeader1.Name = "pageHeader1";
            pageHeader1.Size = new Size(1150, 64);
            pageHeader1.TabIndex = 0;
            pageHeader1.Title = "출석현황";
            // 
            // AttendanceView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Margin = new Padding(2, 2, 2, 2);
            Name = "AttendanceView";
            Size = new Size(1150, 666);
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
        private AppButton btnImport;
        private AppButton btnCreate;
        private AppButton btnExport;
        private AppButton btnSearch;
        private ComboBox cmbStatus;
        private ComboBox cmbEdu;
        private ComboBox cmbStudent;
        private DateTimePicker dtpDate;
    }
}