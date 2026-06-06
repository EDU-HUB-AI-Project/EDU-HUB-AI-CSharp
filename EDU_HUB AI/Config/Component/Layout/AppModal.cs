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
        private const int CardWidth = 420;
        private const int ViewportMargin = 80;
        private const int HeaderHeight = 52;
        private const int FooterHeight = 60;
        private readonly Panel _overlay;
        private readonly Panel _card;
        private readonly Panel _body;
        private readonly Label _title;
        private readonly Button _btnClose;
        private readonly Panel _footer;
        private readonly AppButton _btnConfirm;
        private readonly AppButton _btnCancel;
        private Bitmap? _backdrop;

        public AppModal()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = ThemeColors.ModalScrim;
            Font = ThemeFonts.Body;
            KeyPreview = true;
            KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Cancel(); };

            _overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.ModalScrim,
                Cursor = Cursors.Default
            };
            EnableDoubleBuffer(_overlay);
            _overlay.Paint += PaintOverlay;
            _overlay.MouseDown += (_, _) => Cancel();

            _card = new Panel
            {
                Width = CardWidth,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0)
            };
            _card.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, _card.Width - 1, _card.Height - 1);
            };

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                Padding = new Padding(24, 16, 16, 0),
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
                BackColor = ThemeColors.Surface
            };
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.FlatAppearance.MouseOverBackColor = ThemeColors.Background;
            _btnClose.ForeColor = ThemeColors.TextMuted;
            _btnClose.Font = ThemeFonts.Body;
            _btnClose.Click += (_, _) => Cancel();
            header.Controls.Add(_title);
            header.Controls.Add(_btnClose);
            header.Resize += (_, _) =>
            {
                _title.Location = new Point(24, (header.Height - _title.Height) / 2);
                _btnClose.Location = new Point(header.Width - 16 - _btnClose.Width, (header.Height - _btnClose.Width) / 2);
            };

            _body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 8, 24, 8),
                AutoScroll = false,
                BackColor = ThemeColors.Surface
            };

            _footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(24, 12, 24, 16),
                BackColor = ThemeColors.Surface
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
            _footer.Controls.Add(_btnCancel);
            _footer.Controls.Add(_btnConfirm);
            _footer.Resize += (_, _) => LayoutFooter();

            _card.Controls.Add(_body);
            _card.Controls.Add(_footer);
            _card.Controls.Add(header);

            Controls.Add(_overlay);
            Controls.Add(_card);
            _card.BringToFront();

            Load += (_, _) =>
            {
                FitToOwner();
                CaptureBackdrop();
            };
            Resize += (_, _) => FitCardSize();
            FormClosed += (_, _) =>
            {
                _backdrop?.Dispose();
                _backdrop = null;
            };
        }

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
            FitToOwner();
            CaptureBackdrop();
            _overlay.Invalidate();
            FitCardSize();
            LayoutFooter();
        }

        private void FitToOwner()
        {
            if (Owner is not Form owner) return;
            Bounds = owner.RectangleToScreen(new Rectangle(Point.Empty, owner.ClientSize));
        }

        private void CaptureBackdrop()
        {
            _backdrop?.Dispose();
            _backdrop = null;
            if (Owner is not Form owner) return;

            var w = owner.ClientSize.Width;
            var h = owner.ClientSize.Height;
            if (w <= 0 || h <= 0) return;

            var snap = new Bitmap(w, h);
            owner.DrawToBitmap(snap, new Rectangle(0, 0, w, h));

            if (Width != w || Height != h)
            {
                _backdrop = new Bitmap(Width, Height);
                using var g = Graphics.FromImage(_backdrop);
                g.DrawImage(snap, 0, 0, Width, Height);
                snap.Dispose();
            }
            else
            {
                _backdrop = snap;
            }
        }

        private void PaintOverlay(object? sender, PaintEventArgs e)
        {
            var rect = _overlay.ClientRectangle;
            if (_backdrop != null)
                e.Graphics.DrawImage(_backdrop, rect);
            else
                e.Graphics.Clear(ThemeColors.ModalScrim);

            using var veil = new SolidBrush(ThemeColors.ModalVeil);
            e.Graphics.FillRectangle(veil, rect);
        }

        /// <summary>본문 높이에 맞춰 카드 크기 조정. 뷰포트-80px 이내면 스크롤 없음.</summary>
        protected void FitCardSize()
        {
            _body.SuspendLayout();
            _card.SuspendLayout();
            _body.PerformLayout();

            var contentHeight = MeasureBodyContentHeight();
            var desiredHeight = HeaderHeight + contentHeight + FooterHeight;
            var maxHeight = Math.Max(HeaderHeight + FooterHeight + 80, Height - ViewportMargin);

            if (desiredHeight <= maxHeight)
            {
                _card.Height = desiredHeight;
                _body.AutoScroll = false;
            }
            else
            {
                _card.Height = maxHeight;
                _body.AutoScroll = true;
            }

            _card.ResumeLayout(true);
            _body.ResumeLayout(true);
            _card.Location = new Point((Width - _card.Width) / 2, (Height - _card.Height) / 2);
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

        public void SetCardHeight(int height)
        {
            _card.Height = height;
            FitCardSize();
        }

        private void LayoutFooter()
        {
            _btnConfirm.Location = new Point(_footer.Width - 24 - _btnConfirm.Width, (_footer.Height - _btnConfirm.Height) / 2);
            _btnCancel.Location = new Point(_btnConfirm.Left - 8 - _btnCancel.Width, (_footer.Height - _btnCancel.Height) / 2);
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
