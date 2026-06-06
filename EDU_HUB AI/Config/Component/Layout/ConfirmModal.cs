using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Layout
{
    /// <summary>예/아니오 확인 모달 — 삭제 등 위험 작업 확인용.</summary>
    public class ConfirmModal : AppModal
    {
        public ConfirmModal(string title, string message, string confirmText = "삭제")
        {
            ModalTitle = title;
            ConfirmText = confirmText;
            ConfirmVariant = ButtonVariant.Danger;

            var messageLabel = new Label
            {
                Text = message,
                AutoSize = true,
                MaximumSize = new Size(372, 0),
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.Text,
                BackColor = ThemeColors.Surface,
                Dock = DockStyle.Top
            };
            Body.Controls.Add(messageLabel);
        }

        public static bool Show(IWin32Window owner, string title, string message, string confirmText = "삭제")
        {
            using var modal = new ConfirmModal(title, message, confirmText);
            return modal.ShowDialog(owner) == DialogResult.OK;
        }
    }
}
