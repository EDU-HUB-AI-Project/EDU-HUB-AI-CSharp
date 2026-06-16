using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Theme;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace EDU_HUB_AI.Config.Component.Layout
{
    public partial class Navigation : UserControl
    {
        private readonly Dictionary<MenuKey, Panel> _menuButtons = new();
        private readonly Dictionary<MenuKey, string> _iconNames = new();
        
        private MenuKey _activeKey = MenuKey.Dashboard;
        private MenuKey? _hoverKey;

        // 네비게이션 너비
        private const int NavWidth = 260;

        private static readonly Dictionary<MenuKey, string> _shortcuts = new()
            {
                { MenuKey.Dashboard,  "^1" },
                { MenuKey.Trainees,   "^2" },
                { MenuKey.Attendance, "^3" },
                { MenuKey.Dormitory,  "^4" },
                { MenuKey.EduInfo,    "^5" },
                { MenuKey.Subject,    "^6" },
                { MenuKey.Facilities, "^7" },
                { MenuKey.Cafeteria,  "^8" },
                { MenuKey.Transport,  "^9" },
            };
        public Navigation()
        {
            InitializeComponent();
            if(!DesignMode)
            {
                BuildNavigation();
            }
        }

        public MenuKey ActiveMenu
        {
            get => _activeKey;
            set => SetActiveMenu(value, raiseEvent: false);
        }

        public event EventHandler<MenuKey>? MenuSelected;

        private void BuildNavigation()
        {
            BackColor = ThemeColors.Sidebar;
            Width = NavWidth;
            Dock = DockStyle.Left;

            Controls.Clear();
            _menuButtons.Clear();
            _iconNames.Clear();

            var stack = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,          
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(0, 8, 0, 20),
                BackColor = ThemeColors.Sidebar
            };

            stack.Controls.Add(CreateBrand());
            AddGroup(stack, "메인", [(MenuKey.Dashboard, "대시보드", "layout-dashboard")]);
            AddGroup(stack, "교육생 관리",
            [
                (MenuKey.Trainees, "교육생 관리", "users"),
                (MenuKey.Attendance, "출석 현황", "clipboard-check"),
                (MenuKey.Dormitory, "생활관 배정", "bed-double"),
                (MenuKey.DormRoom, "생활관 관리", "dormitory-room"),
                (MenuKey.EduInfo, "교육과정 관리", "edu-info"),
                (MenuKey.Subject, "과목 관리", "subject")
            ]);
            AddGroup(stack, "콘텐츠",
            [
                (MenuKey.Classroom, "강의실 관리", "classroom"),
                (MenuKey.Facilities, "시설 관리", "facilities"),
                (MenuKey.Cafeteria, "구내식당 메뉴", "utensils"),
                (MenuKey.Transport, "교통정보 관리", "bus")
            ]);

            typeof(FlowLayoutPanel)
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(stack, true);

            Controls.Add(stack);
            SetActiveMenu(MenuKey.Dashboard, raiseEvent: false);
        }

        private Control CreateBrand()
        {
            const int leftPad = 20;
            const int logoHeight = 36;
            var panel = new Panel
            {
                Width = NavWidth,
                Height = 56,
                Margin = new Padding(0, 4, 0, 12),
                BackColor = ThemeColors.Sidebar
            };
            var logo = Properties.Resources.logoNav;
            panel.Paint += (_, e) =>
            {
                if (logo == null) return;
                float scale = (float)logoHeight / logo.Height;
                int w = (int)(logo.Width * scale);
                int y = (panel.Height - logoHeight) / 2;
                e.Graphics.DrawImage(logo, leftPad, y, w, logoHeight);
            };
            return panel;
        }

        private void AddGroup(FlowLayoutPanel parent, string label, (MenuKey key, string text, string icon)[] items)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Font = ThemeFonts.NavGroup,
                ForeColor = ThemeColors.SidebarGroupText,
                AutoSize = false,
                Width = NavWidth,
                Height = 32,
                Margin = new Padding(0, 8, 0, 2),
                Padding = new Padding(20, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            });

            foreach (var (key, text, icon) in items)
            {
                var pnl = CreateMenuButton(key, text, icon);
                parent.Controls.Add(pnl);
                _menuButtons[key] = pnl;
            }
        }

        private Panel CreateMenuButton(MenuKey key, string text, string iconName)
        {
            _iconNames[key] = iconName;

            var panel = new Panel
            {
                Width = NavWidth,
                Height = 44,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2),
                BackColor = ThemeColors.Sidebar,
            };

            panel.Paint += (_, e) => PaintNavItem(e.Graphics, panel, key, text);
            panel.MouseEnter += (_, _) => { _hoverKey = key; panel.Invalidate(); };
            panel.MouseLeave += (_, _) => { if (_hoverKey == key) { _hoverKey = null; panel.Invalidate(); } };
            panel.Click += (_, _) => SetActiveMenu(key, raiseEvent: true);

            return panel;
        }

        private void PaintNavItem(Graphics g, Panel panel, MenuKey key, string text)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            bool active = _activeKey == key;
            bool hover = _hoverKey == key && !active;

            // 기본 배경
            using (var bg = new SolidBrush(ThemeColors.Sidebar))
            {
                g.FillRectangle(bg, panel.ClientRectangle);
            }

            // Active / Hover
            if (active || hover)
            {
                var fillColor = active ? ThemeColors.SidebarActiveBg : ThemeColors.SidebarHover;
                using var brush = new SolidBrush(fillColor);
                FillRoundedRect(g, brush, new Rectangle(4, 3, panel.Width - 8, panel.Height - 6), 6);
            }

            // Active
            if(active)
            {
                using var barBrush = new SolidBrush(ThemeColors.Primary);
                FillRoundedRect(g, barBrush, new Rectangle(0, 10, 3, panel.Height - 20), 2);
            }

            // 아이콘
            var iconColor = active ? Color.White
              : hover ? ThemeColors.SidebarTextHover   
              : ThemeColors.SidebarText;
            var icon = IconHelper.Get(_iconNames[key], 18, iconColor);

            if(icon != null)
            {
                g.DrawImage(icon, 20, (panel.Height - 18) / 2, 18, 18);
            }

            // 텍스트
            var textColor = active ? Color.White
              : hover ? ThemeColors.SidebarTextHover   
              : ThemeColors.SidebarText;

            using var baseFont = ThemeFonts.NavItem;
            using var boldFont = active ? new Font(baseFont.FontFamily, baseFont.Size, FontStyle.Bold) : null;
            var font = boldFont ?? baseFont;

            using var textBrush = new SolidBrush(textColor);
            g.DrawString(text, font, textBrush,
                new RectangleF(48, 0, panel.Width - 92, panel.Height),
                new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });

            // 단축키 힌트
            if(_shortcuts.TryGetValue(key, out var hint))
            {
                using var hintFont = ThemeFonts.NavGroup;
                using var hintBrush = new SolidBrush(ThemeColors.SidebarGroupText);
                g.DrawString(hint, hintFont, hintBrush
                    , new RectangleF(panel.Width - 44, 0, 32, panel.Height)
                    , new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Far
                    });
            }
        }
        private static void FillRoundedRect(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            int d = radius * 2;
            using var path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseAllFigures();
            g.FillPath(brush, path);
        }
        private void SetActiveMenu(MenuKey key, bool raiseEvent)
        {
            _activeKey = key;
            foreach (var (_, panel) in _menuButtons)
                panel.Invalidate();

            if (raiseEvent)
                MenuSelected?.Invoke(this, key);
        }
    }
}
