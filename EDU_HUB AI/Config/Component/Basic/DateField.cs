using EDU_HUB_AI.Config.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDU_HUB_AI.Config.Component.Basic
{
    [ToolboxItem(true)]
    public partial class DateField : UserControl
    {
        private const int InputHeight = 40;
        private const int LabelGap = 6;

        private readonly Label _label;
        private readonly Panel _shell;
        private readonly DateTimePicker _dtp;

        private bool _focused;

        public DateField()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = ThemeColors.Surface;
            Padding = new Padding(0);

            _label = new Label
            {
                AutoSize = true,
                Font = ThemeFonts.FieldLabel,
                ForeColor = ThemeColors.TextMuted,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0)
            };

            _dtp = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = ThemeFonts.Body,
                Location = new Point(1, 1)
            };

            _dtp.Enter += (_, _) => SetFocused(true);
            _dtp.Leave += (_, _) => SetFocused(false);
            _dtp.ValueChanged += (_, _) => ValueChanged?.Invoke(this, EventArgs.Empty);

            _shell = new Panel
            {
                Height = InputHeight,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0),
                TabStop = false
            };

            EnableDoubleBuffer(_shell);
            _shell.Paint += (_, e) => PaintBorder(e);
            _shell.Resize += (_, _) => _dtp.Width = Math.Max(0, _shell.Width - 2);
            _shell.Controls.Add(_dtp);

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
            stack.RowStyles.Add(new RowStyle(SizeType.Absolute, InputHeight));
            stack.Controls.Add(_label, 0, 0);
            stack.Controls.Add(_shell, 0, 1);

            Controls.Add(stack);
            Size = new Size(200, 54);
        }

        // ── 속성 ───────────────────────────
        [Category("EDU-HUB"), DefaultValue("")]
        public string FieldLabel
        {
            get => _label.Text;
            set
            {
                _label.Text = value ?? "";
                _label.Visible = !string.IsNullOrEmpty(_label.Text);
                UpdateHeight();
            }
        }

        [Browsable(false)]
        public DateTime Value
        {
            get => _dtp.Value;
            set => _dtp.Value = value;
        }

        [Category("EDU-HUB")]
        public DateTimePickerFormat Format
        {
            get => _dtp.Format;
            set => _dtp.Format = value;
        }

        [Category("EDU-HUB"), DefaultValue("")]
        public string CustomFormat
        {
            get => _dtp.CustomFormat;
            set => _dtp.CustomFormat = value;
        }

        [Category("EDU-HUB"), DefaultValue(false)]
        public bool ShowCheckBox
        {
            get => _dtp.ShowCheckBox;
            set => _dtp.ShowCheckBox = value;
        }

        [Category("EDU-HUB"), DefaultValue(true)]
        public bool Checked
        {
            get => _dtp.Checked;
            set => _dtp.Checked = value;
        }

        public event EventHandler? ValueChanged;

        // ── 날짜 헬퍼 ─────────────────────────────

        /// <summary>yyMMdd 문자열로 날짜 설정 (EduInfo/Subject 포맷)</summary>
        public void SetYyMMdd(string? value)
        {
            if (!string.IsNullOrEmpty(value) && value.Length == 6
                && DateTime.TryParseExact("20" + value, "yyyyMMdd", null,
                    System.Globalization.DateTimeStyles.None, out var dt))
                _dtp.Value = dt;
        }

        /// <summary>yyMMdd 포맷으로 반환</summary>
        public string ToYyMMdd() => _dtp.Value.ToString("yyMMdd");

        /// <summary>yyyy-MM-dd 포맷으로 반환</summary>
        public string ToDateString() => _dtp.Value.ToString("yyyy-MM-dd");

        // ── 내부 ─────────────────────────────
        private void SetFocused(bool focused)
        {
            if (_focused == focused) return;
            _focused = focused;
            _shell.Invalidate();
        }

        private void UpdateHeight()
        {
            var labelH = _label.Visible ? _label.PreferredHeight + LabelGap : 0;
            Height = labelH + InputHeight;
            MinimumSize = new Size(MinimumSize.Width, Height);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _dtp.Width = Math.Max(0, _shell.Width - 2);
        }

        private void PaintBorder(PaintEventArgs e)
        {
            var bounds = new Rectangle(0, 0, _shell.Width - 1, _shell.Height - 1);
            using var fill = new SolidBrush(ThemeColors.Surface);
            e.Graphics.FillRectangle(fill, bounds);
            using var pen = new Pen(_focused ? ThemeColors.Primary : ThemeColors.Border);
            e.Graphics.DrawRectangle(pen, bounds);
        }

        private static void EnableDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, [true]);
        }
    }
}
