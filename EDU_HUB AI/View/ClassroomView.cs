using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.View
{
    /// <summary>
    /// CRUD 테이블 화면 템플릿 (목데이터 기반).
    ///
    /// ── 사용법 ────────────────────────────────────────────
    /// 1. 이 Form을 복사
    /// 2. StudentDto → 사용할 DTO로 바꾸고 컬럼/입력 필드를 교체
    /// 3. LoadData() 안의 목업을 실제 Controller 호출로 교체
    /// 4. pageHeader1.Title 설정 · SyncClicked 이벤트 연결
    /// 5. actionPanel(ActionBar) 안에 AppButton을 드래그해 추가 — 자동 우측 정렬, Click 연결
    /// 6. OnRowAction()/OnCreate()의 // TODO: API 지점을 연결
    /// 페이지네이션은 전체 목록을 메모리에 두고 클라이언트에서 자르기
    /// ──────────────────────────────────────────────────────────
    /// </summary>
    public partial class ClassroomView : UserControl
    {
        private List<ClassroomDto> _all = new();
        private List<ClassroomDto> _pageItems = new();
        private readonly AdminClassroomController _adminClassroomController = new AdminClassroomController();

        private List<ClassroomDto> _filtered = new();

        public ClassroomView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;

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
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            await LoadAndRender(1);
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(tableCard, 0);
            bodyPanel.Controls.SetChildIndex(gapPanel, 1);
            bodyPanel.Controls.SetChildIndex(filterCard, 2);
            bodyPanel.Controls.SetChildIndex(pagination1, 3);
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
            grid.AddTextActionColumns();

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
                grid.Rows.Add(c.classroomName, c.floor, c.imageId, c.imagePath);
            }
            grid.ResumeLayout();
        }

        // ===== CRUD =====
        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            if (e.Action != TableAction.Edit) return;
            var target = _pageItems[e.RowIndex];

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

        // ===== 필터링 =====
        private void ApplyFilter()
        {
            var result = _all.AsEnumerable();

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
                result = result.Where(c => c.classroomName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

            if (cmbFloor.SelectedValue is int floor)
                result = result.Where(c => c.floor == floor);

            _filtered = result.ToList();
        }

        private void OnFloorChanged(object? sender, EventArgs e)
        {
            ApplyFilter();
            RenderPage(1);
        }
    }
}
