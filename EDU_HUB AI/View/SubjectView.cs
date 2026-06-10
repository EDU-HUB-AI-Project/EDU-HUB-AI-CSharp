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
    public partial class SubjectView : UserControl
    {
        private List<SubjectDto> _all = new();
        private List<SubjectDto> _pageItems = new();
        private List<SubjectDto> _filtered = new();
        private AdminSubjectController _adminSubjectController = new AdminSubjectController();

        private List<EduInfoDto> _eduInfos = new();
        private readonly AdminEduInfoController _adminEduInfoController = new AdminEduInfoController();

        private bool _suppressFilter = false;

        public SubjectView()
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
            btnCreate.Click += OnCreate;
            pagination1.PageChanged += (_, page) => RenderPage(page);

            cmbEdu.SelectedIndexChanged += (_, _) => { if (!_suppressFilter) { ApplyFilter(); RenderPage(1); } };
            cmbStatus.SelectedIndexChanged += (_, _) => { if (!_suppressFilter) { ApplyFilter(); RenderPage(1); } };
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
            bodyPanel.Controls.SetChildIndex(tableCard, 0);
            bodyPanel.Controls.SetChildIndex(gapPanel, 1);
            bodyPanel.Controls.SetChildIndex(filterCard, 2);
            bodyPanel.Controls.SetChildIndex(pagination1, 3);
        }

        // ===== 데이터 연동 지점 =====
        private async Task<List<SubjectDto>> LoadData()
        {
            var res = await _adminSubjectController.GetSubjects();
            return res?.Data ?? new List<SubjectDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩 중...") : null;

            _adminSubjectController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                _all = await LoadData();

                try
                {
                    var eduRes = await _adminEduInfoController.GetEduInfos();
                    _eduInfos = eduRes?.Data ?? new List<EduInfoDto>();
                }
                catch
                {
                    _eduInfos = new List<EduInfoDto>();
                }

                var prevEduId = cmbEdu.SelectedValue?.ToString();

                _suppressFilter = true;
                SetupFilterSource();
                if (!string.IsNullOrEmpty(prevEduId))
                    cmbEdu.SelectedValue = prevEduId;
                _suppressFilter = false;

                ApplyFilter();
                RenderPage(page);
            }
            finally
            {
                _adminSubjectController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        // ===== 그리드 =====
        private void SetupGrid()
        {
            grid.Columns.Add("subjectName", "과목명");
            grid.Columns.Add("edu", "교육과정");
            grid.Columns.Add("startDate", "시작일");
            grid.Columns.Add("endDate", "종료일");
            grid.Columns.Add("endYn", "상태");
            grid.AddTextActionColumns();

            grid.Columns["subjectName"].FillWeight = 250;
            grid.Columns["edu"].FillWeight = 250;
            grid.Columns["startDate"].FillWeight = 120;
            grid.Columns["endDate"].FillWeight = 120;
            grid.Columns["endYn"].FillWeight = 80;

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

            foreach (var s in _pageItems)
            {
                var eduName = _eduInfos.FirstOrDefault(e => e.eduId == s.eduId)?.eduName ?? s.eduId;
                grid.Rows.Add(s.subjectName, eduName, s.startDate, s.endDate, EndYnLabel(s.endYn));
            }
            grid.ResumeLayout();
        }

        // ===== CRUD =====
        protected async void OnCreate(object? sender, EventArgs e)
        {
            var created = SubjectEditModal.Show(this.FindForm(), null);
            if (created == null) return;

            var overlay = LoadingOverlay.Create(bodyPanel, "등록 중...");
            _adminSubjectController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중... \n재시도 {attempt}/{max}");

            try
            {
                var res = await _adminSubjectController.InsertSubject(created);
                if (res?.Status == 200)
                    await LoadAndRender(int.MaxValue, showOverlay: false);
                else
                    MessageBox.Show(this.FindForm(), res?.Message ?? "등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApiException ex) { MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception ex) { MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { _adminSubjectController.OnRetry = null; overlay.Close(); overlay.Dispose(); }
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                var edited = SubjectEditModal.Show(this.FindForm(), target);
                if (edited == null) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _adminSubjectController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _adminSubjectController.UpdateSubject(target.subjectId, edited);
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
                finally { _adminSubjectController.OnRetry = null; overlay.Close(); overlay.Dispose(); }
            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(this.FindForm(), "삭제 확인", $"'{target.subjectName}'을(를) 삭제할까요?"))
                    return;

                var overlay = LoadingOverlay.Create(bodyPanel, "삭제 중...");
                _adminSubjectController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _adminSubjectController.DeleteSubject(target.subjectId);
                    if (res?.Status == 200)
                    {
                        _all.Remove(target);
                        ApplyFilter();
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                        MessageBox.Show(this.FindForm(), res?.Message ?? "삭제에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ApiException ex) { MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                catch (Exception ex) { MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { _adminSubjectController.OnRetry = null; overlay.Close(); overlay.Dispose(); }
            }
        }

        // ===== 헬퍼 =====
        private static string EndYnLabel(string? endYn) => string.Equals(endYn, "Y", StringComparison.OrdinalIgnoreCase) ? "종료" : "진행중";

        // ===== 필터링 =====
        private void SetupFilterSource()
        {
            var eduList = new List<EduInfoDto> { new() { eduId = "", eduName = "전체" } };
            eduList.AddRange(_eduInfos);
            cmbEdu.DataSource = eduList;
            cmbEdu.DisplayMember = "eduName";
            cmbEdu.ValueMember = "eduId";

            var statusList = new[]
            {
                new { Value = "", Label = "전체" },
                new { Value = "N", Label = "진행중" },
                new { Value = "Y", Label = "종료" }
            };
            cmbStatus.DataSource = statusList.ToList();
            cmbStatus.DisplayMember = "Label";
            cmbStatus.ValueMember = "Value";
        }

        private void ApplyFilter()
        {
            var result = _all.AsEnumerable();

            var eduId = cmbEdu.SelectedValue?.ToString();
            if (!string.IsNullOrEmpty(eduId))
                result = result.Where(s => s.eduId == eduId);

            var status = cmbStatus.SelectedValue?.ToString();
            if (!string.IsNullOrEmpty(status))
                result = result.Where(s => s.endYn == status);

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
                result = result.Where(s => s.subjectName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

            _filtered = result.ToList();
        }
    }
}
