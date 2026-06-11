using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Theme;
using System.Drawing.Drawing2D;

namespace EDU_HUB_AI.Config.Component.Layout
{
    /// <summary>예/아니오 확인 모달 — 삭제 등 위험 작업 확인용.</summary>
    public class ConfirmModal : AppModal
    {
        public ConfirmModal(string title, string message, string confirmText = "삭제", ButtonVariant variant = ButtonVariant.Danger)
        {
            ModalTitle = title;
            ConfirmText = confirmText;
            ConfirmVariant = variant;

            bool isDanger = variant == ButtonVariant.Danger;
            Color iconBg = isDanger ? ThemeColors.DangerBg : ThemeColors.InfoBg;
            Color iconFg = isDanger ? ThemeColors.Danger : ThemeColors.Primary;
            string symbol = isDanger ? "!" : "✓";

            // ── 아이콘 원형 ──────────────────────────
            var icon = new Panel { Size = new Size(52, 52), BackColor = ThemeColors.Surface };
            icon.Paint += (_, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var bg = new SolidBrush(iconBg);
                using var fg = new SolidBrush(iconFg);
                using var fnt = ThemeFonts.IconLg;
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.FillEllipse(bg, 0, 0, 51, 51);
                g.DrawString(symbol, fnt, fg, new RectangleF(0, 0, 52, 52), sf);
            };

            var iconRow = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = ThemeColors.Surface };
            iconRow.Controls.Add(icon);
            iconRow.Resize += (_, _) => icon.Location = new Point((iconRow.Width - 52) / 2, 12);

            // ── 메시지 ──────────────────────────────
            var msg = new Label
            {
                Text = message,
                AutoSize = true,
                MaximumSize = new Size(340, 0),
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.TextMuted,
                BackColor = ThemeColors.Surface,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(0, 0, 0, 8),
            };

            var msgRow = new Panel { Dock = DockStyle.Top, AutoSize = true, BackColor = ThemeColors.Surface };
            msgRow.Controls.Add(msg);
            msgRow.Resize += (_, _) => msg.Location = new Point((msgRow.Width - msg.Width) / 2, 0);

            
            Body.Controls.Add(msgRow);
            Body.Controls.Add(iconRow);
        }

        public static bool Show(IWin32Window owner, string title, string message, string confirmText = "삭제", ButtonVariant variant = ButtonVariant.Danger)
        {
            using var modal = new ConfirmModal(title, message, confirmText, variant);
            return modal.ShowDialog(owner) == DialogResult.OK;
        }
    }
}
