namespace EDU_HUB_AI.Config.Theme
{
    /// <summary>reference eh-title-* 단계별 폰트</summary>
    public static class ThemeFonts
    {
        private static readonly FontFamily Base = CreateBase();

        private static FontFamily CreateBase()
        {
            try
            {
                return new FontFamily("맑은 고딕"); 
            }
            catch
            {
                return SystemFonts.MessageBoxFont.FontFamily;
            }
        }

        public static Font PageTitle => new(Base, 14f, FontStyle.Bold);
        public static Font PageDesc => new(Base, 9f, FontStyle.Regular);
        public static Font Section => new(Base, 12f, FontStyle.Bold);
        public static Font Panel => new(Base, 11f, FontStyle.Bold);
        public static Font FieldLabel => new(Base, 9f, FontStyle.Regular);
        public static Font Body => new(Base, 9f, FontStyle.Regular);
        public static Font BodySm => new(Base, 8.25f, FontStyle.Regular);
        public static Font Button => new(Base, 9f, FontStyle.Regular);
        public static Font ButtonSm => new(Base, 8.25f, FontStyle.Regular);
        public static Font NavItem => new(Base, 10.5f, FontStyle.Regular);
        public static Font NavGroup => new(Base, 8.5f, FontStyle.Regular);
        public static Font TableHeader => new(Base, 9f, FontStyle.Bold);
        public static Font TableCell => new(Base, 9f, FontStyle.Regular);
        public static Font Tag => new(Base, 8.25f, FontStyle.Regular);
    }
}
