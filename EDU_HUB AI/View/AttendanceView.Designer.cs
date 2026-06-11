using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;

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
            bodyPanel = new Panel();
            tableCard = new Panel();
            gapPanel = new Panel();
            filterCard = new Panel();
            grid = new AppDataGrid();
            actionPanel = new Panel();
            btnCreate = new AppButton();
            btnImport = new AppButton();
            btnExport = new AppButton();
            //btnSearch = new AppButton();
            txtSearch = new TextField();
            cmbStatus = new ComboField();
            cmbEdu = new ComboField();
            dtpDate = new DateField();
            pagination1 = new Pagination();
            pageHeader1 = new PageHeader();
            bodyPanel.SuspendLayout();
            tableCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();

            // =============== bodyPanel ===============
            bodyPanel.Controls.Add(tableCard);
            bodyPanel.Controls.Add(gapPanel);
            bodyPanel.Controls.Add(filterCard);
            bodyPanel.Controls.Add(pagination1);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Padding = new Padding(34, 20, 34, 24);
            bodyPanel.BackColor = ThemeColors.Background;

            // ===============gapPanel ===============
            gapPanel.Dock = DockStyle.Top;
            gapPanel.Height = 8;
            gapPanel.BackColor = ThemeColors.Background;

            // =============== tableCard ===============
            tableCard.Controls.Add(grid);
            tableCard.Dock = DockStyle.Fill;
            tableCard.BackColor = ThemeColors.Surface;
            tableCard.Padding = new Padding(1);

            // =============== filterCard ===============
            filterCard.Controls.Add(actionPanel);
            filterCard.Dock = DockStyle.Top;
            filterCard.BackColor = ThemeColors.Surface;
            filterCard.Padding = new Padding(16, 12, 16, 12);
            filterCard.Height = 120;

            // =============== grid ===================
            grid.Dock = DockStyle.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = ThemeColors.Surface;
            grid.GridColor = ThemeColors.TableBorder;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeight = 38;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ThemeColors.TableHeader,
                ForeColor = ThemeColors.Text,
                Font = ThemeFonts.TableHeader,
                SelectionBackColor = ThemeColors.TableHeader,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 8, 0)
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ThemeColors.Surface,
                ForeColor = ThemeColors.Text,
                Font = ThemeFonts.TableCell,
                SelectionBackColor = ThemeColors.TableHover,
                SelectionForeColor = ThemeColors.Text,
                Padding = new Padding(12, 0, 8, 0),
                WrapMode = DataGridViewTriState.False
            };
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ThemeColors.TableStripe
            };

            // ============= actionPanel =============
            var pnlFilterFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                //BackColor = ThemeColors.Surface,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            pnlFilterFlow.Controls.Add(cmbEdu);
            pnlFilterFlow.Controls.Add(cmbStatus);
            pnlFilterFlow.Controls.Add(txtSearch);
            pnlFilterFlow.Controls.Add(dtpDate);
            var pnlButtonFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                //BackColor = ThemeColors.Surface,
                Padding = new Padding(12, 8, 0, 8)
            };
            pnlButtonFlow.Controls.Add(btnCreate);
            pnlButtonFlow.Controls.Add(btnImport);
            pnlButtonFlow.Controls.Add(btnExport);
            //pnlButtonFlow.Controls.Add(btnSearch);
            actionPanel.Controls.Add(pnlFilterFlow);
            actionPanel.Controls.Add(pnlButtonFlow);
            actionPanel.Dock = DockStyle.Fill;
            //actionPanel.BackColor = ThemeColors.Surface;
            actionPanel.Padding = new Padding(0);

            // ============= btnCreate ================
            btnCreate.Tag = ButtonVariant.Primary;
            btnCreate.Text = "출석 현황 추가";
            btnCreate.IconName = "plus";
            btnCreate.Margin = new Padding(0, 0, 8, 0);

            // =============== btnImport ===============
            btnImport.Margin = new Padding(0, 0, 8, 0);
            btnImport.IconName = "import";
            btnImport.Text = "엑셀 일괄 등록";
            btnImport.Variant = ButtonVariant.Secondary;

            // =============== btnExport ===============
       
            btnExport.Margin = new Padding(0, 0, 8, 0);
            btnExport.IconName = "export";
            btnExport.Text = "엑셀로 내보내기";
            btnExport.Variant = ButtonVariant.Secondary;

            // =============== btnSearch ===============
            //btnSearch.Margin = new Padding(0, 0, 8, 0);
            //btnSearch.Name = "btnSearch";
            //btnSearch.Text = "조회하기";
            //btnSearch.Variant = ButtonVariant.Primary;

            // =============== txtSearch ==================
            txtSearch.FieldLabel = "검색";
            txtSearch.Placeholder = "이름입력하여 검색";
            txtSearch.Size = new Size(220, 54);
            txtSearch.Margin = new Padding(0, 0, 8, 0);

            // =============== dtpDate ====================
            dtpDate.FieldLabel = "대상일자";
            dtpDate.Margin = new Padding(0, 0, 8, 0);
            dtpDate.Size = new Size(200, 23);

            // =============== cmbStatus ==================
            cmbStatus.FieldLabel = "출석상태";
            cmbStatus.Size = new Size(100, 54);
            cmbStatus.Margin = new Padding (0, 0, 8, 0);

            // =============== cmbEdu =====================
            cmbEdu.FieldLabel = "교육과정";
            cmbStatus.Size = new Size(100, 54);
            cmbEdu.Margin = new Padding(0, 0, 8, 0);

            // =============== pagination1 ===============
            pagination1.BackColor = ThemeColors.Background;
            pagination1.Dock = DockStyle.Bottom;
            pagination1.Size = new Size(1411, 52);
            pagination1.TotalCount = 0;

            // =============== pageHeader1 ===============
            pageHeader1.BackColor = ThemeColors.HeaderBg;
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Size = new Size(1479, 100);
            pageHeader1.Title = "출석현황";

            // =============== AttendanceView ===============
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            BackColor = ThemeColors.Background;
            Size = new Size(1479, 888);

            tableCard.ResumeLayout(false);
            bodyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
            actionPanel.ResumeLayout();
        }

        private Panel bodyPanel;
        private PageHeader pageHeader1;
        private AppDataGrid grid;
        private Pagination pagination1;
        private Panel actionPanel;
        private Panel tableCard;
        private Panel filterCard;
        private Panel gapPanel;
        private AppButton btnImport;
        private AppButton btnCreate;
        private AppButton btnExport;
        //private AppButton btnSearch;
        private TextField txtSearch;
        private ComboField cmbStatus;
        private ComboField cmbEdu;
        private DateField dtpDate;
    }
}