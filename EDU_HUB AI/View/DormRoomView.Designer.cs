using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Asn1.Cmp;

namespace EDU_HUB_AI.View
{
    partial class DormRoomView
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
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
            bodyPanel = new Panel();
            grid = new AppDataGrid();
            actionPanel = new Panel();
            tableCard = new Panel();
            filterCard = new Panel();
            gapPanel = new Panel();
            btnSearch = new AppButton();
            cmbDormRoom = new ComboField();
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

            // ================ grid ====================
            grid.Dock = DockStyle.Fill;
            grid.TabIndex = 1;

            // =============== cmbDormRoom ===============
            cmbDormRoom.FieldLabel = "층";
            cmbDormRoom.Size = new Size(100, 54);
            cmbDormRoom.Margin = new Padding(0, 0, 8, 0);
            cmbDormRoom.TabIndex = 0;

            // =============== actionPanel ===============
            var pnlButtonFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(12, 8, 0, 8)
            };
            pnlButtonFlow.Controls.Add(cmbDormRoom);
            actionPanel.Dock = DockStyle.Fill;
            actionPanel.Controls.Add(pnlButtonFlow);
            actionPanel.Padding = new Padding(0);

            // =============== pagination1 =============== 
            pagination1.BackColor = ThemeColors.Background;
            pagination1.Dock = DockStyle.Bottom;
            pagination1.Size = new Size(1411, 52);
            pagination1.TotalCount = 0;

            // =============== pageHeader1 ===============
            pageHeader1.BackColor = ThemeColors.HeaderBg;
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Size = new Size(1479, 100);
            pageHeader1.Title = "생활관 관리";

            // =============== DormRoomView ===============
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
        private Pagination pagination1;
        private AppDataGrid grid;
        private Panel actionPanel;
        private Panel filterCard;
        private Panel tableCard;
        private Panel gapPanel;
        private AppButton btnSearch;
        private ComboField cmbDormRoom;
    }
}
