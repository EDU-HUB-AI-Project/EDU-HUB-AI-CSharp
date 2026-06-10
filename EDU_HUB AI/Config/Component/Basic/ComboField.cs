using EDU_HUB_AI.Config.Theme;
using System.ComponentModel;
using System.Reflection;
namespace EDU_HUB_AI.Config.Component.Basic
{
    [ToolboxItem(true)]
    public partial class ComboField : UserControl
    {
        private const int InputHeight = 40;
        private const int LabelGap = 6;

        private readonly Label _label;
        private readonly Panel _shell;
        private readonly ComboBox _combo;

        private bool _focused;

        public ComboField()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = ThemeColors.Surface;
            Padding = new Padding(0);

            _label = BuildLabel();
            _combo = BuildCombo();
            _shell = BuildShell(_combo);

            Controls.Add(BuildStack(_label, _shell));
            Size = new Size(160, 54);
        }

        // ── 속성 ──────────────────────────────────────────

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

        [Browsable(false)] public object? DataSource { get => _combo.DataSource; set => _combo.DataSource = value; }
        [Browsable(false)] public string DisplayMember { get => _combo.DisplayMember; set => _combo.DisplayMember = value; }
        [Browsable(false)] public string ValueMember { get => _combo.ValueMember; set => _combo.ValueMember = value; }
        [Browsable(false)] public object? SelectedValue { get => _combo.SelectedValue; set => _combo.SelectedValue = value; }
        [Browsable(false)] public int SelectedIndex { get => _combo.SelectedIndex; set => _combo.SelectedIndex = value; }

        public new event EventHandler? SelectedIndexChanged;

        // ── Builder ──────────────────────────────────────────
        private static Label BuildLabel() => new()
        {
            AutoSize = true,
            Font = ThemeFonts.FieldLabel,
            ForeColor = ThemeColors.TextMuted,
            BackColor = ThemeColors.Surface,
            Margin = new Padding(0)
        };

        private ComboBox BuildCombo()
        {
            var combo = new ComboBox
            {
                FlatStyle = FlatStyle.Flat,
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.Text,
                BackColor = ThemeColors.Surface,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(1, 1),
                Height = InputHeight - 2
            };
            combo.Enter += (_, _) => SetFocused(true);
            combo.Leave += (_, _) => SetFocused(false);
            combo.SelectedIndexChanged += (_, _) => SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            return combo;
        }

        private Panel BuildShell(ComboBox combo)
        {
            var shell = new Panel
            {
                Height = InputHeight,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0),
                TabStop = false
            };
            EnableDoubleBuffer(shell);
            shell.Paint += (_, e) => PaintBorder(e, shell);
            shell.Resize += (_, _) => combo.Width = Math.Max(0, shell.Width - 2);
            shell.Controls.Add(combo);
            return shell;
        }

        private static TableLayoutPanel BuildStack(Label label, Panel shell)
        {
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
            stack.Controls.Add(label, 0, 0);
            stack.Controls.Add(shell, 0, 1);
            return stack;
        }

        // ── 내부 ──────────────────────────────────────────
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
            _combo.Width = Math.Max(0, _shell.Width - 2);
        }

        private void PaintBorder(PaintEventArgs e, Panel shell)
        {
            var bounds = new Rectangle(0, 0, shell.Width - 1, shell.Height - 1);
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
