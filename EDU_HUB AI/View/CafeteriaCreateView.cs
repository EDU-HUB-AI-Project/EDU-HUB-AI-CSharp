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
        private AppButton _btnDeleteSelected;

        private List<DateTime> _datesBreakfast = new List<DateTime>();
        private List<DateTime> _datesLunch = new List<DateTime>();
        private List<DateTime> _datesDinner = new List<DateTime>();

        private static readonly string[] MealTypes = { "BREAKFAST", "LUNCH", "DINNER" };

        private readonly List<CafeteriaDto> _allDetail;

        public event Action? OnBack;

        public CafeteriaCreateView(List<CafeteriaDto> allDetail)
        {
            _allDetail = allDetail;
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

            _btnDeleteSelected = new AppButton
            {
                Text = "선택 삭제",
                Variant = ButtonVariant.Ghost,
                Small = true,
                Enabled = false,
                Margin = new Padding(8, 2, 0, 0)
            };
            _btnDeleteSelected.Click += OnDeleteSelected;

            topPanel.Controls.Add(lblMealTop);
            topPanel.Controls.Add(_cmbMeal);
            topPanel.Controls.Add(lblDate);
            topPanel.Controls.Add(_datePicker);
            topPanel.Controls.Add(btnAddDay);
            topPanel.Controls.Add(_btnDeleteSelected);

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
            ShowSection(0);
        }

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
        }

        private void OnAddDay(object? sender, EventArgs e)
        {
            var dates = GetCurrentDates();
            var grid = GetCurrentGrid();

            DateTime next = dates.Count == 0
                ? _datePicker.Value.Date
                : dates[dates.Count - 1].AddDays(1);

            while (next.DayOfWeek == DayOfWeek.Saturday || next.DayOfWeek == DayOfWeek.Sunday)
            {
                next = next.AddDays(1);
            }

            if (next > DateTime.Now.AddMonths(1))
            {
                MessageBox.Show("오늘로부터 1달 이내의 날짜만 추가할 수 있습니다.", "알림");
                return;
            }

            dates.Add(next);

            string dateStr = next.ToString("yyyy-MM-dd");
            string dayLabel = next.ToString("M/d") + "(" + GetDayOfWeek(next) + ")";
            grid.Rows.Add(false, dayLabel, "");
            grid.Rows[grid.Rows.Count - 1].Tag = dateStr;

            UpdateGridHeight(grid, dates.Count);

            grid.Parent?.PerformLayout();
            grid.Parent?.Parent?.PerformLayout();
        }

        private void OnDeleteSelected(object? sender, EventArgs e)
        { 
            var grid = GetCurrentGrid();
            var dates = GetCurrentDates();

            var toRemove = grid.Rows
                .Cast<DataGridViewRow>()
                .Where(r => Convert.ToBoolean(r.Cells["check"].Value))
                .ToList();

            if (toRemove.Count == 0) return;

            foreach (var row in toRemove)
            {
                string tag = row.Tag?.ToString() ?? "";
                dates.RemoveAll(d => d.ToString("yyyy-MM-dd") == tag);
                grid.Rows.Remove(row);
            }

            UpdateGridHeight(grid, dates.Count);
            grid.Parent?.PerformLayout();
            grid.Parent?.Parent?.PerformLayout();
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
            int headerHeight = grid.ColumnHeadersHeight;
            int rowHeight = rowCount > 0 ? grid.Rows[0].Height : 30;
            grid.Height = headerHeight + (rowCount * rowHeight);
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
                ScrollBars = ScrollBars.None,
                Height = 30,
                DefaultCellStyle = new DataGridViewCellStyle
                { 
                    SelectionBackColor = ThemeColors.Surface,
                    SelectionForeColor = ThemeColors.Text
                }
            };

            grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "✓",
                Name = "check",
                Width = 30,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

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

            grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (grid.IsCurrentCellDirty)
                    grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            grid.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex != grid.Columns["check"].Index) return;

                bool anyChecked = grid.Rows
                .Cast<DataGridViewRow>()
                .Any(r => Convert.ToBoolean(r.Cells["check"].Value));

                _btnDeleteSelected.Enabled = anyChecked;
            };

            grid.ColumnHeaderMouseClick += (s, e) =>
            {
                if (e.ColumnIndex != grid.Columns["check"].Index) return;

                bool allChecked = grid.Rows
                .Cast<DataGridViewRow>()
                .All(r => Convert.ToBoolean(r.Cells["check"].Value));

                grid.EndEdit();

                foreach (DataGridViewRow row in grid.Rows)
                {
                    row.Cells["check"].Value = !allChecked;
                }

                grid.RefreshEdit();
                _btnDeleteSelected.Enabled = !allChecked && grid.Rows.Count > 0;
            };

            return grid;
        }

        private Panel CreateMealSection(string label, DataGridView grid)
        {
            var section = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = ThemeColors.Background,
                Padding = new Padding(0, 0, 0, 16)
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

                grid.Rows.Add(false, dayLabel, savedMenu);
                grid.Rows[grid.Rows.Count - 1].Tag = dateStr;
            }

            UpdateGridHeight(grid, dates.Count);
            grid.Parent?.PerformLayout();
            grid.Parent?.Parent?.PerformLayout();
        }

        private async void OnSave(object? sender, EventArgs e)
        {
            var pastCheck = _datesBreakfast.Concat(_datesLunch).Concat(_datesDinner).Distinct();
            if (pastCheck.Any(d => d.Date < DateTime.Now.Date))
            {
                if (MessageBox.Show("과거 날짜가 포함되어 있습니다. 계속 저장하시겠습니까?", "확인",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            if (_datesBreakfast.Count == 0 && _datesLunch.Count == 0 && _datesDinner.Count == 0)
            {
                MessageBox.Show("입력된 식단이 없습니다.", "알림");
                return;
            }

            var invalidMeals = new List<string>();
            var grids = new[] { _gridBreakfast, _gridLunch, _gridDinner };
            string[] mealLabels = { "조식", "중식", "석식" };

            for (int m = 0; m < 3; m++)
            {
                foreach (DataGridViewRow row in grids[m].Rows)
                {
                    string menuText = row.Cells["menu"].Value?.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrWhiteSpace(menuText) && !IsValidMenu(menuText))
                    {
                        invalidMeals.Add(mealLabels[m]);
                        break;
                    }
                }
            }

            if (invalidMeals.Count > 0)
            {
                string mealList = string.Join(", ", invalidMeals);
                MessageBox.Show($"{mealList} 메뉴에 허용되지 않는 특수문자가 포함되어 있습니다.", "알림");
                return;
            }

            var result = BuildResult();

            var existingDates = _allDetail
                .Select(d => d.mealDate)
                .Distinct()
                .ToHashSet();

            var allDates = result
                .Select(d => d.mealDate)
                .Distinct()
                .ToHashSet();

            if (allDates.Any(d => DateTime.Parse(d) > DateTime.Now.AddMonths(1)))
            {
                MessageBox.Show("오늘로부터 1달 이내의 날짜만 등록할 수 있습니다.", "알림");
                return;
            }

            var duplicates = allDates
                .Where(d => existingDates.Contains(d))
                .ToList();

            if (duplicates.Count > 0)
            {
                string dateList = string.Join(", ", duplicates);
                MessageBox.Show(
                    $"이미 등록된 날짜입니다.\n{dateList}\n해당 날짜를 제거 후 저장해주세요.",
                    "알림");
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
                    
                    bool hasMenu = !string.IsNullOrWhiteSpace(menuText);

                    result.Add(new CafeteriaDto
                    {
                        mealDate = date,
                        mealType = mealType,
                        menu = hasMenu ? TextToJson(menuText) : "[]",
                        mealClosed = hasMenu ? "N" : "Y"
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

        private bool IsValidMenu(string menuText)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
                menuText, @"^[가-힣a-zA-Z0-9\s,]+$");
        }
    }
}