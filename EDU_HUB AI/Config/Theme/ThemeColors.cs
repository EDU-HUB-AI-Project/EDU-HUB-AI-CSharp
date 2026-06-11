namespace EDU_HUB_AI.Config.Theme
{
    /// <summary>reference/styles.css :root 토큰 — WinForm 공통 색상</summary>
    public static class ThemeColors
    {
        public static Color Background => Color.FromArgb(244, 246, 249);
        public static Color Surface => Color.White;
        public static Color HeaderBg => Color.FromArgb(228, 231, 240);
        public static Color BodyBg => Color.FromArgb(244, 246, 249);
        public static Color Sidebar => Color.FromArgb(43, 50, 66);
        public static Color SidebarHover => Color.FromArgb(48, 58, 78);
        public static Color SidebarGroupText => Color.FromArgb(128, 137, 151);
        public static Color SidebarText => Color.FromArgb(148, 163, 184);
        public static Color SidebarActiveBg => Color.FromArgb(77, 88, 112);
        public static Color Primary => Color.FromArgb(37, 99, 235);
        public static Color PrimaryHover => Color.FromArgb(29, 78, 216);
        public static Color Secondary => Color.FromArgb(37, 99, 235);
        public static Color SecondaryHover => Color.FromArgb(239, 246, 255);
        public static Color Sync => Color.FromArgb(129, 141, 178);        // #818DB2
        public static Color SyncHover => Color.FromArgb(110, 122, 158);
        public static Color Text => Color.FromArgb(15, 23, 42);
        public static Color TextMuted => Color.FromArgb(100, 116, 139);
        public static Color Border => Color.FromArgb(226, 232, 240);
        public static Color InputFocusRing => Color.FromArgb(64, 37, 99, 235);
        public static Color TableBorder => Color.FromArgb(226, 232, 240);
        public static Color Ok => Color.FromArgb(16, 185, 129);
        public static Color OkBg => Color.FromArgb(209, 250, 229);
        public static Color OkText => Color.FromArgb(4, 120, 87);
        public static Color Warn => Color.FromArgb(245, 158, 11);
        public static Color WarnBg => Color.FromArgb(254, 243, 199);
        public static Color WarnText => Color.FromArgb(180, 83, 9);
        public static Color Danger => Color.FromArgb(239, 68, 68);
        public static Color DangerBg => Color.FromArgb(254, 226, 226);
        public static Color DangerText => Color.FromArgb(185, 28, 28);
        public static Color InfoBg => Color.FromArgb(219, 234, 254);
        public static Color InfoText => Color.FromArgb(29, 78, 216);
        public static Color TableHeader => Color.FromArgb(241, 245, 249);
        public static Color TableHover => Color.FromArgb(248, 250, 252);
        public static Color TableStripe => Color.FromArgb(250, 251, 252);
        public static Color Link => Color.FromArgb(37, 99, 235);
        public static Color LinkDanger => Color.FromArgb(185, 28, 28);
        // 모달 스크림 — 캡처 실패 시 단색 배경 (#F8FAFC)
        public static Color ModalScrim => Color.FromArgb(248, 250, 252);
        // 모달 베일 — Paint 전용, 캡처 위에 덮는 연한 흰색 (alpha 200 ≈ 78%)
        public static Color ModalVeil => Color.FromArgb(120, 15, 23, 42);

        public static Color SidebarTextHover => Color.FromArgb(203, 213, 225);
        public static Color TableSelected => Color.FromArgb(179, 208, 252); 
        public static Color TableSelectedText => Color.FromArgb(15, 23, 42);
    }
}
