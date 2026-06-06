using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Basic
{
    public enum ButtonVariant
    {
        Primary,
        Secondary,
        Ghost,
        Danger
    }

    public static class ButtonStyles
    {
        public static void Apply(Button button, ButtonVariant variant, bool small = false)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Font = small ? ThemeFonts.ButtonSm : ThemeFonts.Button;
            button.Cursor = Cursors.Hand;
            button.Padding = small ? new Padding(8, 4, 8, 4) : new Padding(12, 6, 12, 6);
            button.Tag = variant;

            switch (variant)
            {
                case ButtonVariant.Primary:
                    button.BackColor = ThemeColors.Primary;
                    button.FlatAppearance.BorderColor = ThemeColors.Primary;
                    button.ForeColor = Color.White;
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.MouseOverBackColor = ThemeColors.PrimaryHover;
                    break;
                case ButtonVariant.Secondary:
                    button.BackColor = ThemeColors.Surface;
                    button.ForeColor = ThemeColors.Secondary;
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = ThemeColors.Secondary;
                    button.FlatAppearance.MouseOverBackColor = ThemeColors.SecondaryHover;
                    break;
                case ButtonVariant.Ghost:
                    button.BackColor = ThemeColors.Surface;
                    button.ForeColor = ThemeColors.TextMuted;
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = ThemeColors.Border;
                    button.FlatAppearance.MouseOverBackColor = ThemeColors.Background;
                    break;
                case ButtonVariant.Danger:
                    button.BackColor = ThemeColors.DangerBg;
                    button.ForeColor = ThemeColors.DangerText;
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = ThemeColors.DangerBg;
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 202, 202);
                    break;
            }
        }

        public static Button Create(string text, ButtonVariant variant, bool small = false, Image? icon = null)
        {
            var btn = new Button
            {
                Text = text,
                AutoSize = false,
                Height = small ? 28 : 32,
                TextImageRelation = icon != null ? TextImageRelation.ImageBeforeText : TextImageRelation.Overlay,
                Image = icon,
                ImageAlign = ContentAlignment.MiddleLeft
            };
            Apply(btn, variant, small);
            return btn;
        }
    }
}
