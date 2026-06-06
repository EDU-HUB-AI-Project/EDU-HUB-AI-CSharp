using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Domain
{
    /// <summary>주간 5일 식단표 UI — mock 데이터, API 연동은 추후</summary>
    public class MealWeekTable : UserControl
    {
        private static readonly (string Key, string Label, string Sub)[] Rows =
        {
            ("breakfast", "조식", ""),
            ("lunchA", "중식", "A 코스"),
            ("lunchB", "중식", "B 코스"),
            ("dinner", "석식", "")
        };

        private static readonly Dictionary<string, string[]> Sample = new()
        {
            ["breakfast"] = ["쌀밥·된장찌개·김치", "우유·시리얼·빵", "쌀밥·미역국", "계란죽·김", "잔치국수"],
            ["lunchA"] = ["비빔밥·미역국", "제육볶음·밥", "돈까스·샐러드", "칼국수", "불고기덮밥"],
            ["lunchB"] = ["냉면·만두", "김치찌개·밥", "치킨가스·밥", "쫄면", "오므라이스"],
            ["dinner"] = ["잔치국수", "돈까스", "삼겹구이·밥", "라면·김밥", "비빔밥"]
        };

        private readonly Label _rangeLabel = new();
        private readonly DataGridView _grid = new();
        private DateTime _weekStart;

        public MealWeekTable()
        {
            _weekStart = DateHelper.GetMonday(DateTime.Today);
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Surface;
            BuildLayout();
            RenderWeek();
        }

        public DateTime WeekStart => _weekStart;

        private void BuildLayout()
        {
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 0, 0, 8) };

            _rangeLabel.Font = ThemeFonts.Panel;
            _rangeLabel.ForeColor = ThemeColors.Text;
            _rangeLabel.AutoSize = true;
            _rangeLabel.Location = new Point(0, 8);

            var sub = new Label
            {
                Text = "월~금 · 중식 A/B 코스",
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Location = new Point(0, 28)
            };

            var btnPrev = ButtonStyles.Create("이전 주", ButtonVariant.Ghost, small: true,
                IconHelper.Get("chevron-left", 14));
            var btnToday = ButtonStyles.Create("이번 주", ButtonVariant.Secondary, small: true,
                IconHelper.Get("calendar-days", 14));
            var btnNext = ButtonStyles.Create("다음 주", ButtonVariant.Ghost, small: true,
                IconHelper.Get("chevron-right", 14));
            var btnSave = ButtonStyles.Create("식단 저장", ButtonVariant.Primary, small: true,
                IconHelper.Get("save", 14, Color.White));

            btnPrev.Click += (_, _) => { _weekStart = DateHelper.AddDays(_weekStart, -7); RenderWeek(); };
            btnToday.Click += (_, _) => { _weekStart = DateHelper.GetMonday(DateTime.Today); RenderWeek(); };
            btnNext.Click += (_, _) => { _weekStart = DateHelper.AddDays(_weekStart, 7); RenderWeek(); };

            var btnFlow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                WrapContents = false
            };
            btnFlow.Controls.AddRange([btnPrev, btnToday, btnNext, btnSave]);

            toolbar.Controls.Add(_rangeLabel);
            toolbar.Controls.Add(sub);
            toolbar.Controls.Add(btnFlow);
            toolbar.Resize += (_, _) => btnFlow.Location = new Point(toolbar.Width - btnFlow.Width, 4);

            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.RowHeadersVisible = false;
            _grid.BorderStyle = BorderStyle.FixedSingle;
            _grid.BackgroundColor = ThemeColors.Surface;
            _grid.GridColor = ThemeColors.Border;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = ThemeColors.TableHeader;
            _grid.ColumnHeadersDefaultCellStyle.Font = ThemeFonts.TableHeader;
            _grid.DefaultCellStyle.Font = ThemeFonts.TableCell;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.RowTemplate.Height = 48;

            Controls.Add(_grid);
            Controls.Add(toolbar);
        }

        private void RenderWeek()
        {
            _rangeLabel.Text = DateHelper.FormatWeekRange(_weekStart);

            _grid.Columns.Clear();
            _grid.Rows.Clear();
            _grid.Columns.Add("meal", "구분");
            _grid.Columns[0].Width = 88;
            _grid.Columns[0].DefaultCellStyle.BackColor = ThemeColors.TableHeader;

            var dayNames = new[] { "월", "화", "수", "목", "금" };
            for (var i = 0; i < 5; i++)
            {
                var day = DateHelper.AddDays(_weekStart, i);
                _grid.Columns.Add($"d{i}", $"{dayNames[i]}\n{DateHelper.FormatShort(day)}");
            }

            foreach (var (key, label, sub) in Rows)
            {
                var rowLabel = string.IsNullOrEmpty(sub) ? label : $"{label}\n{sub}";
                var values = new List<object> { rowLabel };
                values.AddRange(Sample[key].Take(5).Cast<object>());
                _grid.Rows.Add(values.ToArray());
            }
        }
    }
}
