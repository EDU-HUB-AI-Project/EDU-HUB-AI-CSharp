using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Layout
{
    /// <summary>비동기 작업 중 입력 차단 + 상태 표시 오버레이.</summary>
    public class LoadingOverlay : Form
    {
        private readonly Label _lblMessage;

        private LoadingOverlay(Form owner, string message)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(30, 30, 30);
            Opacity = 0.65;
            ShowInTaskbar = false;
            Owner = owner;

            Bounds = owner.Bounds;

            EventHandler moveHandler = (_, _) => { if (!IsDisposed) Bounds = Owner.Bounds; };
            EventHandler resizeHandler = (_, _) => { if (!IsDisposed) Bounds = Owner.Bounds; };

            owner.Move += moveHandler;
            owner.Resize += resizeHandler;

            FormClosed += (_, _) =>
            {
                owner.Move -= moveHandler;
                owner.Resize -= resizeHandler;
            };

            _lblMessage = new Label
            {
                Text = message,
                ForeColor = Color.White,
                Font = ThemeFonts.Section,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            Controls.Add(_lblMessage);
        }

        private LoadingOverlay(Control target, string message)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(30, 30, 30);
            Opacity = 0.65;
            ShowInTaskbar = false;

            var parentForm = target.FindForm();
            Owner = parentForm;

            void Reposition()
            {
                if(!IsDisposed)
                {
                    Bounds = target.RectangleToScreen(target.ClientRectangle);
                }
            }
            Reposition();

            EventHandler repositionHandler = (_, _) => Reposition();

            if(parentForm != null)
            {
                parentForm.Move += repositionHandler;
                parentForm.Resize += repositionHandler;
            }
            target.Resize += repositionHandler;

            FormClosed += (_, _) =>
            {
                if (parentForm != null)
                {
                    parentForm.Move -= repositionHandler;
                    parentForm.Resize -= repositionHandler;
                }
                target.Resize -= repositionHandler;
            };

            _lblMessage = new Label
            {
                Text = message,
                ForeColor = Color.White,
                Font = ThemeFonts.Section,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            Controls.Add(_lblMessage);
        }

        public void UpdateMessage(string message)
        {
            if(IsDisposed)
            {
                return;
            }
            if(InvokeRequired)
            {
                Invoke(() => _lblMessage.Text = message);
            }
            else
            {
                _lblMessage.Text = message;
            }
        }

        public static LoadingOverlay Create(Form owner, string message = "처리 중...")
        {
            var overlay = new LoadingOverlay(owner, message);
            overlay.Show(owner);
            Application.DoEvents();
            return overlay;
        }

        public static LoadingOverlay Create(Control target, string message = "처리 중...")
        {
            var overlay = new LoadingOverlay(target, message);
            overlay.Show(target.FindForm());
            Application.DoEvents();
            return overlay;
        }
    }
}
