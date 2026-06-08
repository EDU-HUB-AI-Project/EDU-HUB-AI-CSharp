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
    public partial class EduInfoView : UserControl
    {
        private List<EduInfoDto> _all = new();
        private List<EduInfoDto> _pageItems = new();
        private readonly AdminEduInfoController _adminEduInfoController = new AdminEduInfoController();

        private List<EduInfoDto> _filtered = new();

        public EduInfoView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;
            //FixDockOrder();

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender(1);
            btnCreate.Click += OnCreate;
            pagination1.PageChanged += (_, page) => RenderPage(page);

            txtSearch.TextChanged += (_, _) => { ApplyFilter(); RenderPage(1); };
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            await LoadAndRender(1);
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(grid, 0);
            bodyPanel.Controls.SetChildIndex(actionPanel, 1);
            bodyPanel.Controls.SetChildIndex(pagination1, 2);
        }

        // ===== 데이터 연동 지점 =====
        private async Task<List<EduInfoDto>> LoadData()
        {
            var res = await _adminEduInfoController.GetEduInfos();
            return res?.Data ?? new List<EduInfoDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩 중...") : null;

            _adminEduInfoController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                _all = await LoadData();
                ApplyFilter();
                RenderPage(page);
            }
            finally
            {
                _adminEduInfoController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        // ===== 그리드 =====
        private void SetupGrid()
        {
            grid.Columns.Add("eduName", "과정명");
            grid.Columns.Add("startDate", "시작일");
            grid.Columns.Add("endDate", "종료일");
            grid.Columns.Add("batchNumber", "기수");
            grid.Columns.Add("capacity", "정원");
            grid.AddTextActionColumns();

            grid.Columns["eduName"].FillWeight = 300;
            grid.Columns["startDate"].FillWeight = 130;
            grid.Columns["endDate"].FillWeight = 130;
            grid.Columns["batchNumber"].FillWeight = 80;
            grid.Columns["capacity"].FillWeight = 80;

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

            foreach(var e in _pageItems)
            {
                grid.Rows.Add(e.eduName, e.startDate, e.endDate, $"{e.batchNumber}기", e.capacity);
            }
            grid.ResumeLayout();
        }

        // ===== CRUD =====
        /// <summary>등록 버튼 Click 이벤트에 연결 (디자이너에서 AppButton 추가 후 연결)</summary>
        protected async void OnCreate(object? sender, EventArgs e)
        {
            var created = EduInfoEditModal.Show(this.FindForm(), null);
            if (created == null) return;

            var overlay = LoadingOverlay.Create(bodyPanel, "등록 중...");
            _adminEduInfoController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중... \n재시도 {attempt}/{max}");

            try
            {
                var res = await _adminEduInfoController.InsertEduInfo(created);
                if (res?.Status == 200)
                {
                    await LoadAndRender(int.MaxValue, showOverlay: false);
                }
                else
                {
                    MessageBox.Show(res?.Message ?? "등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(ApiException ex)
            {
                MessageBox.Show(ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminEduInfoController.OnRetry = null;
                overlay.Close();
                overlay.Dispose();
            }
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                var edited = EduInfoEditModal.Show(this.FindForm(), target);
                if (edited == null) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _adminEduInfoController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _adminEduInfoController.UpdateEduInfo(target.eduId, edited);
                    if (res?.Status == 200)
                    {
                        var idx = _all.IndexOf(target);
                        if (idx >= 0) _all[idx] = edited;
                        ApplyFilter();
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
                    _adminEduInfoController.OnRetry = null;
                    overlay.Close();
                    overlay.Dispose();
                }
            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(this.FindForm(), "삭제 확인", $"'{target.eduName}'을(를) 삭제할까요?"))
                {
                    return;
                }

                var overlay = LoadingOverlay.Create(bodyPanel, "삭제 중...");
                _adminEduInfoController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _adminEduInfoController.DeleteEduInfo(target.eduId);
                    if (res?.Status == 200)
                    {
                        _all.Remove(target);
                        ApplyFilter();
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                    {
                        MessageBox.Show(res?.Message ?? "삭제에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    _adminEduInfoController.OnRetry = null;
                    overlay.Close();
                    overlay.Dispose();
                }
            }
        }
        

        private void ApplyFilter()
        {
            var result = _all.AsEnumerable();

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
                result = result.Where(e => e.eduName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

            _filtered = result.ToList();
        }
    }
}
