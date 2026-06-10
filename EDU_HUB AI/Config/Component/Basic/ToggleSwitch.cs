using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Reflection;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Basic
{
    [ToolboxItem(true)]
    public partial class ToggleSwitch : UserControl
    {
        private const int TrackW = 50;
        private const int TrackH = 26;
        private const int ThumbPad = 3;
        private int ThumbSize => TrackH - ThumbPad * 2;

        private readonly Label _fieldLabel;
        private readonly Panel _track;
        private readonly Label _inlineLabel;
        private bool _checked;

        public event EventHandler? CheckedChanged;

        public ToggleSwitch()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = ThemeColors.Surface;
            Padding = new Padding(0);

            _fieldLabel = new Label
            {
                AutoSize = true,
                Font = ThemeFonts.FieldLabel,
                ForeColor = ThemeColors.TextMuted,
                BackColor = ThemeColors.Surface,
                Visible = false
            };

            _track = new Panel
            {
                Width = TrackW,
                Height = TrackH,
                Cursor = Cursors.Hand,
                BackColor = ThemeColors.Surface
            };
            EnableDoubleBuffer(_track);
            _track.Paint += PaintTrack;
            _track.Click += (_, _) => Toggle();

            _inlineLabel = new Label
            {
                AutoSize = true,
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.Text,
                BackColor = ThemeColors.Surface,
                Cursor = Cursors.Hand,
                Margin = new Padding(8, 4, 0, 0)
            };
            _inlineLabel.Click += (_, _) => Toggle();

            var row = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            row.Controls.Add(_track);
            row.Controls.Add(_inlineLabel);

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.Controls.Add(_fieldLabel, 0, 0);
            stack.Controls.Add(row, 0, 1);

            Controls.Add(stack);
            Size = new Size(200, 38);
        }

        [Category("EDU-HUB"), DefaultValue("")]
        public string FieldLabel
        {
            get => _fieldLabel.Text;
            set
            {
                _fieldLabel.Text = value ?? "";
                _fieldLabel.Visible = !string.IsNullOrEmpty(_fieldLabel.Text);
                UpdateHeight();
            }
        }

        [Category("EDU-HUB"), DefaultValue("")]
        public string InlineLabel
        {
            get => _inlineLabel.Text;
            set => _inlineLabel.Text = value ?? "";
        }

        [Category("EDU-HUB"), DefaultValue(false)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                _track.Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Toggle()
        {
            Checked = !_checked;
        }

        private void PaintTrack(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 트랙
            var trackRect = new Rectangle(0, 0, TrackW - 1, TrackH - 1);
            var trackColor = _checked ? ThemeColors.Primary : ThemeColors.Border;
            using var trackPath = RoundedPath(trackRect, TrackH / 2);
            using var trackBrush = new SolidBrush(trackColor);
            g.FillPath(trackBrush, trackPath);

            // 섬
            var thumbSize = ThumbSize;
            var thumbX = _checked ? TrackW - thumbSize - ThumbPad : ThumbPad;
            using var thumbBrush = new SolidBrush(Color.White);
            g.FillEllipse(thumbBrush, thumbX, ThumbPad, thumbSize, thumbSize);
        }

        private static GraphicsPath RoundedPath(Rectangle rect, int radius)
        {
            var d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void EnableDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, [true]);
        }

        private void UpdateHeight()
        {
            var labelH = _fieldLabel.Visible ? _fieldLabel.PreferredHeight + 6 : 0;
            Height = labelH + TrackH;
            MinimumSize = new Size(MinimumSize.Width, Height);
        }
    }
}