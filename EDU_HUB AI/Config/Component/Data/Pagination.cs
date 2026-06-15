using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Data
{
    public partial class Pagination : UserControl
    {
        private int _totalCount = 128;
        private int _pageIndex = 1;
        private int _pageSize = 15;

        public Pagination()
        {
            InitializeComponent();
            DoubleBuffered = true;
            EnableDoubleBuffer(flowPages);
            ApplyTheme();
            if (DesignTimeHelper.IsDesignMode(this))
                ShowDesignPreview();
            else
                UpdateDisplay();
        }

        public int TotalCount
        {
            get => _totalCount;
            set { _totalCount = Math.Max(0, value); if (!DesignTimeHelper.IsDesignMode(this)) UpdateDisplay(); }
        }

        public int PageIndex
        {
            get => _pageIndex;
            set { _pageIndex = Math.Max(1, value); if (!DesignTimeHelper.IsDesignMode(this)) UpdateDisplay(); }
        }

        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = Math.Max(1, value); if (!DesignTimeHelper.IsDesignMode(this)) UpdateDisplay(); }
        }

        public event EventHandler<int>? PageChanged;

        private int TotalPages => Math.Max(1, (int)Math.Ceiling(_totalCount / (double)_pageSize));

        private void ApplyTheme()
        {
            BackColor = ThemeColors.Surface;
            lblInfo.Font = ThemeFonts.BodySm;
            lblInfo.ForeColor = ThemeColors.TextMuted;
            flowPages.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            flowPages.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void ShowDesignPreview()
        {
            lblInfo.Text = "총 128건 · 1 / 13 페이지";
            ButtonStyles.Apply(btnPrev, ButtonVariant.Ghost, small: true);
            ButtonStyles.Apply(btnNext, ButtonVariant.Ghost, small: true);
            ButtonStyles.Apply(btnPage1, ButtonVariant.Primary, small: true);
            ButtonStyles.Apply(btnPage2, ButtonVariant.Ghost, small: true);
            ButtonStyles.Apply(btnPage3, ButtonVariant.Ghost, small: true);
            ButtonStyles.Apply(btnPageLast, ButtonVariant.Ghost, small: true);
        }

        private void UpdateDisplay()
        {
            SuspendLayout();
            flowPages.SuspendLayout();
            lblInfo.Text = $"총 {_totalCount}건 · {_pageIndex} / {TotalPages} 페이지";
            BuildPageButtons();
            flowPages.ResumeLayout(false);
            
            
            ResumeLayout(false);
            LayoutControls();
        }

        private void BuildPageButtons()
        {
            flowPages.Controls.Clear();
            flowPages.Controls.Add(btnPrev);
            btnPrev.Enabled = _pageIndex > 1;

            foreach (var page in VisiblePages())
            {
                if (page < 0)
                {
                    flowPages.Controls.Add(new Label
                    {
                        Text = "…",
                        AutoSize = false,
                        Width = 24,
                        Height = 32,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = ThemeColors.TextMuted,
                        Font = ThemeFonts.BodySm,
                        Padding = new Padding(0)
                    });
                    continue;
                }

                var isActive = page == _pageIndex;
                var btn = new Button
                {
                    Text = page.ToString(),
                    AutoSize = false,
                    Width = 32,
                    Height = 32,
                    FlatStyle = FlatStyle.Flat,
                    Font = ThemeFonts.ButtonSm,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(1, 0, 1, 0),
                    Padding = new Padding(0)
                };

                if(isActive)
                {
                    btn.BackColor = ThemeColors.Primary;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseOverBackColor = ThemeColors.PrimaryHover;
                }
                else
                {
                    btn.BackColor = ThemeColors.Surface;
                    btn.ForeColor = ThemeColors.Text;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseOverBackColor = ThemeColors.Background;
                }
                
                var captured = page;
                btn.Click += (_, _) => GoToPage(captured);
                flowPages.Controls.Add(btn);
            }

            flowPages.Controls.Add(btnNext);
            btnNext.Enabled = _pageIndex < TotalPages;

            if (!DesignTimeHelper.IsDesignMode(this))
                ApplyNavIcons();
        }

        private void ApplyNavIcons()
        {
            ButtonStyles.Apply(btnPrev, ButtonVariant.Ghost, small: true);
            ButtonStyles.Apply(btnNext, ButtonVariant.Ghost, small: true);
            btnPrev.Text = "";
            btnNext.Text = "";
            btnPrev.Padding = new Padding(0);
            btnNext.Padding = new Padding(0);
            btnPrev.ImageAlign = ContentAlignment.MiddleCenter;
            btnNext.ImageAlign = ContentAlignment.MiddleCenter;
            btnPrev.Image = IconHelper.Get("chevron-left", 16, ThemeColors.TextMuted);
            btnNext.Image = IconHelper.Get("chevron-right", 16, ThemeColors.TextMuted);
            btnPrev.Size = new Size(32, 32);
            btnNext.Size = new Size(32, 32);
            btnPrev.Margin = new Padding(0, 0, 6, 0);
            btnNext.Margin = new Padding(6, 0, 0, 0);
        }

        private bool _laying;

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            LayoutControls();
        }

        private void LayoutControls()
        {
            if (_laying) return;
            _laying = true;
            try
            {
                flowPages.PerformLayout();
                lblInfo.Location = new Point(12, (Height - lblInfo.Height) / 2);

                var refWidth = Width;
                var offset = 0;
                if (Parent != null)
                {
                    refWidth = Parent.ClientSize.Width - Parent.Padding.Horizontal;
                    offset = Parent.Padding.Left - Left;
                }
                var x = offset + (refWidth - flowPages.Width) / 2;
                flowPages.Location = new Point(x, (Height - flowPages.Height) / 2);
            }
            finally
            {
                _laying = false;
            }
        }

        private IEnumerable<int> VisiblePages()
        {
            var total = TotalPages;
            if (total <= 7)
            {
                for (var i = 1; i <= total; i++) yield return i;
                yield break;
            }

            var start = Math.Max(1, _pageIndex - 2);
            var end = Math.Min(total, _pageIndex + 2);

            if (end - start < 4)
            {
                if (start == 1) end = Math.Min(total, 5);
                else if (end == total) start = Math.Max(1, total - 4);
            }

            if (start > 1) { yield return 1; if (start > 2) yield return -1; }
            for (var i = start; i <= end; i++) yield return i;
            if (end < total) { if (end < total - 1) yield return -1; yield return total; }
        }

        private void GoToPage(int page)
        {
            var clamped = Math.Clamp(page, 1, TotalPages);
            if (clamped == _pageIndex) return;
            _pageIndex = clamped;
            UpdateDisplay();
            PageChanged?.Invoke(this, _pageIndex);
        }

        private void Pagination_Resize(object? sender, EventArgs e) => LayoutControls();
        private void btnPrev_Click(object? sender, EventArgs e) => GoToPage(_pageIndex - 1);
        private void btnNext_Click(object? sender, EventArgs e) => GoToPage(_pageIndex + 1);

        // ── 키보드 페이지 이동용 public 메서드 ──────────────
        public void GoToFirst() => GoToPage(1);
        public void GoToPrev() => GoToPage(_pageIndex - 1);
        public void GoToNext() => GoToPage(_pageIndex + 1);
        public void GoToLast() => GoToPage(TotalPages);

        // ── 더블버퍼 ──────────────
        private static void EnableDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, control, new object[] { true }
                );
        }

    }
}