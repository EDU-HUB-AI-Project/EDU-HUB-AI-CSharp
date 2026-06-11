using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;
using System.Text.Json;

namespace EDU_HUB_AI.View
{
    public partial class CafeteriaView : UserControl
    {
        private List<Dictionary<string, object>> _all = new List<Dictionary<string, object>>();
        private List<Dictionary<string, object>> _pageItems = new List<Dictionary<string, object>>();
        private List<CafeteriaDto> _allDetail = new List<CafeteriaDto>();
        private readonly AdminCafeteriaController _adminCafeteriaController = new AdminCafeteriaController();
        private DateField _datePickerStart;
        private DateField _datePickerEnd;
        private Panel _filterCard;
        private Panel _gapPanel;

        public CafeteriaView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            _filterCard = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(16, 12, 16, 12),
                Height = 100
            };

            var topPanel = new Panel
            {
                Dock = DockStyle.Fill,
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
                WrapContents = false
            };

            var dateStart = new DateField
            {
                FieldLabel = "시작일",
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                Size = new Size(150, 62),
                Margin = new Padding(0, 0, 8, 0)
            };

            var lblSep = new Label
            {
                Text = "~",
                Font = ThemeFonts.Body,
                AutoSize = true,
                Margin = new Padding(0, 35, 8, 0)
            };

            var dateEnd = new DateField
            {
                FieldLabel = "종료일",
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                    DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
                Size = new Size(150, 62),
                Margin = new Padding(0, 0, 8, 0)
            };

            var btnSearch = new AppButton
            {
                Text = "조회",
                Variant = ButtonVariant.Primary,
                Margin = new Padding(8, 20, 0, 0)
            };
            btnSearch.Click += async (_, _) => await LoadAndRender();

            _datePickerStart = dateStart;
            _datePickerEnd = dateEnd;

            dateStart.ValueChanged += async (_, _) =>
            {
                await LoadAndRender();
                ScrollToData(_datePickerStart.Value.ToString("yyyy-MM-dd"));
            };

            leftPanel.Controls.Add(dateStart);
            leftPanel.Controls.Add(lblSep);
            leftPanel.Controls.Add(dateEnd);
            leftPanel.Controls.Add(btnSearch);

            var btnCreateNew = new AppButton
            {
                Text = "+ 식단 추가",
                Variant = ButtonVariant.Primary,
                Margin = new Padding(0, 20, 0, 0)
            };
            btnCreateNew.Click += OnCreate;

            rightPanel.Controls.Add(btnCreateNew);

            topPanel.Controls.Add(rightPanel);
            topPanel.Controls.Add(leftPanel);
            _filterCard.Controls.Add(topPanel);

            _gapPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 14,
                BackColor = ThemeColors.Background
            };

            bodyPanel.Controls.Add(_filterCard);
            bodyPanel.Controls.Add(_gapPanel);

            SetupGrid();
            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender();
            pagination1.PageChanged += (_, page) => RenderPage(page);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            await LoadAndRender();
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(grid, 0);
            bodyPanel.Controls.SetChildIndex(pagination1, 1);
            bodyPanel.Controls.SetChildIndex(_gapPanel, 2);
            bodyPanel.Controls.SetChildIndex(_filterCard, 3);
        }

        private async Task<List<Dictionary<string, object>>> LoadData()
        {
            var result = new List<Dictionary<string, object>>();
            _allDetail = new List<CafeteriaDto>();

            DateTime start = _datePickerStart.Value.Date;
            DateTime end = _datePickerEnd.Value.Date;

            if (start > end)
            {
                MessageBox.Show("시작일이 종료일보다 클 수 없습니다.", "알림");
                return result;
            }

            string startStr = start.ToString("yyyy-MM-dd");
            string endStr = end.ToString("yyyy-MM-dd");

            DateTime current = new DateTime(start.Year, start.Month, 1);
            while (current <= end)
            {
                string month = current.ToString("yyyy-MM");
                var res = await _adminCafeteriaController.GetCafeteriaSummary(month);
                var monthData = res?.Data ?? new List<Dictionary<string, object>>();

                foreach (var item in monthData)
                {
                    if (!item.ContainsKey("mealDate")) continue;

                    string mealDateStr = item["mealDate"].ToString();

                    if (string.Compare(mealDateStr, startStr) >= 0 &&
                        string.Compare(mealDateStr, endStr) <= 0)
                    {
                        result.Add(item);

                        if (item.ContainsKey("details") && item["details"] is JsonElement detailsElement
                            && detailsElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var d in detailsElement.EnumerateArray())
                            {
                                _allDetail.Add(new CafeteriaDto
                                {
                                    cafeteriaId = d.TryGetProperty("cafeteriaId", out var cid) ? cid.GetString() : null,
                                    mealDate = d.TryGetProperty("mealDate", out var md) ? md.GetString() : null,
                                    mealType = d.TryGetProperty("mealType", out var mt) ? mt.GetString() : null,
                                    menu = d.TryGetProperty("menu", out var mn) ? mn.GetString() : null,
                                    mealClosed = d.TryGetProperty("mealClosed", out var mc) ? mc.GetString() : null
                                });
                            }
                        }
                    }
                }

                current = current.AddMonths(1);
            }

            return result;
        }

        private async Task LoadAndRender()
        {
            _all = await LoadData();
            RenderPage(1);
        }

        private void SetupGrid()
        {
            grid.Columns.Add("mealDate", "날짜");
            grid.Columns.Add("breakfast", "조식");
            grid.Columns.Add("lunch", "점심");
            grid.Columns.Add("dinner", "석식");
            grid.AddTextActionColumns();
            grid.ActionClicked += OnRowAction;
            grid.CellDoubleClick += OnCellDoubleClick;
        }

        private void RenderPage(int page)
        {
            pagination1.TotalCount = _all.Count;
            var size = pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_all.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination1.PageIndex = page;

            _pageItems = new List<Dictionary<string, object>>();
            int startIndex = (page - 1) * size;
            int endIndex = Math.Min(startIndex + size, _all.Count);
            for (int i = startIndex; i < endIndex; i++)
            {
                _pageItems.Add(_all[i]);
            }

            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach (var s in _pageItems)
            {
                string mealDate = "";
                string breakfast = "X";
                string lunch = "X";
                string dinner = "X";

                if (s.ContainsKey("mealDate")) mealDate = s["mealDate"].ToString();
                if (s.ContainsKey("BREAKFAST")) breakfast = s["BREAKFAST"].ToString();
                if (s.ContainsKey("LUNCH")) lunch = s["LUNCH"].ToString();
                if (s.ContainsKey("DINNER")) dinner = s["DINNER"].ToString();

                if (breakfast == "0") breakfast = "O";
                if (lunch == "0") lunch = "O";
                if (dinner == "0") dinner = "O";

                grid.Rows.Add(mealDate, breakfast, lunch, dinner);
            }
            grid.ResumeLayout();
        }

        private void ScrollToData(string targetData)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells["mealDate"].Value?.ToString() == targetData)
                {
                    grid.FirstDisplayedScrollingRowIndex = row.Index;
                    row.Selected = true;
                    break;
                }
            }
        }

        private void OnCreate(object? sender, EventArgs e)
        {
            var createView = new CafeteriaCreateView(_allDetail);
            createView.Dock = DockStyle.Fill;

            createView.OnBack += async () =>
            {
                this.Controls.Remove(createView);
                SetAllControlsVisible(true);
                await LoadAndRender();
            };

            SetAllControlsVisible(false);
            this.Controls.Add(createView);
            createView.BringToFront();
        }

        private void SetAllControlsVisible(bool visible)
        {
            foreach (Control c in this.Controls)
            {
                c.Visible = visible;
            }
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                string mealDate = target.ContainsKey("mealDate") ? target["mealDate"].ToString() : "";

                var existingList = _allDetail
                    .Where(d => d.mealDate == mealDate)
                    .ToList();

                using var modal = new CafeteriaEditModal(mealDate);
                modal.LoadExistingData(existingList);

                if (modal.ShowDialog(this.FindForm()) != DialogResult.OK) return;

                await _adminCafeteriaController.SaveCafeteriaList(modal.Result);
                await LoadAndRender();
            }
            else if (e.Action == TableAction.Delete)
            {
                string mealDate = target.ContainsKey("mealDate") ? target["mealDate"].ToString() : "";
                if (!ConfirmModal.Show(this.FindForm(), "삭제 확인", $"'{mealDate}' 식단을 삭제할까요?"))
                    return;
                await DeleteByDate(mealDate);
                await LoadAndRender();
            }
        }

        private async void OnCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];
            string mealDate = target.ContainsKey("mealDate") ? target["mealDate"].ToString() : "";

            var existingList = _allDetail
                .Where(d => d.mealDate == mealDate)
                .ToList();

            using var modal = new CafeteriaEditModal(mealDate);
            modal.LoadExistingData(existingList);

            if (modal.ShowDialog(this.FindForm()) != DialogResult.OK) return;

            await _adminCafeteriaController.SaveCafeteriaList(modal.Result);
            await LoadAndRender();
        }

        private async Task DeleteByDate(string mealDate)
        {
            var list = _allDetail
                .Where(d => d.mealDate == mealDate)
                .ToList();

            foreach (var item in list)
            {
                if (item.cafeteriaId == null) continue;
                await _adminCafeteriaController.DeleteCafeteria(item.cafeteriaId);
            }
        }
    }
}