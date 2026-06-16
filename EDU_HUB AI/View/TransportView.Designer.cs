using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using NPOI.HSSF.Util;

namespace EDU_HUB_AI.View
{
    partial class TransportView
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
            grid = new AppDataGrid();
            btnCreate = new AppButton();
            rbTypeAll = new RadioButton();
            rbTypeKtx = new RadioButton();
            rbTypeSrt = new RadioButton();
            rbTypeExbus = new RadioButton();
            rbTypeAirport = new RadioButton();
            rbTypeShuttle = new RadioButton();
            pagination1 = new Pagination();
            pageHeader1 = new PageHeader();
            filterCard = new Panel();
            actionPanel = new Panel();
            pnlButtonHost = new Panel();
            bodyPanel.SuspendLayout();
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

            // ── gapPanel ───────────────────────────────────────
            gapPanel.Dock = DockStyle.Top;
            gapPanel.Height = 8;
            gapPanel.BackColor = ThemeColors.Background;

            // ── tableCard ──────────────────────────────────────
            tableCard.Controls.Add(grid);
            tableCard.Dock = DockStyle.Fill;
            tableCard.BackColor = ThemeColors.Surface;
            tableCard.Padding = new Padding(1);

            // ── filterCard ─────────────────────────────────────
            filterCard.Controls.Add(actionPanel);
            filterCard.Dock = DockStyle.Top;
            filterCard.BackColor = ThemeColors.Surface;
            filterCard.Padding = new Padding(16, 12, 16, 12);
            filterCard.Height = 120;

            // ── grid ───────────────────────────────────────────
            grid.Dock = DockStyle.Fill;
            grid.TabIndex = 3;

            // ── actionPanel ────────────────────────────────────
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
            pnlFilterFlow.Controls.Add(rbTypeKtx);
            pnlFilterFlow.Controls.Add(rbTypeSrt);
            pnlFilterFlow.Controls.Add(rbTypeExbus);
            pnlFilterFlow.Controls.Add(rbTypeAirport);
            pnlFilterFlow.Controls.Add(rbTypeShuttle);

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

            // ── rbTypeAll ──────────────────────────────────────
            rbTypeAll.Text = "전체";
            rbTypeAll.Font = ThemeFonts.Body;
            rbTypeAll.ForeColor = ThemeColors.Text;
            rbTypeAll.BackColor = ThemeColors.Surface;
            rbTypeAll.AutoSize = true;
            rbTypeAll.Checked = true;
            rbTypeAll.Margin = new Padding(0, 0, 16, 0);

            // ── rbTypeKtx ──────────────────────────────────────
            rbTypeKtx.Text = "KTX";
            rbTypeKtx.Font = ThemeFonts.Body;
            rbTypeKtx.ForeColor = ThemeColors.Text;
            rbTypeKtx.BackColor = ThemeColors.Surface;
            rbTypeKtx.AutoSize = true;
            rbTypeKtx.Margin = new Padding(0, 0, 16, 0);

            // ── rbTypeSrt ──────────────────────────────────────
            rbTypeSrt.Text = "SRT";
            rbTypeSrt.Font = ThemeFonts.Body;
            rbTypeSrt.ForeColor = ThemeColors.Text;
            rbTypeSrt.BackColor = ThemeColors.Surface;
            rbTypeSrt.AutoSize = true;
            rbTypeSrt.Margin = new Padding(0, 0, 16, 0);

            // ── rbTypeExbus ────────────────────────────────────
            rbTypeExbus.Text = "고속·시외버스";
            rbTypeExbus.Font = ThemeFonts.Body;
            rbTypeExbus.ForeColor = ThemeColors.Text;
            rbTypeExbus.BackColor = ThemeColors.Surface;
            rbTypeExbus.AutoSize = true;
            rbTypeExbus.Margin = new Padding(0, 0, 16, 0);

            // ── rbTypeAirport ──────────────────────────────────
            rbTypeAirport.Text = "공항";
            rbTypeAirport.Font = ThemeFonts.Body;
            rbTypeAirport.ForeColor = ThemeColors.Text;
            rbTypeAirport.BackColor = ThemeColors.Surface;
            rbTypeAirport.AutoSize = true;
            rbTypeAirport.Margin = new Padding(0, 0, 16, 0);

            // ── rbTypeShuttle ──────────────────────────────────
            rbTypeShuttle.Text = "셔틀버스";
            rbTypeShuttle.Font = ThemeFonts.Body;
            rbTypeShuttle.ForeColor = ThemeColors.Text;
            rbTypeShuttle.BackColor = ThemeColors.Surface;
            rbTypeShuttle.AutoSize = true;
            rbTypeShuttle.Margin = new Padding(0);

            // ── btnCreate ──────────────────────────────────────
            btnCreate.Text = "운행 등록";
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
            pageHeader1.Title = "교통 정보 관리";

            // ── TransportView ──────────────────────────────────
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = ThemeColors.Background;
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader1);
            Size = new Size(1479, 888);

            tableCard.ResumeLayout(false);
            bodyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }

        private Panel bodyPanel;
        private Panel tableCard;
        private Panel filterCard;
        private Panel gapPanel;
        private PageHeader pageHeader1;
        private AppDataGrid grid;
        private Pagination pagination1;
        private Panel actionPanel;
        private Panel pnlButtonHost;
        private AppButton btnCreate;
        private RadioButton rbTypeAll;
        private RadioButton rbTypeKtx;
        private RadioButton rbTypeSrt;
        private RadioButton rbTypeExbus;
        private RadioButton rbTypeAirport;
        private RadioButton rbTypeShuttle;
    }
}