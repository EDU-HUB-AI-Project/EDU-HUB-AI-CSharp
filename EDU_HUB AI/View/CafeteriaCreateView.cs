using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
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

        private DateField _datePicker;
        private ComboField _cmbMeal;
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

            var bodyPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.Background,
                Padding = new Padding(34, 20, 34, 24)
            };

            var filterCard = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(16, 12, 16, 12),
                Height = 100
            };

            _cmbMeal = new ComboField
            {
                FieldLabel = "식사",
                Size = new Size(120, 62),
                Margin = new Padding(0, 0, 16, 0)
            };
            _cmbMeal.PerformLayout();
            _cmbMeal.Items.AddRange(new object[] { "조식", "중식", "석식" });
            _cmbMeal.SelectedIndex = 0;
            _cmbMeal.SelectedIndexChanged += OnMealChanged;

            _datePicker = new DateField
            {
                FieldLabel = "시작일",
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Value = DateTime.Now,
                Size = new Size(150, 62),
                Margin = new Padding(0, 0, 16, 0)
            };
            _datePicker.PerformLayout();

            var btnAddDay = new AppButton
            {
                Text = "+ 하루 추가",
                Variant = ButtonVariant.Primary,
                Margin = new Padding(8, 20, 0, 0)
            };
            btnAddDay.Click += OnAddDay;

            _btnDeleteSelected = new AppButton
            {
                Text = "선택 삭제",
                Variant = ButtonVariant.Ghost,
                Enabled = false,
                Margin = new Padding(8, 20, 0, 0)
            };
            _btnDeleteSelected.Click += OnDeleteSelected;

            var gapPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 8,
                BackColor = ThemeColors.Background
            };

            var tableCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(16)
            };

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ThemeColors.Surface
            };

            var gridPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = ThemeColors.Surface
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
            tableCard.Controls.Add(scrollPanel);
            
            var topPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 76,
                BackColor = ThemeColors.Surface
            };

            var leftPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = ThemeColors.Surface,
                WrapContents = false
            };

            var rightPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = ThemeColors.Surface,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0)
            };

            leftPanel.Controls.Add(_cmbMeal);
            leftPanel.Controls.Add(_datePicker);
            leftPanel.Controls.Add(btnAddDay);
            leftPanel.Controls.Add(_btnDeleteSelected);

            var btnCancel = new AppButton
            {
                Text = "취소",
                Variant = ButtonVariant.Ghost,
                Margin = new Padding(0, 12, 8, 0)
            };
            btnCancel.Click += (_, _) => OnBack?.Invoke();

            var btnSave = new AppButton
            {
                Text = "저장",
                Variant = ButtonVariant.Primary,
                Margin = new Padding(0, 12, 0, 0)
            };
            btnSave.Click += OnSave;

            rightPanel.Controls.Add(btnCancel);
            rightPanel.Controls.Add(btnSave);

            topPanel.Controls.Add(rightPanel);
            topPanel.Controls.Add(leftPanel);

            filterCard.Controls.Add(topPanel);

            bodyPanel.Controls.Add(tableCard);
            bodyPanel.Controls.Add(gapPanel);
            bodyPanel.Controls.Add(filterCard);

            var pageHeader = new PageHeader
            {
                Dock = DockStyle.Top,
                Title = "식단 등록",
                ShowSyncButton = true,
                BackColor = ThemeColors.HeaderBg
            };
            pageHeader.SyncClicked += (_, _) => OnBack?.Invoke();
            Controls.Add(bodyPanel);
            Controls.Add(pageHeader);
            ShowSection(0);

            this.HandleCreated += (_, _) =>
            {
                var dates = GetCurrentDates();
                var grid = GetCurrentGrid();
                InitDatesFor(dates);
                RebuildRows(grid, dates);

                _datePicker.ValueChanged += OnDatePickerChanged;
            };
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
            int rowHeight = rowCount > 0 ? grid.Rows[0].Height : 44;
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
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("맑은 고딕", 9F),
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
                RowTemplate = { Height = 44 },
                ScrollBars = ScrollBars.None,
                Height = 30,
                GridColor = Color.FromArgb(226, 232, 240),
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    BackColor = Color.FromArgb(241, 245, 249),
                    Font = new Font("맑은 고딕", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    Padding = new Padding(8, 0, 8, 0),
                    SelectionBackColor = Color.FromArgb(241, 245, 249)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    Font = new Font("맑은 고딕", 9F),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    Padding = new Padding(8, 0, 8, 0),
                    SelectionBackColor = Color.White,
                    SelectionForeColor = Color.FromArgb(15, 23, 42),
                    WrapMode = DataGridViewTriState.False
                }
            };

            grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "✓",
                Name = "check",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "날짜",
                Name = "date",
                Width = 120,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            grid.Columns["date"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

            grid.CellFormatting += (s, e) =>
            {
                  if (e.ColumnIndex == grid.Columns["date"].Index)
                  {
                      e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                      e.CellStyle.BackColor = Color.FromArgb(241, 245, 249);
                      e.CellStyle.ForeColor = Color.FromArgb(100, 116, 139);
                      e.FormattingApplied = true;
                  }
            };

            grid.CellPainting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (grid.Columns[e.ColumnIndex].Name != "date") return;

                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                using var brush = new SolidBrush(Color.FromArgb(100, 116, 139));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(241, 245, 249)), e.CellBounds);
                e.Graphics.DrawString(e.Value?.ToString(), grid.Font, brush, e.CellBounds, sf);
                e.Handled = true;
            };

            grid.RowsAdded += (s, e) =>
            {
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    if (i >= 0 && i < grid.Rows.Count)
                        grid.Rows[i].Height = 44;
                }
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
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0, 0, 0, 16)
            };

            section.Controls.Add(grid);
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