using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.View
{
    public partial class ClassroomView : UserControl, ISearchFocusable
    {
        private List<ClassroomDto> _all = new();
        private List<ClassroomDto> _pageItems = new();
        private readonly AdminClassroomController _adminClassroomController = new AdminClassroomController();

        private List<ClassroomDto> _filtered = new();

        private string _sortColumn = "floor";
        private bool _sortAscending = true;

        private readonly KpiSummaryBar _kpiBar = new();
        public ClassroomView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();

            grid.SortChanged += (_, s) =>
            {
                _sortColumn = s.Column;
                _sortAscending = s.Ascending;
                ApplyFilter();
                RenderPage(1);
            };

            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;

            bodyPanel.Controls.Add(_kpiBar);
            _kpiBar.SetCards(
                ("전체 강의실", ThemeColors.Primary),
                ("1층", ThemeColors.Ok),
                ("2층", ThemeColors.Warn),
                ("3층", ThemeColors.Danger),
                ("4층", ThemeColors.Sync)
            );

            tableCard.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, tableCard.Width - 1, tableCard.Height - 1);
            };
            filterCard.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, filterCard.Width - 1, filterCard.Height - 1);
            };

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender(1);
            pagination1.PageChanged += (_, page) => RenderPage(page);

            txtSearch.TextChanged += (_, _) => { ApplyFilter(); RenderPage(1); };
            cmbFloor.SelectedIndexChanged += OnFloorChanged;

            grid.PageNavigationRequested += (_, nav) =>
            {
                switch (nav)
                {
                    case PageNavigation.Next: pagination1.GoToNext(); break;
                    case PageNavigation.Prev: pagination1.GoToPrev(); break;
                    case PageNavigation.First: pagination1.GoToFirst(); break;
                    case PageNavigation.Last: pagination1.GoToLast(); break;
                }
            };
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            grid.SetInitialSort(_sortColumn, _sortAscending);
            await LoadAndRender(1);
            grid.Focus();
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(tableCard, 0);
            bodyPanel.Controls.SetChildIndex(gapPanel, 1);
            bodyPanel.Controls.SetChildIndex(filterCard, 2);
            bodyPanel.Controls.SetChildIndex(_kpiBar, 3);
            bodyPanel.Controls.SetChildIndex(pagination1, 4);
        }

        // ===== 데이터 연동 지점 =====
        private async Task<List<ClassroomDto>> LoadData()
        {
            var res = await _adminClassroomController.GetClassrooms();
            return res?.Data ?? new List<ClassroomDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩 중...") : null;

            _adminClassroomController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                _all = await LoadData();
                UpdateKpi();
                InitFloorCombo();
                ApplyFilter();
                RenderPage(page);
            }
            finally
            {
                _adminClassroomController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        private void UpdateKpi()
        {
            _kpiBar.SetValues(
                $"{_all.Count}개",
                $"{_all.Count(c => c.floor == 1)}개",
                $"{_all.Count(c => c.floor == 2)}개",
                $"{_all.Count(c => c.floor == 3)}개",
                $"{_all.Count(c => c.floor == 4)}개"
            );
        }

        private void InitFloorCombo()
        {
            var floorItems = new[] { new { Value = (int?)null, Label = "전체" } }
                .Concat(_all.Select(c => c.floor).Distinct().OrderBy(f => f)
                    .Select(f => new { Value = (int?)f, Label = $"{f}층" }))
                .ToList();

            cmbFloor.SelectedIndexChanged -= OnFloorChanged;
            cmbFloor.DataSource = floorItems;
            cmbFloor.DisplayMember = "Label";
            cmbFloor.ValueMember = "Value";
            cmbFloor.SelectedIndex = 0;
            cmbFloor.SelectedIndexChanged += OnFloorChanged;
        }

        // ===== 그리드 =====
        private void SetupGrid()
        {
            grid.Columns.Add("classroomName", "강의실명");
            grid.Columns.Add("floor", "층");
            grid.Columns.Add("imageId", "SVG ID");
            grid.Columns.Add("imagePath", "이미지 경로");
            grid.AddTextActionColumns(includeDelete: false);

            grid.Columns["classroomName"].FillWeight = 200;
            grid.Columns["floor"].FillWeight = 80;
            grid.Columns["imageId"].FillWeight = 150;
            grid.Columns["imagePath"].FillWeight = 300;

            grid.ActionClicked += OnRowAction;
        }

        private void RenderPage(int page)
        {
            pagination1.TotalCount = _filtered.Count;
            var size = pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_filtered.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination1.PageIndex = page;

            _pageItems = _filtered.Skip((page - 1) * size).Take(size).ToList();

            grid.SuspendLayout();
            grid.Rows.Clear();

            foreach (var c in _pageItems)
            {
                var idx = grid.Rows.Add(c.classroomName, c.floor, c.imageId, c.imagePath);
                grid.Rows[idx].Tag = c;
            }
            grid.ResumeLayout();
        }

        // ===== CRUD =====
        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            var target = e.Tag as ClassroomDto;
            if(target == null || e.Action != TableAction.Edit)
            {
                return;
            }

            var edited = ClassroomEditModal.Show(this.FindForm(), target);
            if (edited == null) return;

            var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
            _adminClassroomController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                var res = await _adminClassroomController.UpdateClassroom(target.classroomId, edited);
                if (res?.Status == 200)
                {
                    var idx = _all.IndexOf(target);
                    if (idx >= 0) _all[idx] = edited;
                    ApplyFilter();
                    RenderPage(pagination1.PageIndex);
                }
                else
                    MessageBox.Show(this.FindForm(), res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApiException ex) { MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception ex) { MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { _adminClassroomController.OnRetry = null; overlay.Close(); overlay.Dispose(); }
        }

        // ===== 필터 =====
        private void ApplyFilter()
        {
            var result = _all.AsEnumerable();

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
                result = result.Where(c => c.classroomName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

            if (cmbFloor.SelectedValue is int floor)
                result = result.Where(c => c.floor == floor);

            _filtered = result.ToList();
            ApplySort();
        }

        private void ApplySort()
        {
            if (string.IsNullOrEmpty(_sortColumn)) return;
            Func<ClassroomDto, object?> key = _sortColumn switch
            {
                "classroomName" => c => c.classroomName,
                "floor" => c => (object?)c.floor,
                "imageId" => c => c.imageId,
                "imagePath" => c => c.imagePath,
                _ => c => null
            };
            _filtered = _sortAscending ? _filtered.OrderBy(key).ToList() : _filtered.OrderByDescending(key).ToList();
        }

        private void OnFloorChanged(object? sender, EventArgs e)
        {
            ApplyFilter();
            RenderPage(1);
        }

        // ===== ISearchFocusable =====
        public void FocusSearch() => txtSearch.Focus();

        // ===== 키보드 이벤트 =====
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ActiveControl is TextBox or ComboBox)
                return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.Control | Keys.Right: pagination1.GoToNext(); return true;
                case Keys.Control | Keys.Left: pagination1.GoToPrev(); return true;
                case Keys.Control | Keys.Home: pagination1.GoToFirst(); return true;
                case Keys.Control | Keys.End: pagination1.GoToLast(); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
