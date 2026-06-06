using System.ComponentModel;
using System.Reflection;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Basic
{
    /// <summary>
    /// 라벨 + 스타일 입력 — reference .eh-field / .eh-input (높이 32, border #E2E8F0).
    /// </summary>
    [ToolboxItem(true)]
    public class TextField : UserControl
    {
        private const int InputHeight = 32;
        private const int LabelGap = 6;

        private readonly Label _label;
        private readonly Panel _inputShell;
        private readonly TextBox _input;

        private bool _focused;
        private bool _hasError;

        public TextField()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = ThemeColors.Surface;
            Padding = new Padding(0);
            MinimumSize = new Size(120, InputHeight + LabelGap + 16);

            _label = new Label
            {
                AutoSize = true,
                Font = ThemeFonts.FieldLabel,
                ForeColor = ThemeColors.TextMuted,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0)
            };

            _inputShell = new Panel
            {
                Height = InputHeight,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(12, 7, 12, 7),
                Margin = new Padding(0),
                Cursor = Cursors.IBeam,
                TabStop = false
            };
            EnableDoubleBuffer(_inputShell);
            _inputShell.Paint += PaintInputBorder;
            _inputShell.Click += (_, _) => _input.Focus();

            _input = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.Text,
                BackColor = ThemeColors.Surface,
                Dock = DockStyle.Fill
            };
            _input.Enter += (_, _) => SetFocused(true);
            _input.Leave += (_, _) => SetFocused(false);
            _input.TextChanged += (_, _) => TextChanged?.Invoke(this, EventArgs.Empty);

            _inputShell.Controls.Add(_input);

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
            stack.Controls.Add(_inputShell, 0, 1);

            Controls.Add(stack);
            Size = new Size(280, 54);
        }

        [Category("EDU-HUB")]
        [DefaultValue("")]
        [Description("필드 라벨 (비우면 숨김)")]
        public string FieldLabel
        {
            get => _label.Text;
            set
            {
                _label.Text = value ?? "";
                _label.Visible = _label.Text.Length > 0;
                UpdateHeight();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => _input.Text;
            set => _input.Text = value ?? "";
        }

        [Category("EDU-HUB")]
        [DefaultValue("")]
        public string Placeholder
        {
            get => _input.PlaceholderText;
            set => _input.PlaceholderText = value ?? "";
        }

        [Category("EDU-HUB")]
        [DefaultValue(false)]
        public bool HasError
        {
            get => _hasError;
            set
            {
                if (_hasError == value) return;
                _hasError = value;
                _inputShell.Invalidate();
            }
        }

        [Category("EDU-HUB")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => _input.ReadOnly;
            set
            {
                _input.ReadOnly = value;
                _inputShell.Cursor = value ? Cursors.Default : Cursors.IBeam;
            }
        }

        public new event EventHandler? TextChanged;

        private void SetFocused(bool focused)
        {
            if (_focused == focused) return;
            _focused = focused;
            _inputShell.Invalidate();
        }

        private void UpdateHeight()
        {
            var labelH = _label.Visible ? _label.PreferredHeight + LabelGap : 0;
            Height = labelH + InputHeight;
            MinimumSize = new Size(MinimumSize.Width, Height);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _label.Font = ThemeFonts.FieldLabel;
            _input.Font = ThemeFonts.Body;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _inputShell.Width = Width;
        }

        private void PaintInputBorder(object? sender, PaintEventArgs e)
        {
            var bounds = new Rectangle(0, 0, _inputShell.Width - 1, _inputShell.Height - 1);
            using var fill = new SolidBrush(ThemeColors.Surface);
            e.Graphics.FillRectangle(fill, bounds);

            var borderColor = _hasError ? ThemeColors.Danger
                : _focused ? ThemeColors.Primary
                : ThemeColors.Border;
            using var pen = new Pen(borderColor);
            e.Graphics.DrawRectangle(pen, bounds);
        }

        private static void EnableDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                control,
                [true]);
        }
    }
}
