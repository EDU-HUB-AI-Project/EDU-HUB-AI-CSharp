using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.View
{
    public partial class CafeteriaCreateView : UserControl
    {
        private readonly AdminCafeteriaController _controller = new AdminCafeteriaController();

        private DataGridView _gridBreakfast;
        private DataGridView _gridLunch;
        private DataGridView _gridDinner;

        private Panel _sectionBreakfast;
        private Panel _sectionLunch;
        private Panel _sectionDinner;

        private DateTimePicker _datePicker;
        private ComboBox _cmbMeal;

        // 각 식사 종류별 독립적인 날짜 리스트
        private List<DateTime> _datesBreakfast = new List<DateTime>();
        private List<DateTime> _datesLunch = new List<DateTime>();
        private List<DateTime> _datesDinner = new List<DateTime>();

        private static readonly string[] MealTypes = { "BREAKFAST", "LUNCH", "DINNER" };

        public event Action? OnBack;

        public CafeteriaCreateView()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = ThemeColors.Background,
                Padding = new Padding(16)
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = ThemeColors.Background,
                Margin = new Padding(0, 0, 0, 8)
            };

            var lblMealTop = new Label
            {
                Text = "식사",
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Margin = new Padding(0, 6, 4, 0)
            };

            _cmbMeal = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = ThemeFonts.Body,
                Width = 100,
                Margin = new Padding(0, 2, 16, 0)
            };
            _cmbMeal.Items.AddRange(new object[] { "조식", "중식", "석식" });
            _cmbMeal.SelectedIndex = 0;
            _cmbMeal.SelectedIndexChanged += OnMealChanged;

            var lblDate = new Label
            {
                Text = "시작일",
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Margin = new Padding(0, 6, 4, 0)
            };

            _datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Value = DateTime.Now,
                Font = ThemeFonts.Body,
                Width = 150
            };
            _datePicker.ValueChanged += OnDatePickerChanged;

            var btnAddDay = new AppButton
            {
                Text = "+ 하루 추가",
                Variant = ButtonVariant.Ghost,
                Small = true,
                Margin = new Padding(8, 2, 0, 0)
            };
            btnAddDay.Click += OnAddDay;

            topPanel.Controls.Add(lblMealTop);
            topPanel.Controls.Add(_cmbMeal);
            topPanel.Controls.Add(lblDate);
            topPanel.Controls.Add(_datePicker);
            topPanel.Controls.Add(btnAddDay);

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ThemeColors.Background
            };

            var gridPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = ThemeColors.Background
            };
            gridPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            gridPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            gridPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            gridPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _gridBreakfast = CreateMealGrid();
            _gridLunch = CreateMealGrid();
            _gridDinner = CreateMealGrid();

            _sectionBreakfast = CreateMealSection("조식", _gridBreakfast);
            _sectionLunch = CreateMealSection("중식", _gridLunch);
            _sectionDinner = CreateMealSection("석식", _gridDinner);

            gridPanel.Controls.Add(_sectionBreakfast, 0, 0);
            gridPanel.Controls.Add(_sectionLunch, 0, 1);
            gridPanel.Controls.Add(_sectionDinner, 0, 2);

            scrollPanel.Controls.Add(gridPanel);

            var bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                BackColor = ThemeColors.Background,
                Padding = new Padding(0, 8, 0, 0)
            };

            var btnSave = new AppButton
            {
                Text = "저장",
                Variant = ButtonVariant.Primary
            };
            btnSave.Click += OnSave;

            var btnCancel = new AppButton
            {
                Text = "취소",
                Variant = ButtonVariant.Ghost,
                Margin = new Padding(0, 0, 8, 0)
            };
            btnCancel.Click += (_, _) => OnBack?.Invoke();

            bottomPanel.Controls.Add(btnSave);
            bottomPanel.Controls.Add(btnCancel);

            mainPanel.Controls.Add(topPanel, 0, 0);
            mainPanel.Controls.Add(scrollPanel, 0, 1);
            mainPanel.Controls.Add(bottomPanel, 0, 2);

            Controls.Add(mainPanel);

            // 각 식사 종류별 초기 날짜 세팅
            InitDatesFor(_datesBreakfast);
            InitDatesFor(_datesLunch);
            InitDatesFor(_datesDinner);

            RebuildRows(_gridBreakfast, _datesBreakfast);
            RebuildRows(_gridLunch, _datesLunch);
            RebuildRows(_gridDinner, _datesDinner);

            UpdateGridHeight(_gridBreakfast, _datesBreakfast.Count);
            UpdateGridHeight(_gridLunch, _datesLunch.Count);
            UpdateGridHeight(_gridDinner, _datesDinner.Count);

            ShowSection(0);
        }

        // 현재 선택된 식사 종류의 날짜 리스트 반환
        private List<DateTime> GetCurrentDates()
        {
            return _cmbMeal.SelectedIndex switch
            {
                0 => _datesBreakfast,
                1 => _datesLunch,
                2 => _datesDinner,
                _ => _datesBreakfast
            };
        }

        // 현재 선택된 식사 종류의 그리드 반환
        private DataGridView GetCurrentGrid()
        {
            return _cmbMeal.SelectedIndex switch
            {
                0 => _gridBreakfast,
                1 => _gridLunch,
                2 => _gridDinner,
                _ => _gridBreakfast
            };
        }

        private void OnMealChanged(object? sender, EventArgs e)
        {
            // 드롭다운 전환 시 해당 식사 종류의 시작일을 DatePicker에 반영
            var dates = GetCurrentDates();
            if (dates.Count > 0)
            {
                _datePicker.ValueChanged -= OnDatePickerChanged;
                _datePicker.Value = dates[0];
                _datePicker.ValueChanged += OnDatePickerChanged;
            }

            ShowSection(_cmbMeal.SelectedIndex);
        }

        private void OnDatePickerChanged(object? sender, EventArgs e)
        {
            var dates = GetCurrentDates();
            var grid = GetCurrentGrid();

            InitDatesFor(dates);
            RebuildRows(grid, dates);
            UpdateGridHeight(grid, dates.Count);
        }

        private void OnAddDay(object? sender, EventArgs e)
        {
            var dates = GetCurrentDates();
            var grid = GetCurrentGrid();

            DateTime next = dates[dates.Count - 1].AddDays(1);
            while (next.DayOfWeek == DayOfWeek.Saturday || next.DayOfWeek == DayOfWeek.Sunday)
            {
                next = next.AddDays(1);
            }
            dates.Add(next);

            RebuildRows(grid, dates);
            UpdateGridHeight(grid, dates.Count);
        }

        private void ShowSection(int index)
        {
            _sectionBreakfast.Visible = index == 0;
            _sectionLunch.Visible = index == 1;
            _sectionDinner.Visible = index == 2;
        }

        private void InitDatesFor(List<DateTime> dates)
        {
            dates.Clear();
            DateTime start = _datePicker.Value.Date;

            while (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
            {
                start = start.AddDays(1);
            }

            dates.Add(start);
        }

        private void UpdateGridHeight(DataGridView grid, int rowCount)
        {
            grid.Height = 30 + (rowCount * 30);
        }

        private DataGridView CreateMealGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Top,
                AutoSize = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BackgroundColor = ThemeColors.Surface,
                BorderStyle = BorderStyle.Fixed3D,
                Font = ThemeFonts.Body,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                ScrollBars = ScrollBars.None
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "날짜",
                Name = "date",
                Width = 120,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ThemeColors.Background,
                    ForeColor = ThemeColors.TextMuted
                }
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "메뉴 (예: 돈까스, 밥, 국)",
                Name = "menu",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Padding = new Padding(4)
                }
            });

            return grid;
        }

        private Panel CreateMealSection(string label, DataGridView grid)
        {
            var section = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ThemeColors.Background,
                Margin = new Padding(0, 0, 0, 16)
            };

            var lblMeal = new Label
            {
                Text = label,
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.Text,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 4)
            };

            section.Controls.Add(grid);
            section.Controls.Add(lblMeal);
            return section;
        }

        private void RebuildRows(DataGridView grid, List<DateTime> dates)
        {
            var savedMenus = new Dictionary<string, string>();
            foreach (DataGridViewRow row in grid.Rows)
            {
                string date = row.Cells["date"].Value?.ToString() ?? "";
                string menu = row.Cells["menu"].Value?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(date))
                {
                    savedMenus[date] = menu;
                }
            }

            grid.Rows.Clear();

            foreach (var date in dates)
            {
                string dateStr = date.ToString("yyyy-MM-dd");
                string dayLabel = date.ToString("M/d") + "(" + GetDayOfWeek(date) + ")";
                string savedMenu = savedMenus.ContainsKey(dateStr) ? savedMenus[dateStr] : "";

                grid.Rows.Add(dayLabel, savedMenu);
                grid.Rows[grid.Rows.Count - 1].Tag = dateStr;
            }
        }

        private async void OnSave(object? sender, EventArgs e)
        {
            var result = BuildResult();

            if (result.Count == 0)
            {
                MessageBox.Show("입력된 식단이 없습니다.", "알림");
                return;
            }

            await _controller.SaveCafeteriaList(result);
            MessageBox.Show("저장되었습니다.", "완료");
            OnBack?.Invoke();
        }

        private List<CafeteriaDto> BuildResult()
        {
            var result = new List<CafeteriaDto>();

            var grids = new[] { _gridBreakfast, _gridLunch, _gridDinner };

            for (int m = 0; m < 3; m++)
            {
                var grid = grids[m];
                string mealType = MealTypes[m];

                foreach (DataGridViewRow row in grid.Rows)
                {
                    string date = row.Tag?.ToString() ?? "";
                    string menuText = row.Cells["menu"].Value?.ToString()?.Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(date)) continue;
                    if (string.IsNullOrWhiteSpace(menuText)) continue;

                    result.Add(new CafeteriaDto
                    {
                        mealDate = date,
                        mealType = mealType,
                        menu = TextToJson(menuText),
                        mealClosed = "N"
                    });
                }
            }

            return result;
        }

        private string GetDayOfWeek(DateTime date)
        {
            string[] days = { "일", "월", "화", "수", "목", "금", "토" };
            return days[(int)date.DayOfWeek];
        }

        private string TextToJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "[]";

            var quoted = new List<string>();
            foreach (var item in text.Split(','))
            {
                string trimmed = item.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    quoted.Add($"\"{trimmed}\"");
                }
            }
            return "[" + string.Join(", ", quoted) + "]";
        }
    }
}