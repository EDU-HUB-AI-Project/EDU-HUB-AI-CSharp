using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Basic
{
    public enum TitleLevel
    {
        Page,
        PageDesc,
        Section,
        Panel,
        Field
    }

    public static class Typography
    {
        public static Label Create(string text, TitleLevel level)
        {
            var label = new Label
            {
                Text = text,
                AutoSize = true
            };

            switch (level)
            {
                case TitleLevel.Page:
                    label.Font = ThemeFonts.PageTitle;
                    label.ForeColor = ThemeColors.Text;
                    break;
                case TitleLevel.PageDesc:
                    label.Font = ThemeFonts.PageDesc;
                    label.ForeColor = ThemeColors.TextMuted;
                    break;
                case TitleLevel.Section:
                    label.Font = ThemeFonts.Section;
                    label.ForeColor = ThemeColors.Text;
                    break;
                case TitleLevel.Panel:
                    label.Font = ThemeFonts.Panel;
                    label.ForeColor = ThemeColors.Text;
                    break;
                case TitleLevel.Field:
                    label.Font = ThemeFonts.FieldLabel;
                    label.ForeColor = ThemeColors.TextMuted;
                    break;
            }

            return label;
        }
    }
}
