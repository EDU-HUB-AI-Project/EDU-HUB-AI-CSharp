using EDU_HUB_AI.Config.Theme;
using Svg;

namespace EDU_HUB_AI.Config.Component.Common
{
    /// <summary>Lucide SVG → Bitmap (NavBar·툴바 등). 테이블 액션은 텍스트 링크 사용.</summary>
    public static class IconHelper
    {
        private static readonly Dictionary<string, string> SvgByName = new(StringComparer.OrdinalIgnoreCase)
        {
            ["layout-dashboard"] = LucideRaw("""<rect width="7" height="9" x="3" y="3" rx="1"/><rect width="7" height="5" x="14" y="3" rx="1"/><rect width="7" height="9" x="14" y="12" rx="1"/><rect width="7" height="5" x="3" y="16" rx="1"/>"""),
            ["users"] = Lucide("M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2M9 11a4 4 0 100-8 4 4 0 000 8zM23 21v-2a4 4 0 00-3-3.87M16 3.13a4 4 0 010 7.75"),
            ["clipboard-check"] = Lucide("M16 4h2a2 2 0 012 2v14a2 2 0 01-2 2H6a2 2 0 01-2-2V6a2 2 0 012-2h2M9 2h6v4H9V2zM9 14l2 2 4-4"),
            ["utensils"] = Lucide("M3 2v7c0 1.1.9 2 2 2h0a2 2 0 002-2V2M7 2v20M21 15V2v0a5 5 0 00-5 5v6c0 1.1.9 2 2 2h3zM21 15v7"),
            ["bed-double"] = Lucide("M2 20v-8a2 2 0 012-2h16a2 2 0 012 2v8M4 10V6a2 2 0 012-2h12a2 2 0 012 2v4M12 4v6M2 18h20"),
            ["facilities"] = LucideRaw("""<path d="M10 12h4"/><path d="M10 8h4"/><path d="M14 21v-3a2 2 0 0 0-4 0v3"/><path d="M6 10H4a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V9a2 2 0 0 0-2-2h-2"/><path d="M6 21V5a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v16"/>"""),
            ["bus"] = LucideRaw("""<path d="M8 6v6"/><path d="M15 6v6"/><path d="M2 12h19.6"/><path d="M18 18h3s.5-1.7.8-2.8c.1-.4.2-.8.2-1.2 0-.4-.1-.8-.2-1.2l-1.4-5C20.1 6.8 19.1 6 18 6H4a2 2 0 0 0-2 2v10h3"/><circle cx="7" cy="18" r="2"/><path d="M9 18h5"/><circle cx="16" cy="18" r="2"/>"""),
            ["chevron-left"] = Lucide("M15 18l-6-6 6-6"),
            ["chevron-right"] = Lucide("M9 18l6-6-6-6"),
            ["calendar-days"] = Lucide("M8 2v4M16 2v4M3 10h18M5 4h14a2 2 0 012 2v14a2 2 0 01-2 2H5a2 2 0 01-2-2V6a2 2 0 012-2zM8 14h.01M12 14h.01M16 14h.01M8 18h.01M12 18h.01M16 18h.01"),
            ["save"] = Lucide("M19 21H5a2 2 0 01-2-2V5a2 2 0 012-2h11l5 5v11a2 2 0 01-2 2zM17 21v-8H7v8M7 3v5h8"),
            ["search"] = Lucide("M11 19a8 8 0 100-16 8 8 0 000 16zM21 21l-4.35-4.35"),
            ["plus"] = Lucide("M12 5v14M5 12h14"),
            ["calendar"] = Lucide("M8 2v4M16 2v4M3 10h18M5 4h14a2 2 0 012 2v14a2 2 0 01-2 2H5a2 2 0 01-2-2V6a2 2 0 012-2z"),
            ["dormitory-room"] = LucideRaw("""<path d="M6 22V4a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v18Z"/><path d="M6 12H4a2 2 0 0 0-2 2v6a2 2 0 0 0 2 2h2"/><path d="M18 9h2a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2h-2"/><path d="M10 6h4"/><path d="M10 10h4"/><path d="M10 14h4"/><path d="M10 18h4"/>"""),
            ["edu-info"] = LucideRaw("""<path d="M21.42 10.922a1 1 0 0 0-.019-1.838L12.83 5.18a2 2 0 0 0-1.66 0L2.6 9.08a1 1 0 0 0 0 1.832l8.57 3.908a2 2 0 0 0 1.66 0z"/><path d="M22 10v6"/><path d="M6 12.5V16a6 3 0 0 0 12 0v-3.5"/>"""),
            ["subject"] = LucideRaw("""<path d="M4 19.5v-15A2.5 2.5 0 0 1 6.5 2H19a1 1 0 0 1 1 1v18a1 1 0 0 1-1 1H6.5a1 1 0 0 1 0-5H20"/><path d="M8 11h8"/><path d="M8 7h6"/>"""),
            ["classroom"] = LucideRaw("""<path d="M2 3h20"/><path d="M21 3v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V3"/><path d="m7 21 5-5 5 5"/><path d="M12 16v5"/>"""),
        };

        private static readonly Dictionary<string, Image> Cache = new();

        public static Image? Get(string name, int size = 18, Color? color = null)
        {
            var stroke = color ?? ThemeColors.SidebarText;
            var key = $"{name}:{size}:{stroke.ToArgb()}";
            if (Cache.TryGetValue(key, out var cached))
                return cached;

            if (!SvgByName.TryGetValue(name, out var svgContent))
                return null;

            try
            {
                var colored = svgContent.Replace("currentColor", ColorTranslator.ToHtml(stroke));
                var svg = SvgDocument.FromSvg<SvgDocument>(colored);
                var bitmap = svg.Draw(size, size);
                Cache[key] = bitmap;
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private static string Lucide(string pathD) =>
            LucideRaw($"""<path d="{pathD}"/>""");

        private static string LucideRaw(string inner) =>
            $"""<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">{inner}</svg>""";
    }
}
