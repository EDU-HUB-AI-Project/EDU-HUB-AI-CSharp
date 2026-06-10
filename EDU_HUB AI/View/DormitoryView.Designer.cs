using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;
using Microsoft.VisualBasic;
using NPOI.HSSF.Util;
using Org.BouncyCastle.Asn1.Cmp;

namespace EDU_HUB_AI.View
{
    partial class DormitoryView
    {
       
        private System.ComponentModel.IContainer components = null;

      
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            bodyPanel = new Panel();
            //grid = new AppDataGrid();
            //actionPanel = new ActionBar();
            //btnCreate = new AppButton();
            //btnImport = new AppButton();
            //btnExport = new AppButton();
            //btnSearch = new AppButton();
            pageHeader1 = new PageHeader();
            bodyPanel.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            //actionPanel.SuspendLayout();
            SuspendLayout();

            // 
            // bodyPanel
            // 
            //bodyPanel.Controls.Add(grid);
            //bodyPanel.Controls.Add(actionPanel);
            //bodyPanel.Controls.Add(pagination1);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Location = new Point(0, 85);
            bodyPanel.Margin = new Padding(4);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Padding = new Padding(31, 32, 31, 32);
            bodyPanel.Size = new Size(1479, 803);
            bodyPanel.TabIndex = 1;
            // 
            // grid
            // 
            //grid.AllowUserToAddRows = false;
            //grid.AllowUserToDeleteRows = false;
            //grid.AllowUserToResizeRows = false;
            //dataGridViewCellStyle4.BackColor = Color.FromArgb(250, 251, 252);
            //grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            //grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //grid.BackgroundColor = Color.White;
            //grid.BorderStyle = BorderStyle.None;
            //grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            //dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridViewCellStyle5.BackColor = Color.FromArgb(241, 245, 249);
            //dataGridViewCellStyle5.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            //dataGridViewCellStyle5.ForeColor = Color.FromArgb(15, 23, 42);
            //dataGridViewCellStyle5.Padding = new Padding(8, 0, 8, 0);
            //dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(241, 245, 249);
            //grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            //grid.ColumnHeadersHeight = 36;
            //grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridViewCellStyle6.BackColor = Color.White;
            //dataGridViewCellStyle6.Font = new Font("맑은 고딕", 9F);
            //dataGridViewCellStyle6.ForeColor = Color.FromArgb(15, 23, 42);
            //dataGridViewCellStyle6.Padding = new Padding(8, 0, 8, 0);
            //dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(248, 250, 252);
            //dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(15, 23, 42);
            //dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            //grid.DefaultCellStyle = dataGridViewCellStyle6;
            //grid.Dock = DockStyle.Fill;
            //grid.EnableHeadersVisualStyles = false;
            //grid.Font = new Font("맑은 고딕", 9F);
            //grid.GridColor = Color.FromArgb(226, 232, 240);
            //grid.Location = new Point(31, 122);
            //grid.Margin = new Padding(4);
            //grid.Name = "grid";
            //grid.ReadOnly = true;
            //grid.RowHeadersVisible = false;
            //grid.RowHeadersWidth = 51;
            //grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //grid.Size = new Size(1417, 585);
            //grid.TabIndex = 0;
            // 
            // actionPanel
            // 
            //actionPanel.BackColor = Color.FromArgb(244, 246, 249);
            //actionPanel.Controls.Add(btnCreate);
            //actionPanel.Controls.Add(btnImport);
            //actionPanel.Controls.Add(btnExport);
            //actionPanel.Controls.Add(btnSearch);
            //actionPanel.Dock = DockStyle.Top;
            //actionPanel.FlowDirection = FlowDirection.RightToLeft;
            //actionPanel.Location = new Point(31, 32);
            //actionPanel.Margin = new Padding(0);
            //actionPanel.Name = "actionPanel";
            //actionPanel.Padding = new Padding(0, 5, 0, 11);
            //actionPanel.Size = new Size(1417, 90);
            //actionPanel.TabIndex = 2;
            //actionPanel.WrapContents = false;
            // 
            // btnCreate
            // 
            //btnCreate.AutoSize = true;
            //btnCreate.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            //btnCreate.BackColor = Color.FromArgb(37, 99, 235);
            //btnCreate.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            //btnCreate.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
            //btnCreate.FlatStyle = FlatStyle.Flat;
            //btnCreate.Font = new Font("맑은 고딕", 9F);
            //btnCreate.ForeColor = Color.White;
            //btnCreate.Location = new Point(1266, 5);
            //btnCreate.Margin = new Padding(0);
            //btnCreate.Name = "btnCreate";
            //btnCreate.Padding = new Padding(15, 8, 15, 8);
            //btnCreate.Size = new Size(151, 48);
            //btnCreate.TabIndex = 0;
            //btnCreate.Tag = ButtonVariant.Primary;
            //btnCreate.Text = "출석 현황 추가";
            //btnCreate.UseVisualStyleBackColor = false;
            // 
            // btnImport
            // 
            //btnImport.AutoSize = true;
            //btnImport.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            //btnImport.BackColor = Color.White;
            //btnImport.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            //btnImport.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
            //btnImport.FlatStyle = FlatStyle.Flat;
            //btnImport.Font = new Font("맑은 고딕", 9F);
            //btnImport.ForeColor = Color.FromArgb(37, 99, 235);
            //btnImport.Location = new Point(1111, 5);
            //btnImport.Margin = new Padding(0, 0, 10, 0);
            //btnImport.Name = "btnImport";
            //btnImport.Padding = new Padding(12, 6, 12, 6);
            //btnImport.Size = new Size(145, 44);
            //btnImport.TabIndex = 1;
            //btnImport.Tag = ButtonVariant.Secondary;
            //btnImport.Text = "엑셀 일괄 등록";
            //btnImport.UseVisualStyleBackColor = false;
            //btnImport.Variant = ButtonVariant.Secondary;
            // 
            // btnExport
            // 
            //btnExport.AutoSize = true;
            //btnExport.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            //btnExport.BackColor = Color.White;
            //btnExport.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            //btnExport.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
            //btnExport.FlatStyle = FlatStyle.Flat;
            //btnExport.Font = new Font("맑은 고딕", 9F);
            //btnExport.ForeColor = Color.FromArgb(37, 99, 235);
            //btnExport.Location = new Point(953, 8);
            //btnExport.Name = "btnExport";
            //btnExport.Padding = new Padding(12, 6, 12, 6);
            //btnExport.Size = new Size(155, 44);
            //btnExport.TabIndex = 2;
            //btnExport.Tag = ButtonVariant.Secondary;
            //btnExport.Text = "엑셀로 내보내기";
            //btnExport.UseVisualStyleBackColor = false;
            //btnExport.Variant = ButtonVariant.Secondary;
            //// 
            // btnSearch
            // 
            //btnSearch.AutoSize = true;
            //btnSearch.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            //btnSearch.BackColor = Color.White;
            //btnSearch.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            //btnSearch.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
            //btnSearch.FlatStyle = FlatStyle.Flat;
            //btnSearch.Font = new Font("맑은 고딕", 9F);
            //btnSearch.ForeColor = Color.FromArgb(37, 99, 235);
            //btnSearch.Location = new Point(842, 8);
            //btnSearch.Name = "btnSearch";
            //btnSearch.Padding = new Padding(12, 6, 12, 6);
            //btnSearch.Size = new Size(105, 44);
            //btnSearch.TabIndex = 3;
            //btnSearch.Tag = ButtonVariant.Secondary;
            //btnSearch.Text = "조회하기";
            //btnSearch.UseVisualStyleBackColor = false;
            //btnSearch.Variant = ButtonVariant.Secondary;
            // 
            // pagination1
            // 
            //pagination1.BackColor = Color.FromArgb(244, 246, 249);
            //pagination1.Dock = DockStyle.Bottom;
            //pagination1.Location = new Point(31, 707);
            //pagination1.Margin = new Padding(5);
            //pagination1.Name = "pagination1";
            //pagination1.PageIndex = 1;
            //pagination1.PageSize = 10;
            //pagination1.Size = new Size(1417, 64);
            //pagination1.TabIndex = 1;
            //pagination1.TotalCount = 0;
            // 
            // pageHeader1
            // 
            pageHeader1.BackColor = Color.FromArgb(228, 231, 240);
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Location = new Point(0, 0);
            pageHeader1.Margin = new Padding(5);
            pageHeader1.Name = "pageHeader1";
            pageHeader1.Size = new Size(1479, 85);
            pageHeader1.TabIndex = 0;
            pageHeader1.Title = "생활관 배정 현황";
            // 
            // DormitoryView
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Name = "DormitoryView";
            Size = new Size(1479, 888);
            bodyPanel.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)grid).EndInit();
            //actionPanel.ResumeLayout(false);
            //actionPanel.PerformLayout();
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private PageHeader pageHeader1;
        //private AppDataGrid grid;
        //private Pagination pagination1;
        //private ActionBar actionPanel;
        //private AppButton btnImport;
        //private AppButton btnCreate;
        //private AppButton btnExport;
        //private AppButton btnSearch;

    }
}
