using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using System.Data;


namespace EDU_HUB_AI.View
{
    public partial class DormRoomView : UserControl
    {
        private List<DormitoryDto> _all = new();
        private List<DormitoryDto> _pageItems = new();
        private List<DormitoryDto> _fiteredList = new();
        private readonly AdminDormitoryController _adminDormitoryController = new();

        private string _sortColumn = "dormitoryRoomName";
        private bool _sortAscending = true;

        public DormRoomView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();

            grid.SortChanged += (_, s) =>
            {
                _sortColumn = s.Column;
                _sortAscending = s.Ascending;
                ApplySort();
                RenderPage(1);
            };

            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;
            pageHeader1.SyncClicked += (_, _) => LoadAndRender(1);
            pagination1.PageChanged += (_, page) => RenderPage(page);
            cmbDormRoom.SelectedIndexChanged += (_, _) => { ApplySearchFilter(); RenderPage(1); };
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            await LoadCmb();
            grid.SetInitialSort(_sortColumn, _sortAscending);
            await LoadAndRender(1);
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(tableCard, 0);
            bodyPanel.Controls.SetChildIndex(gapPanel, 1);
            bodyPanel.Controls.SetChildIndex(filterCard, 2);
            bodyPanel.Controls.SetChildIndex(pagination1, 3);
        }

        private async Task<List<DormitoryDto>> LoadData()
        {
            var res = await _adminDormitoryController.GetDormRoomAssignStatus(null);
            return res?.Data ?? new List<DormitoryDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩중...") : null;
            _adminDormitoryController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
            try
            {
                _all = await LoadData();
                ApplySort();
                RenderPage(page);
            }
            catch (ApiException ex)
            {
                MessageBox.Show(ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminDormitoryController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        private void SetupGrid()
        {
            grid.Columns.Add("dormitoryRoomName", "호실");
            grid.Columns.Add("currentCount", "배정 인원");
            grid.Columns.Add("maxCount", "최대 인원");
            grid.AddTextActionColumns(true, false);
            grid.ActionClicked += OnRowAction;
            grid.CellFormatting += OnCellFormatting;
        }

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RenderPage(int page)
        {
            bool hasFilter = cmbDormRoom.SelectedIndex > 0;
            var source = hasFilter ? _fiteredList : _all;
            pagination1.TotalCount = source.Count;
            var size = pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(source.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination1.PageIndex = page;

            _pageItems = source.Skip((page - 1) * size).Take(size).ToList();

            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach (var a in _pageItems)
                grid.Rows.Add(a.dormitoryRoomName, a.currentCount, a.maxCount);
            grid.ResumeLayout();
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                var edited = DormMaxCntEditModal.Show(this.FindForm(), target);
                if (edited == null) return;
                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _adminDormitoryController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                try
                {
                    // TODO: API 수정 — await new AdminStudentController().UpdateStudent(target.studentId, edited);
                    var res = await _adminDormitoryController.UpdateDormAssignMaxCnt(edited);
                    if (res?.Status == 200)
                    {
                        var idx = _all.IndexOf(target);
                        if (idx >= 0) _all[idx] = edited;
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                    {
                        MessageBox.Show(res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch (ApiException ex)
                {
                    MessageBox.Show(ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _adminDormitoryController.OnRetry = null;
                    overlay.Close();
                    overlay.Dispose();
                }

            }
        }

        private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || sender is not AppDataGrid grid) return;
            var col = grid.Columns[e.ColumnIndex].Name;
            if (col is "dormitoryRoomName" or "phone")
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            if (col == "dormitoryRoomName" && e.Value is string room && !string.IsNullOrEmpty(room))
            {
                e.Value = room + "호";
                e.FormattingApplied = true;
            }
            if (col is "checkIn" or "checkOut" && e.Value is string dt && !string.IsNullOrEmpty(dt))
            {
                if (DateTime.TryParse(dt, out var parsed))
                {
                    e.Value = parsed.ToString("yyyy-MM-dd HH:mm");
                    e.FormattingApplied = true;
                }
            }

            if(col == "currentCount" && e.RowIndex < _pageItems.Count)
            {
                var item = _pageItems[e.RowIndex];
                if(item.maxCount > 0 && item.currentCount >= item.maxCount)
                {
                    e.CellStyle.BackColor = ThemeColors.DangerBg;
                    e.CellStyle.ForeColor = ThemeColors.DangerText;
                    e.CellStyle.SelectionBackColor = ThemeColors.DangerBg;
                    e.CellStyle.SelectionForeColor = ThemeColors.DangerText;
                }
            }
        }

        private async Task LoadCmb()
        {
            var response = await _adminDormitoryController.GetDormRoomAssignStatus();
            if (response?.Status == 200)
            {
                var floors = response.Data
                                .Where(x => !string.IsNullOrEmpty(x.dormitoryRoomName))
                                .Select(x => x.dormitoryRoomName[0].ToString())
                                .Distinct()
                                .OrderBy(f => f)
                                .Select(f => new { Floor = f, Label = f + "층" })
                                .ToList();

                floors.Insert(0, new { Floor = "", Label = "전체" });

                cmbDormRoom.DataSource = floors;
                cmbDormRoom.DisplayMember = "Label";
                cmbDormRoom.ValueMember = "Floor";
            }
        }

        private void ApplySearchFilter()
        {
            var floor = cmbDormRoom.SelectedValue?.ToString();
            var result = string.IsNullOrEmpty(floor)
                ? _all.AsEnumerable()
                : _all.Where(d => !string.IsNullOrEmpty(d.dormitoryRoomName) && d.dormitoryRoomName.StartsWith(floor));
            _fiteredList = result.ToList();
            ApplySort();
        }

        private void ApplySort()
        {
            if (string.IsNullOrEmpty(_sortColumn)) return;
            Func<DormitoryDto, object?> key = _sortColumn switch
            {
                "dormitoryRoomName" => d => d.dormitoryRoomName,
                "currentCount" => d => (object?)d.currentCount,
                "maxCount" => d => (object?)d.maxCount,
                _ => d => null
            };
            _all = _sortAscending ? _all.OrderBy(key).ToList() : _all.OrderByDescending(key).ToList();
            _fiteredList = _sortAscending ? _fiteredList.OrderBy(key).ToList() : _fiteredList.OrderByDescending(key).ToList();
        }
    }
}
