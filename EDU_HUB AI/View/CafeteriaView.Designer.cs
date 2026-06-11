using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;

namespace EDU_HUB_AI.View
{
    partial class CafeteriaView
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
            pagination1 = new Pagination();
            pageHeader1 = new PageHeader();
            bodyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();
            // bodyPanel
            bodyPanel.Controls.Add(grid);
            bodyPanel.Controls.Add(pagination1);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Location = new Point(0, 107);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Padding = new Padding(34, 40, 34, 40);
            bodyPanel.Size = new Size(1479, 781);
            bodyPanel.TabIndex = 1;
            // grid
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
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.RowHeadersWidth = 62;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(1411, 531);
            grid.TabIndex = 0;
            // pagination1
            pagination1.BackColor = Color.FromArgb(244, 246, 249);
            pagination1.Dock = DockStyle.Bottom;
            pagination1.Location = new Point(34, 661);
            pagination1.Name = "pagination1";
            pagination1.PageIndex = 1;
            pagination1.PageSize = 10;
            pagination1.Size = new Size(1411, 80);
            pagination1.TabIndex = 1;
            pagination1.TotalCount = 0;
            // pageHeader1
            pageHeader1.BackColor = Color.FromArgb(228, 231, 240);
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Location = new Point(0, 0);
            pageHeader1.Name = "pageHeader1";
            pageHeader1.Size = new Size(1479, 107);
            pageHeader1.TabIndex = 0;
            pageHeader1.Title = "식단 관리";
            // CafeteriaView
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Name = "CafeteriaView";
            Size = new Size(1479, 888);
            bodyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private PageHeader pageHeader1;
        private AppDataGrid grid;
        private Pagination pagination1;
    }
}