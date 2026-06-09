using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;

namespace EDU_HUB_AI.View
{
    partial class ClassroomView
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
            txtSearch = new TextBox();
            pagination1 = new Pagination();
            pageHeader1 = new PageHeader();
            cmbFloor = new ComboBox();
            bodyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            actionPanel.SuspendLayout();
            SuspendLayout();

            var pnlSearch = new Panel();
            var lblSearch = new Label();
            var pnlFloor = new Panel();
            var lblFloor = new Label();

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
            actionPanel.Controls.Add(pnlSearch);
            actionPanel.Controls.Add(pnlFloor);
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
            // lblSearch
            //
            lblSearch.AutoSize = true;
            lblSearch.Font = new Font("맑은 고딕", 8.25F);
            lblSearch.ForeColor = Color.FromArgb(100, 116, 139);
            lblSearch.Location = new Point(0, 0);
            lblSearch.Text = "검색";
            // 
            // txtSearch
            // 
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("맑은 고딕", 9F);
            txtSearch.Location = new Point(0, 18);
            txtSearch.Size = new Size(280, 23);
            txtSearch.PlaceholderText = "강의실명으로 검색";
            txtSearch.TabIndex = 2;
            //
            // pnlSearch
            //
            pnlSearch.BackColor = Color.FromArgb(244, 246, 249);
            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Size = new Size(280, 54);
            pnlSearch.Margin = new Padding(0, 0, 0, 0);
            //
            // cmbFloor
            //
            cmbFloor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFloor.Font = new Font("맑은 고딕", 9F);
            cmbFloor.Location = new Point(0, 18);
            cmbFloor.Size = new Size(120, 23);
            cmbFloor.TabIndex = 3;
            //
            // lblFloor
            //
            lblFloor.AutoSize = true;
            lblFloor.Font = new Font("맑은 고딕", 8.25F);
            lblFloor.ForeColor = Color.FromArgb(100, 116, 139);
            lblFloor.Location = new Point(0, 0);
            lblFloor.Text = "층";
            //
            // pnlFloor
            //
            pnlFloor.BackColor = Color.FromArgb(244, 246, 249);
            pnlFloor.Controls.Add(lblFloor);
            pnlFloor.Controls.Add(cmbFloor);
            pnlFloor.Size = new Size(120, 54);
            pnlFloor.Margin = new Padding(0, 0, 12, 0);
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
            pageHeader1.Title = "강의실 관리";
            // 
            // ClassroomView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Name = "ClassroomView";
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
        private ComboBox cmbFloor;
        private TextBox txtSearch;
    }
}
