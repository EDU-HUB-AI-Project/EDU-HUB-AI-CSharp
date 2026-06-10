using EDU_HUB_AI.Config.Theme;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace EDU_HUB_AI.Config.Component.Layout
{
    [ToolboxItem(true)]
    public partial class CardControl : UserControl
    {
        private string _title = "";
        private string _value = "";
        private string _subText = "";
        private Color _accentColor = Color.Green;

        private string _linkText = "";
        private float _progressValue = 0f;

        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }
        public string Value
        {
            get => _value;
            set { _value = value; Invalidate(); }
        }
        public string SubText
        {
            get => _subText;
            set { _subText = value; Invalidate(); }
        }
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public string LinkText
        {
            get => _linkText;
            set
            {
                _linkText = value;
                Cursor = string.IsNullOrEmpty(value) ? Cursors.Default : Cursors.Hand;
                Invalidate();
            }
        }
        [DefaultValue(0f)]
        public float ProgressValue
        {
            get => _progressValue;
            set { _progressValue = Math.Clamp(value, 0f, 1f); Invalidate(); }
        }

        public CardControl()
        {
            DoubleBuffered = true;
            InitializeComponent();
            BackColor = ThemeColors.Surface;
            Size = new Size(300, 165);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            const int pad = 20;
            const int barH = 5;
            int barY = Height - barH;

            // 배경
            using (var bg = new SolidBrush(ThemeColors.Surface))
                g.FillRectangle(bg, ClientRectangle);

            // 테두리
            using (var border = new Pen(ThemeColors.Border))
                g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

            // 타이틀
            using var titleFont = ThemeFonts.FieldLabel;
            using (var titleBrush = new SolidBrush(ThemeColors.TextMuted))
                g.DrawString(_title, titleFont, titleBrush, pad, 16);

            // 큰 값
            using var bodyBase = ThemeFonts.Body;
            using var valueFont = new Font(bodyBase.FontFamily, 22f, FontStyle.Bold);
            using (var valueBrush = new SolidBrush(ThemeColors.Text))
                g.DrawString(_value, valueFont, valueBrush, pad, 36);

            // 서브텍스트 / 링크
            float valueH = g.MeasureString(_value, valueFont).Height;
            float subY = 36 + valueH + 2;
            using var bodySmFont = ThemeFonts.BodySm;
            using (var subBrush = new SolidBrush(ThemeColors.TextMuted))
                g.DrawString(_subText, bodySmFont, subBrush, pad, subY);

            if (!string.IsNullOrEmpty(_linkText))
            {
                SizeF linkSize = g.MeasureString(_linkText, bodySmFont);
                float linkX = Width - pad - linkSize.Width;
                float linkY = barY - linkSize.Height - 8;
                using (var linkBrush = new SolidBrush(ThemeColors.Link))
                    g.DrawString(_linkText, bodySmFont, linkBrush, linkX, linkY);
            }

            // 프로그레스바 배경 (전체 너비)
            using (var bgBar = new SolidBrush(ThemeColors.Border))
                g.FillRectangle(bgBar, new Rectangle(0, barY, Width, barH));

            // 프로그레스바 foreground
            int fgW = (int)(Width * _progressValue);
            if (fgW > 0)
            {
                using var fgBrush = new SolidBrush(_accentColor);
                g.FillRectangle(fgBrush, new Rectangle(0, barY, fgW, barH));
            }
        }
    }
}
