using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;

namespace EDU_HUB_AI.View
{
    partial class TableTemplateForm
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
            navigation1 = new Navigation();
            panelMain = new Panel();
            bodyPanel = new Panel();
            grid = new AppDataGrid();
            actionPanel = new ActionBar();
            btnCreate = new AppButton();
            btnExcel = new AppButton();
            pagination1 = new Pagination();
            pageHeader1 = new PageHeader();
            panelMain.SuspendLayout();
            bodyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            actionPanel.SuspendLayout();
            SuspendLayout();
            // 
            // navigation1
            // 
            navigation1.ActiveMenu = Config.Component.Common.MenuKey.Dashboard;
            navigation1.BackColor = Color.FromArgb(43, 50, 66);
            navigation1.Dock = DockStyle.Left;
            navigation1.Location = new Point(0, 0);
            navigation1.Margin = new Padding(4, 4, 4, 4);
            navigation1.Name = "navigation1";
            navigation1.Size = new Size(309, 907);
            navigation1.TabIndex = 0;
            // 
            // panelMain
            // 
            panelMain.Controls.Add(bodyPanel);
            panelMain.Controls.Add(pageHeader1);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(309, 0);
            panelMain.Margin = new Padding(4, 4, 4, 4);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(1080, 907);
            panelMain.TabIndex = 1;
            // 
            // bodyPanel
            // 
            bodyPanel.Controls.Add(grid);
            bodyPanel.Controls.Add(actionPanel);
            bodyPanel.Controls.Add(pagination1);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Location = new Point(0, 85);
            bodyPanel.Margin = new Padding(4, 4, 4, 4);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Padding = new Padding(31, 32, 31, 32);
            bodyPanel.Size = new Size(1080, 822);
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
            grid.Location = new Point(31, 104);
            grid.Margin = new Padding(4, 4, 4, 4);
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.RowHeadersWidth = 51;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(1018, 622);
            grid.TabIndex = 0;
            // 
            // actionPanel
            // 
            actionPanel.BackColor = Color.FromArgb(244, 246, 249);
            actionPanel.Controls.Add(btnCreate);
            actionPanel.Controls.Add(btnExcel);
            actionPanel.Dock = DockStyle.Top;
            actionPanel.FlowDirection = FlowDirection.RightToLeft;
            actionPanel.Location = new Point(31, 32);
            actionPanel.Margin = new Padding(0);
            actionPanel.Name = "actionPanel";
            actionPanel.Padding = new Padding(0, 5, 0, 11);
            actionPanel.Size = new Size(1018, 72);
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
            btnCreate.Location = new Point(887, 5);
            btnCreate.Margin = new Padding(0);
            btnCreate.Name = "btnCreate";
            btnCreate.Padding = new Padding(15, 8, 15, 8);
            btnCreate.Size = new Size(131, 48);
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
            btnExcel.Location = new Point(726, 5);
            btnExcel.Margin = new Padding(0, 0, 10, 0);
            btnExcel.Name = "btnExcel";
            btnExcel.Padding = new Padding(15, 8, 15, 8);
            btnExcel.Size = new Size(151, 48);
            btnExcel.TabIndex = 1;
            btnExcel.Tag = ButtonVariant.Secondary;
            btnExcel.Text = "엑셀 일괄 등록";
            btnExcel.UseVisualStyleBackColor = false;
            btnExcel.Variant = ButtonVariant.Secondary;
            // 
            // pagination1
            // 
            pagination1.BackColor = Color.FromArgb(244, 246, 249);
            pagination1.Dock = DockStyle.Bottom;
            pagination1.Location = new Point(31, 726);
            pagination1.Margin = new Padding(5, 5, 5, 5);
            pagination1.Name = "pagination1";
            pagination1.PageIndex = 1;
            pagination1.PageSize = 10;
            pagination1.Size = new Size(1018, 64);
            pagination1.TabIndex = 1;
            pagination1.TotalCount = 0;
            // 
            // pageHeader1
            // 
            pageHeader1.BackColor = Color.FromArgb(228, 231, 240);
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Location = new Point(0, 0);
            pageHeader1.Margin = new Padding(5, 5, 5, 5);
            pageHeader1.Name = "pageHeader1";
            pageHeader1.Size = new Size(1080, 85);
            pageHeader1.TabIndex = 0;
            pageHeader1.Title = "교육생 관리";
            // 
            // TableTemplateForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1389, 907);
            Controls.Add(panelMain);
            Controls.Add(navigation1);
            Margin = new Padding(4, 4, 4, 4);
            Name = "TableTemplateForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EDU-HUB — 테이블 템플릿";
            panelMain.ResumeLayout(false);
            bodyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            actionPanel.ResumeLayout(false);
            actionPanel.PerformLayout();
            ResumeLayout(false);
        }

        private Navigation navigation1;
        private Panel panelMain;
        private Panel bodyPanel;
        private PageHeader pageHeader1;
        private AppDataGrid grid;
        private Pagination pagination1;
        private ActionBar actionPanel;
        private AppButton btnExcel;
        private AppButton btnCreate;
    }
}
