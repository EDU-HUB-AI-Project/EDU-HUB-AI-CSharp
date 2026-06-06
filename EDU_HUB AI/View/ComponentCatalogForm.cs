using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.View
{
    /// <summary>reference/component.html — WinForm 공통 컴포넌트 미리보기</summary>
    public class ComponentCatalogForm : Form
    {
        private readonly Navigation _nav = new();
        private readonly Panel _content = new();
        private readonly Label _pageTitle = new();
        private readonly Label _pageDesc = new();

        public ComponentCatalogForm()
        {
            Text = "EDU-HUB 공통 컴포넌트";
            Size = new Size(1100, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ThemeColors.Background;
            Font = ThemeFonts.Body;

            _nav.MenuSelected += (_, key) => ShowSection(key);

            _pageTitle.Font = ThemeFonts.PageTitle;
            _pageTitle.ForeColor = ThemeColors.Text;
            _pageTitle.AutoSize = true;
            _pageTitle.Text = "컴포넌트 카탈로그";

            _pageDesc.Font = ThemeFonts.PageDesc;
            _pageDesc.ForeColor = ThemeColors.TextMuted;
            _pageDesc.AutoSize = true;
            _pageDesc.Text = "reference/component.html 스펙 · 테이블 액션은 텍스트 링크";

            _content.Dock = DockStyle.Fill;
            _content.AutoScroll = true;
            _content.Padding = new Padding(24);

            var header = new Panel { Dock = DockStyle.Top, Height = 64, Padding = new Padding(24, 16, 24, 0), BackColor = ThemeColors.Surface };
            header.Controls.Add(_pageDesc);
            header.Controls.Add(_pageTitle);
            _pageDesc.Location = new Point(24, 36);
            _pageTitle.Location = new Point(24, 12);

            var main = new Panel { Dock = DockStyle.Fill };
            main.Controls.Add(_content);
            main.Controls.Add(header);

            Controls.Add(main);
            Controls.Add(_nav);

            BuildCatalog();
        }

        private void ShowSection(MenuKey key)
        {
            _pageTitle.Text = key switch
            {
                MenuKey.Dashboard => "대시보드",
                MenuKey.Trainees => "교육생 정보",
                MenuKey.Attendance => "출석 현황",
                MenuKey.Dormitory => "생활관 배정",
                MenuKey.Facilities => "시설 · 식당",
                MenuKey.Transport => "교통 정보",
                _ => "컴포넌트"
            };
            _pageDesc.Text = "좌측 메뉴는 Navigation UserControl · 데이터는 mock";
        }

        private void BuildCatalog()
        {
            _content.Controls.Clear();
            var y = 0;

            y = AddBlock("Title", "Page / Section / Panel / Field", y, p =>
            {
                p.Controls.Add(Typography.Create("출석 현황 관리", TitleLevel.Page));
                p.Controls.Add(Typography.Create("일자별 · 과정별 조회", TitleLevel.PageDesc));
                p.Controls.Add(Typography.Create("필터 조건", TitleLevel.Section));
                p.Controls.Add(Typography.Create("당일 출석 목록", TitleLevel.Panel));
                p.Controls.Add(Typography.Create("교육 과정", TitleLevel.Field));
            });

            y = AddBlock("Button", "Primary / Secondary / Ghost / Danger", y, p =>
            {
                var flow = new FlowLayoutPanel { AutoSize = true, WrapContents = true };
                flow.Controls.Add(ButtonStyles.Create("등록", ButtonVariant.Primary, icon: IconHelper.Get("plus", 16, Color.White)));
                flow.Controls.Add(ButtonStyles.Create("Excel 업로드", ButtonVariant.Secondary));
                flow.Controls.Add(ButtonStyles.Create("취소", ButtonVariant.Ghost));
                flow.Controls.Add(ButtonStyles.Create("삭제", ButtonVariant.Danger));
                flow.Controls.Add(ButtonStyles.Create("저장", ButtonVariant.Primary, small: true));
                p.Controls.Add(flow);
            });

            y = AddBlock("Table", "액션 열 — 텍스트 링크 「수정」「삭제」", y, p =>
            {
                var wrap = new Panel { Height = 220, Dock = DockStyle.Top };
                var grid = new AppDataGrid { Dock = DockStyle.Fill, Height = 170 };
                grid.Columns.Add("name", "이름");
                grid.Columns.Add("course", "과정");
                grid.Columns.Add("status", "상태");
                grid.AddTextActionColumns();
                grid.Rows.Add("김민수", "AI 실무", "출석");
                grid.Rows.Add("이지은", "클라우드 기초", "지각");
                grid.ActionClicked += (_, e) =>
                    MessageBox.Show($"행 {e.RowIndex + 1} · {e.Action}");

                var pagination = new Pagination { Dock = DockStyle.Bottom };
                wrap.Controls.Add(pagination);
                wrap.Controls.Add(grid);
                p.Controls.Add(wrap);
            });

            y = AddBlock("Tag", "상태 pill", y, p =>
            {
                var flow = new FlowLayoutPanel { AutoSize = true };
                flow.Controls.Add(StatusTag.Create("출석", TagVariant.Ok));
                flow.Controls.Add(StatusTag.Create("지각", TagVariant.Warn));
                flow.Controls.Add(StatusTag.Create("결석", TagVariant.Danger));
                flow.Controls.Add(StatusTag.Create("조퇴", TagVariant.Info));
                p.Controls.Add(flow);
            });

            y = AddBlock("Tab", "Button + Panel 전환", y, p =>
            {
                var tabs = new TabStrip { Height = 120, Dock = DockStyle.Top };
                tabs.AddTab("meal", "구내식당", new Label { Text = "구내식당 · 식단표", AutoSize = true, Padding = new Padding(8) });
                tabs.AddTab("facility", "시설 위치", new Label { Text = "시설 위치 탭", AutoSize = true, Padding = new Padding(8) });
                tabs.AddTab("traffic", "교통", new Label { Text = "교통 정보 탭", AutoSize = true, Padding = new Padding(8) });
                p.Controls.Add(tabs);
            });

            y = AddBlock("MealTable", "주간 5일 · mock 데이터", y, p =>
            {
                p.Controls.Add(new MealWeekTable { Height = 280, Dock = DockStyle.Top });
            });

            _nav.MenuSelected += (_, _) => { };
        }

        private void InitializeComponent()
        {

        }

        private int AddBlock(string title, string desc, int y, Action<Panel> build)
        {
            var block = new Panel
            {
                Location = new Point(0, y),
                Width = _content.ClientSize.Width - 48,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 0, 0, 24)
            };

            var h = Typography.Create(title, TitleLevel.Section);
            h.Location = new Point(0, 0);
            var d = Typography.Create(desc, TitleLevel.PageDesc);
            d.Location = new Point(0, 24);

            var body = new Panel
            {
                Location = new Point(0, 48),
                Width = block.Width,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(16),
                BorderStyle = BorderStyle.FixedSingle
            };

            build(body);
            block.Controls.Add(h);
            block.Controls.Add(d);
            block.Controls.Add(body);
            _content.Controls.Add(block);

            return y + block.Height + 16;
        }
    }
}
