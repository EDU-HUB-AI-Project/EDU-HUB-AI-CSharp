using System.ComponentModel;
using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Layout
{
    [ToolboxItem(true)]
    public partial class PageHeader : UserControl
    {
        private readonly System.Windows.Forms.Timer _clock = new();

        public PageHeader()
        {
            DoubleBuffered = true;
            InitializeComponent();
            ApplyTheme();
            btnSync.Click += (_, _) => SyncClicked?.Invoke(this, EventArgs.Empty);
            Resize += (_, _) => LayoutControls();
            _clock.Interval = 1000;
            _clock.Tick += (_, _) => UpdateClock();
            HandleCreated += (_, _) => { if (ShowClock) _clock.Start(); };
            HandleDestroyed += (_, _) => _clock.Stop();
        }

        // Designer.cs가 Height를 임의로 덮어쓰는 것을 차단
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, 80, specified);
        }

        [Category("EDU-HUB")]
        [DefaultValue("페이지 제목")]
        public string Title
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }

        [Category("EDU-HUB")]
        [DefaultValue(true)]
        public bool ShowClock
        {
            get => lblClock.Visible;
            set
            {
                lblClock.Visible = value;
                if (IsHandleCreated)
                {
                    if (value) _clock.Start();
                    else _clock.Stop();
                }
                LayoutControls();
            }
        }

        [Category("EDU-HUB")]
        [DefaultValue(true)]
        public bool ShowSyncButton
        {
            get => btnSync.Visible;
            set { btnSync.Visible = value; LayoutControls(); }
        }

        public event EventHandler? SyncClicked;

        private void ApplyTheme()
        {
            Height = 100;
            BackColor = ThemeColors.HeaderBg;
            lblTitle.Font = ThemeFonts.PageTitle;
            lblTitle.ForeColor = ThemeColors.Text;
            lblClock.Font = ThemeFonts.BodySm;
            lblClock.ForeColor = ThemeColors.TextMuted;
            ButtonStyles.Apply(btnSync, ButtonVariant.Primary, small: true);
            btnSync.BackColor = ThemeColors.Sync;
            btnSync.FlatAppearance.BorderColor = ThemeColors.Sync;
            btnSync.FlatAppearance.MouseOverBackColor = ThemeColors.SyncHover;
            Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            };
            UpdateClock();
        }

        private void UpdateClock()
        {
            lblClock.Text = DateTime.Now.ToString("yyyy. M. d.  HH:mm:ss");
            LayoutControls();
        }

        private void LayoutControls()
        {
            lblTitle.Location = new Point(24, (Height - lblTitle.Height) / 2);
            if (btnSync.Visible)
            {
                btnSync.Location = new Point(Width - 24 - btnSync.Width, (Height - btnSync.Height) / 2);
                lblClock.Location = new Point(
                    btnSync.Left - 16 - lblClock.Width,
                    (Height - lblClock.Height) / 2);
            }
            else
            {
                lblClock.Location = new Point(
                    Width - 24 - lblClock.Width,
                    (Height - lblClock.Height) / 2);
            }
        }
    }
}