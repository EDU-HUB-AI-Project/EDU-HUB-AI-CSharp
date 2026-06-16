using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.View
{
    partial class FacilityLocationView
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
            filterCard = new Panel();
            actionPanel = new Panel();
            pnlButtonHost = new Panel();
            gapPanel = new Panel();
            tableCard = new Panel();
            grid = new AppDataGrid();
            rbTypeAll = new RadioButton();
            rbTypeInner = new RadioButton();
            rbTypeOuter = new RadioButton();
            btnCreate = new AppButton();
            pagination1 = new Pagination();
            pageHeader1 = new PageHeader();

            bodyPanel.SuspendLayout();
            filterCard.SuspendLayout();
            tableCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();

            // ── bodyPanel ──────────────────────────────────────
            bodyPanel.Controls.Add(tableCard);
            bodyPanel.Controls.Add(gapPanel);
            bodyPanel.Controls.Add(filterCard);
            bodyPanel.Controls.Add(pagination1);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Padding = new Padding(34, 20, 34, 24);
            bodyPanel.BackColor = ThemeColors.Background;

            // ── filterCard ─────────────────────────────────────
            filterCard.Controls.Add(actionPanel);
            filterCard.Dock = DockStyle.Top;
            filterCard.BackColor = ThemeColors.Surface;
            filterCard.Padding = new Padding(16, 12, 16, 12);
            filterCard.Height = 120;

            // ── actionPanel (TransportView / StudentView 동일 패턴) ──
            var pnlFilterFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(0, 32, 0, 32),
                Margin = new Padding(0)
            };
            pnlFilterFlow.Controls.Add(rbTypeAll);
            pnlFilterFlow.Controls.Add(rbTypeInner);
            pnlFilterFlow.Controls.Add(rbTypeOuter);

            pnlButtonHost.Controls.Add(btnCreate);
            pnlButtonHost.Dock = DockStyle.Right;
            pnlButtonHost.Padding = new Padding(12, 0, 4, 0);
            pnlButtonHost.Width = 140;
            pnlButtonHost.BackColor = ThemeColors.Surface;

            // Right를 먼저 추가해야 Dock 시 오른쪽 영역이 잘리지 않음
            actionPanel.Controls.Add(pnlButtonHost);
            actionPanel.Controls.Add(pnlFilterFlow);
            actionPanel.Dock = DockStyle.Fill;
            actionPanel.Padding = new Padding(0);

            // ── gapPanel ───────────────────────────────────────
            gapPanel.Dock = DockStyle.Top;
            gapPanel.Height = 8;
            gapPanel.BackColor = ThemeColors.Background;

            // ── tableCard ──────────────────────────────────────
            tableCard.Controls.Add(grid);
            tableCard.Dock = DockStyle.Fill;
            tableCard.BackColor = ThemeColors.Surface;
            tableCard.Padding = new Padding(1);

            // ── grid ───────────────────────────────────────────
            grid.Dock = DockStyle.Fill;
            grid.TabIndex = 0;

            // ── rbTypeAll ──────────────────────────────────────
            rbTypeAll.Text = "전체";
            rbTypeAll.Font = ThemeFonts.Body;
            rbTypeAll.ForeColor = ThemeColors.Text;
            rbTypeAll.BackColor = ThemeColors.Surface;
            rbTypeAll.AutoSize = true;
            rbTypeAll.Checked = true;
            rbTypeAll.Margin = new Padding(0, 0, 20, 0);

            // ── rbTypeInner ────────────────────────────────────
            rbTypeInner.Text = "내부";
            rbTypeInner.Font = ThemeFonts.Body;
            rbTypeInner.ForeColor = ThemeColors.Text;
            rbTypeInner.BackColor = ThemeColors.Surface;
            rbTypeInner.AutoSize = true;
            rbTypeInner.Margin = new Padding(0, 0, 20, 0);

            // ── rbTypeOuter ────────────────────────────────────
            rbTypeOuter.Text = "외부";
            rbTypeOuter.Font = ThemeFonts.Body;
            rbTypeOuter.ForeColor = ThemeColors.Text;
            rbTypeOuter.BackColor = ThemeColors.Surface;
            rbTypeOuter.AutoSize = true;
            rbTypeOuter.Margin = new Padding(0);

            // ── btnCreate ──────────────────────────────────────
            btnCreate.Text = "시설 등록";
            btnCreate.Variant = ButtonVariant.Primary;
            btnCreate.IconName = "plus";
            btnCreate.Anchor = AnchorStyles.None;
            btnCreate.Margin = new Padding(0);

            // ── pagination1 ────────────────────────────────────
            pagination1.BackColor = ThemeColors.Background;
            pagination1.Dock = DockStyle.Bottom;
            pagination1.Size = new Size(1411, 52);
            pagination1.TotalCount = 0;

            // ── pageHeader1 ────────────────────────────────────
            pageHeader1.BackColor = ThemeColors.HeaderBg;
            pageHeader1.Dock = DockStyle.Top;
            pageHeader1.Size = new Size(1479, 100);
            pageHeader1.Title = "시설 관리";

            // ── FacilityLocationView ───────────────────────────
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = ThemeColors.Background;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Size = new Size(1479, 888);

            tableCard.ResumeLayout(false);
            filterCard.ResumeLayout(false);
            bodyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private Panel filterCard;
        private Panel actionPanel;
        private Panel pnlButtonHost;
        private Panel gapPanel;
        private Panel tableCard;
        private PageHeader pageHeader1;
        private AppDataGrid grid;
        private Pagination pagination1;
        private RadioButton rbTypeAll;
        private RadioButton rbTypeInner;
        private RadioButton rbTypeOuter;
        private AppButton btnCreate;
    }
}
