using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Data
{
    public enum TagVariant
    {
        Ok,
        Warn,
        Danger,
        Info
    }

    public static class StatusTag
    {
        public static Label Create(string text, TagVariant variant)
        {
            var label = new Label
            {
                Text = text,
                AutoSize = true,
                Font = ThemeFonts.Tag,
                Padding = new Padding(8, 3, 8, 3),
                Margin = new Padding(0)
            };

            switch (variant)
            {
                case TagVariant.Ok:
                    label.BackColor = ThemeColors.OkBg;
                    label.ForeColor = ThemeColors.OkText;
                    break;
                case TagVariant.Warn:
                    label.BackColor = ThemeColors.WarnBg;
                    label.ForeColor = ThemeColors.WarnText;
                    break;
                case TagVariant.Danger:
                    label.BackColor = ThemeColors.DangerBg;
                    label.ForeColor = ThemeColors.DangerText;
                    break;
                case TagVariant.Info:
                    label.BackColor = ThemeColors.InfoBg;
                    label.ForeColor = ThemeColors.InfoText;
                    break;
            }

            return label;
        }

        public static TagVariant FromStatus(string status) => status switch
        {
            "출석" => TagVariant.Ok,
            "지각" => TagVariant.Warn,
            "결석" => TagVariant.Danger,
            "조퇴" => TagVariant.Info,
            _ => TagVariant.Ok
        };
    }
}
