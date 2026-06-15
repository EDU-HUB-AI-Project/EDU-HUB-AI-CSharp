using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Basic
{
    public partial class KpiCard : UserControl
    {

        private readonly Panel _topBar;
        private readonly Label _lblTitle;
        private readonly Label _lblValue;
        private readonly Label _lblSubtitle;

        public string Title
        {
            get => _lblTitle.Text;
            set => _lblTitle.Text = value;
        }

        public string Value
        {
            get => _lblValue.Text;
            set => _lblValue.Text = value;
        }

        public string Subtitle
        {
            get => _lblSubtitle.Text;
            set
            {
                _lblSubtitle.Text = value;
                _lblSubtitle.Visible = !string.IsNullOrEmpty(value);
            }
        }

        public Color Accent
        {
            get => _topBar.BackColor;
            set
            {
                _topBar.BackColor = value;
                _lblValue.ForeColor = value;
            }
        }

        public KpiCard()
        {
            DoubleBuffered = true;
            BackColor = ThemeColors.Surface;
            Size = new Size(160, 76);

            _topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 3,
                BackColor = ThemeColors.Primary
            };

            var content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 8, 14, 6),
                BackColor = Color.Transparent
            };

            _lblTitle = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 24,
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.TextMuted,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            var valueTlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            valueTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            valueTlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // 값
            valueTlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // subtitle (없으면 0px)

            _lblValue = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                Font = ThemeFonts.KpiValue,
                ForeColor = ThemeColors.Primary,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            _lblSubtitle = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                Visible = false,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            valueTlp.Controls.Add(_lblValue, 0, 0);
            valueTlp.Controls.Add(_lblSubtitle, 0, 1);

            content.Controls.Add(valueTlp);   // index 0: DockFill
            content.Controls.Add(_lblTitle);  // index 1: DockTop

            Controls.Add(content);
            Controls.Add(_topBar);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var pen = new Pen(ThemeColors.Border);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }
}
