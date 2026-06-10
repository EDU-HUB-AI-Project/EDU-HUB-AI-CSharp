using System.Drawing.Drawing2D;
using System.Reflection;
using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Layout
{
    /// <summary>
    /// 웹 스타일 모달 — 부모 화면 캡처 + 연한 스크림 + 중앙 카드.
    /// WinForms TransparencyKey 대신 스냅샷+Paint로 CSS opacity 느낌을 낸다.
    /// </summary>
    public class AppModal : Form
    {
        // ── 상수 ────────────────────────
        private const int CardWidth = 420;
        private const int ViewportMargin = 80;
        private const int HeaderHeight = 52;
        private const int FooterHeight = 60;
        private const int CardRadius = 12;

        // ── 컨트롤 ────────────────────────
        private readonly Panel _overlay;
        private readonly Panel _card;
        private readonly Panel _header;
        private readonly Panel _body;
        private readonly Panel _footer;
        private readonly Label _title;
        private readonly Button _btnClose;
        private readonly AppButton _btnConfirm;
        private readonly AppButton _btnCancel;

        // ── 상태 ────────────────────────
        private Bitmap? _backdrop;
        private bool _loaded;
        private System.Windows.Forms.Timer? _fadeTimer;

        public AppModal()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = ThemeColors.ModalScrim;
            Font = ThemeFonts.Body;
            KeyPreview = true;
            KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Cancel(); };

            // ── 오버레이 ───────────────────────────────
            _overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.ModalScrim,
            };
            EnableDoubleBuffer(_overlay);
            _overlay.Paint += PaintOverlay;
            _overlay.MouseDown += (_, _) => Cancel();

            // ── 카드 ───────────────────────────────
            _card = new Panel
            {
                Width = CardWidth,
                BackColor = ThemeColors.Surface
            };
            EnableDoubleBuffer(_card);
            _card.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, _card.Width - 1, _card.Height - 1), CardRadius);
                using var fill = new SolidBrush(ThemeColors.Surface);
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.FillPath(fill, path);
                e.Graphics.DrawPath(pen, path);
            };

            // ── Header ───────────────────────────────
            _header = new Panel
            {
                Dock = DockStyle.Top,
                Height = HeaderHeight,
                BackColor = ThemeColors.Surface
            };
            _title = new Label
            {
                AutoSize = true,
                Font = ThemeFonts.Section,
                ForeColor = ThemeColors.Text,
                BackColor = ThemeColors.Surface,
                Text = "제목"
            };
            _btnClose = new Button
            {
                Text = "✕",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(32, 32),
                Cursor = Cursors.Hand,
                TabStop = false,
                BackColor = ThemeColors.Surface,
                ForeColor = ThemeColors.TextMuted,
                Font = ThemeFonts.Body
            };

            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.FlatAppearance.MouseOverBackColor = ThemeColors.Background;
            _btnClose.Click += (_, _) => Cancel();

            _header.Controls.AddRange([_title, _btnClose]);
            _header.Paint += PaintHeader;
            _header.Resize += (_, _) => LayoutHeader();

            // ── Body ───────────────────────────────
            _body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 8, 24, 8),
                AutoScroll = false,
                BackColor = ThemeColors.Surface
            };

            // ── Footer ───────────────────────────────
            _footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = FooterHeight,
                BackColor = ThemeColors.Background
            };
            _footer.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawLine(pen, 0, 0, _footer.Width, 0);
            };

            _btnCancel = new AppButton { Text = "취소", Variant = ButtonVariant.Ghost, Small = true };
            _btnConfirm = new AppButton { Text = "저장", Variant = ButtonVariant.Primary, Small = true };

            _btnCancel.Click += (_, _) => Cancel();
            _btnConfirm.Click += (_, _) => OnConfirm();
            _footer.Controls.AddRange([_btnCancel, _btnConfirm]);
            _footer.Resize += (_, _) => LayoutFooter();

            // ── 조립 ──────────────────────────────
            _card.Controls.AddRange([_body, _footer, _header]);
            Controls.AddRange([_overlay, _card]);
            _card.BringToFront();

            // ── Event ──────────────────────────────
            Load += OnLoad;
            Resize += (_, _) => { if (_loaded) FitCardSize(); };
            FormClosed += (_, _) =>
            {
                _fadeTimer?.Stop();
                _fadeTimer?.Dispose();
                _backdrop?.Dispose();
                _backdrop = null;
            };
        }

        // ── Public Props ──────────────────────────────
        public string ModalTitle
        {
            get => _title.Text;
            set => _title.Text = value;
        }

        public string ConfirmText
        {
            get => _btnConfirm.Text;
            set => _btnConfirm.Text = value;
        }

        public ButtonVariant ConfirmVariant
        {
            get => _btnConfirm.Tag is ButtonVariant v ? v : ButtonVariant.Primary;
            set => ButtonStyles.Apply(_btnConfirm, value, small: true);
        }

        protected Panel Body => _body;

        // ── Override ──────────────────────────────
        protected virtual void OnConfirm()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected void Cancel()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _overlay.Invalidate();
        }

        // ── Layout ──────────────────────────────
        private void OnLoad(object? sender, EventArgs e)
        {
            SuspendLayout();
            Opacity = 0;
            FitToOwner();
            FitCardSize();
            LayoutHeader();
            LayoutFooter();
            CaptureBackdrop();
            _loaded = true;
            ResumeLayout(false);
            _overlay.Invalidate();
            StartFadeIn();
        }

        private void FitToOwner()
        {
            if (Owner is not Form owner) return;
            Bounds = owner.Bounds;
        }

        protected void FitCardSize()
        {
            _body.SuspendLayout();
            _card.SuspendLayout();
            _body.PerformLayout();

            var contentHeight = MeasureBodyContentHeight();
            var desiredHeight = HeaderHeight + contentHeight + FooterHeight;
            var maxHeight = Math.Max(HeaderHeight + FooterHeight + 80, Height - ViewportMargin);

            _card.Height = desiredHeight <= maxHeight ? desiredHeight : maxHeight;
            _body.AutoScroll = desiredHeight > maxHeight;

            _card.ResumeLayout(true);
            _body.ResumeLayout(true);
            _card.Location = new Point((Width - _card.Width) / 2, (Height - _card.Height) / 2);
            UpdateCardRegion();
            _overlay.Invalidate();
        }
        private int MeasureBodyContentHeight()
        {
            if (_body.Controls.Count == 0)
                return _body.Padding.Vertical + 24;

            var bottom = _body.Padding.Top;
            foreach (Control child in _body.Controls)
            {
                if (!child.Visible) continue;
                bottom = Math.Max(bottom, child.Bottom);
            }
            return bottom + _body.Padding.Bottom;
        }

        protected void SetCardWidth(int width)
        {
            _card.Width = width;
            FitCardSize();
        }

        private void LayoutHeader()
        {
            _title.Location = new Point(32, (_header.Height - _title.Height) / 2);
            _btnClose.Location = new Point(_header.Width - 16 - _btnClose.Width, (_header.Height - _btnClose.Height) / 2);
        }
        private void LayoutFooter()
        {
            _btnConfirm.Location = new Point(_footer.Width - 24 - _btnConfirm.Width, (_footer.Height - _btnConfirm.Height) / 2);
            _btnCancel.Location = new Point(_btnConfirm.Left - 8 - _btnCancel.Width, (_footer.Height - _btnCancel.Height) / 2);
        }


        // ── Rendering ──────────────────────────────
        private void CaptureBackdrop()
        {
            if (Owner is not Form owner) return;
            var bounds = owner.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var snap = new Bitmap(bounds.Width, bounds.Height);
            using var g = Graphics.FromImage(snap);
            g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);

            var old = _backdrop;
            _backdrop = snap;
            old?.Dispose();
        }

        private void PaintOverlay(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = _overlay.ClientRectangle;

            if(_backdrop != null)
            {
                g.DrawImage(_backdrop, rect);
            }
            else
            {
                g.Clear(ThemeColors.ModalScrim);
            }

            using var veil = new SolidBrush(ThemeColors.ModalVeil);
            g.FillRectangle(veil, rect);

            DrawCardShadow(g, new Rectangle(_card.Left, _card.Top, _card.Width, _card.Height));
        }

        private void PaintHeader(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var accent = new SolidBrush(ThemeColors.Primary);
            using var pen = new Pen(ThemeColors.Border);
            e.Graphics.FillRectangle(accent, 0, 14, 3, _header.Height - 28);
            e.Graphics.DrawLine(pen, 0, _header.Height - 1, _header.Width, _header.Height - 1);
        }

        private void DrawCardShadow(Graphics g, Rectangle cardRect)
        {
            const int offset = 4;
            const int layers = 6;

            for(int i = layers - 1; i >= 1; i--)
            {
                var alpha = (int)(35 * (1.0 - (double)i / layers));
                if (alpha <= 0) continue;
                var expand = i * 2;
                var shadowRect = new Rectangle(
                    cardRect.X - expand + offset,
                    cardRect.Y - expand + offset,
                    cardRect.Width + expand * 2,
                    cardRect.Height + expand * 2
                    );

                using var brush = new SolidBrush(Color.FromArgb(alpha, 15, 23, 42));
                using var path = RoundedRect(shadowRect, CardRadius + expand);
                g.FillPath(brush, path);
            }
        }

        // ── Animation ──────────────────────────────
        private void StartFadeIn()
        {
            _fadeTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _fadeTimer.Tick += (_, _) =>
            {
                Opacity = Math.Min(1.0, Opacity + 0.12);
                if (Opacity >= 1.0)
                {
                    _fadeTimer.Stop();
                    _fadeTimer.Dispose();
                    _fadeTimer = null;
                }
            };
            _fadeTimer.Start();
        }

        // ── Util ──────────────────────────────
        private void UpdateCardRegion()
        {
            using var path = RoundedRect(new Rectangle(0, 0, _card.Width, _card.Height), CardRadius);
            var old = _card.Region;
            _card.Region = new Region(path);
            old?.Dispose();
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
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
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, [true]
                );
        }
    }
}
