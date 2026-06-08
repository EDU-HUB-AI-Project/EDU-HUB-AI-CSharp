using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Layout
{
    public partial class Navigation : UserControl
    {
        private readonly Dictionary<MenuKey, Button> _menuButtons = new();
        private MenuKey _activeKey = MenuKey.Dashboard;

        public Navigation()
        {
            InitializeComponent();
            BuildNavigation();
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
            Width = 260;
            Dock = DockStyle.Left;

            Controls.Clear();
            _menuButtons.Clear();

            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ThemeColors.Sidebar
            };

            var stack = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(16, 22, 16, 22),
                BackColor = ThemeColors.Sidebar
            };

            stack.Controls.Add(CreateBrand());
            AddGroup(stack, "메인", [(MenuKey.Dashboard, "대시보드", "layout-dashboard")]);
            AddGroup(stack, "교육생 관리",
            [
                (MenuKey.Trainees, "교육생 정보", "users"),
                (MenuKey.Attendance, "출석 현황", "clipboard-check"),
                (MenuKey.Dormitory, "생활관 배정", "bed-double"),
                (MenuKey.EduInfo, "교육과정 관리", "eduInfo")
            ]);
            AddGroup(stack, "콘텐츠",
            [
                (MenuKey.Facilities, "시설 · 식당", "utensils"),
                (MenuKey.Transport, "교통 정보", "bus"),
                (MenuKey.Classroom, "강의실 관리", "classroom")
            ]);

            scroll.Controls.Add(stack);
            Controls.Add(scroll);
            SetActiveMenu(MenuKey.Dashboard, raiseEvent: false);
        }

        private Control CreateBrand()
        {
            var panel = new Panel
            {
                Width = 166,
                Height = 44,   // 이미지 크기
                Margin = new Padding(0, 8, 0, 12),
                BackColor = ThemeColors.Sidebar
            };
            var pic = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = ThemeColors.Sidebar,
                Image = Properties.Resources.logoNav
            };
            panel.Controls.Add(pic);
            return panel;
        }

        private void AddGroup(FlowLayoutPanel parent, string label, (MenuKey key, string text, string icon)[] items)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Font = ThemeFonts.NavGroup,
                ForeColor = ThemeColors.SidebarGroupText,
                Width = 206,
                Height = 38,
                Padding = new Padding(0, 20, 0, 0)
            });

            foreach (var (key, text, icon) in items)
            {
                var btn = CreateMenuButton(key, text, icon);
                parent.Controls.Add(btn);
                _menuButtons[key] = btn;
            }
        }

        private Button CreateMenuButton(MenuKey key, string text, string iconName)
        {
            var btn = new Button
            {
                Text = "" + text,
                Width = 206,
                Height = 49,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Tag = key,
                Font = ThemeFonts.NavItem,
                ForeColor = ThemeColors.SidebarText,
                BackColor = ThemeColors.Sidebar,
                Margin = new Padding(0, 2, 0, 2),
                Padding = new Padding(6, 0, 6, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ThemeColors.SidebarHover;

            var image = IconHelper.Get(iconName, 18, ThemeColors.SidebarText);
            if (image != null)
                btn.Image = PadRight(image, 4);

            btn.Click += (_, _) => SetActiveMenu(key, raiseEvent: true);
            return btn;
        }

        private static Image PadRight(Image src, int pad)
        {
            var bmp = new Bitmap(src.Width + pad, src.Height);
            using var g = Graphics.FromImage(bmp);
            g.DrawImage(src, 0, 0, src.Width, src.Height);
            return bmp;
        }

        private void SetActiveMenu(MenuKey key, bool raiseEvent)
        {
            _activeKey = key;
            foreach (var (menuKey, btn) in _menuButtons)
            {
                var active = menuKey == key;
                btn.ForeColor = active ? Color.White : ThemeColors.SidebarText;
                btn.BackColor = active ? ThemeColors.SidebarActiveBg : ThemeColors.Sidebar;
                btn.Font = active
                    ? new Font(ThemeFonts.NavItem.FontFamily, ThemeFonts.NavItem.Size, FontStyle.Bold)
                    : ThemeFonts.NavItem;
            }

            if (raiseEvent)
                MenuSelected?.Invoke(this, key);
        }
    }
}
