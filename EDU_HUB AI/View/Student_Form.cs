using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

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
    public partial class Student_Form : Form
    {
        private List<StudentDto> _all = new();
        private List<StudentDto> _pageItems = new();
        private AdminStudentController _adminStudentController = new AdminStudentController();

        private List<EduInfoDto> _eduInfos = new();
        private List<StudentDto> _filtered = new();
        private readonly AdminEduInfoController _adminEduInfoController = new();

        public Student_Form()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;
            FixDockOrder();

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender();
            btnCreate.Click += OnCreate;
            pagination1.PageChanged += (_, page) => RenderPage(page);

            cmbEdu.SelectedIndexChanged += (_, _) => { ApplyFilter(); RenderPage(1); };
            cmbBatch.SelectedIndexChanged += (_, _) => { ApplyFilter(); RenderPage(1); };
            txtSearch.TextChanged += (_, _) => { ApplyFilter(); RenderPage(1); };
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
            bodyPanel.Controls.SetChildIndex(actionPanel, 1);
            bodyPanel.Controls.SetChildIndex(pagination1, 2);
        }

        // ===== 데이터 연동 지점 =====
        private async Task<List<StudentDto>> LoadData()
        {
            var res = await _adminStudentController.GetStudents();
            return res?.Data ?? new List<StudentDto>();
        }

        private async Task LoadAndRender()
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

            SetupFilterSource();
            ApplyFilter();
            RenderPage(1);
        }

        // ===== 그리드 =====
        private void SetupGrid()
        {
            grid.Columns.Add("name", "이름");
            grid.Columns.Add("birth", "생년월일");
            grid.Columns.Add("phone", "연락처");
            grid.Columns.Add("edu", "과정");
            grid.Columns.Add("dorm", "생활관");
            grid.AddTextActionColumns();
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

            foreach(var s in _pageItems)
            {
                grid.Rows.Add(s.studentName, s.birthDate, s.phoneNumber, s.eduId, DormLabel(s.dormYn));
            }
            grid.ResumeLayout();
        }

        // ===== CRUD =====
        /// <summary>등록 버튼 Click 이벤트에 연결 (디자이너에서 AppButton 추가 후 연결)</summary>
        protected async void OnCreate(object? sender, EventArgs e)
        {
            var created = StudentEditModal.Show(this, null);
            if (created == null) return;

            var res = await _adminStudentController.InsertStudent(created);
            if (res?.Status == 200)
            {
                await LoadAndRender();
                RenderPage(int.MaxValue);
            }
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                var edited = StudentEditModal.Show(this, target);
                if (edited == null) return;

                var res = await _adminStudentController.UpdateStudent(target.studentId, edited);
                if (res?.Status == 200)
                {
                    var idx = _all.IndexOf(target);
                    if (idx >= 0) _all[idx] = edited;
                    RenderPage(pagination1.PageIndex);
                }
            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(this, "삭제 확인", $"'{target.studentName}'을(를) 삭제할까요?"))
                    return;

                var res = await _adminStudentController.DeleteStudent(target.studentId);
                if (res?.Status == 200)
                {
                    _all.Remove(target);
                    RenderPage(pagination1.PageIndex);
                }
            }
        }

        // ===== 헬퍼 =====
        private static string DormLabel(string? dormYn) =>
            string.Equals(dormYn, "Y", StringComparison.OrdinalIgnoreCase) ? "배정" : "미배정";

        // ===== 엑셀 업로드  =====
        private async void btnExcel_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "엑셀 파일 선택",
                Filter = "Excel 파일 (*.xlsx) | *.xlsx"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            List<StudentDto> students;

            try
            {
                students = ParseExcel(dialog.FileName);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"파일 파싱 오류 : {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(students.Count == 0)
            {
                MessageBox.Show("등록할 데이터가 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var res = await _adminStudentController.BatchInsertStudent(students);

            if(res?.Status == 200)
            {
                MessageBox.Show($"{students.Count} 명이 등록되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadAndRender();
            }
        }

        private List<StudentDto> ParseExcel(string filePath)
        {
            var result = new List<StudentDto>();
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);

            for(int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                if(row == null)
                {
                    continue;
                }

                var studentName = row.GetCell(0)?.ToString()?.Trim();
                var birthDate = row.GetCell(1)?.ToString()?.Trim();
                var phoneNumber = row.GetCell(2)?.ToString()?.Trim().Replace("-", "");
                var eduId = row.GetCell(3)?.ToString()?.Trim();
                var dormYn = row.GetCell(4)?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(studentName) || string.IsNullOrWhiteSpace(birthDate))
                {
                    throw new Exception($"{i + 1} 행 : 이름 또는 생년월일이 비어있습니다.");
                }
                result.Add(new StudentDto
                {
                    studentName = studentName,
                    birthDate = birthDate,
                    phoneNumber = phoneNumber ?? "",
                    eduId = eduId ?? "",
                    dormYn = string.IsNullOrWhiteSpace(dormYn) ? "N" : dormYn.ToUpper(),
                });
            }
            return result;
        }

        // ===== 필터링 =====
        private void SetupFilterSource()
        {
            var eduList = new List<EduInfoDto>
            {
                new()
                {
                    eduId = "",
                    eduName = "전체"
                }
            };

            eduList.AddRange(_eduInfos);
            cmbEdu.DataSource = eduList;
            cmbEdu.DisplayMember = "eduName";
            cmbEdu.ValueMember = "eduId";

            var batchList = new[]
            {
                new
                {
                    Value = 0,
                    Label = "전체"
                }
            }.Concat(_eduInfos.Select(e => e.batchNumber).Distinct().OrderBy(b => b)
            .Select(b => new
            {
                Value = b,
                Label = $"{b}기"
            })).ToList();

            cmbBatch.DataSource = batchList;
            cmbBatch.DisplayMember = "Label";
            cmbBatch.ValueMember = "Value";
        }

        private void ApplyFilter()
        {
            var result = _all.AsEnumerable();

            var eduId = cmbEdu.SelectedValue?.ToString();

            if(!string.IsNullOrEmpty(eduId))
            {
                result = result.Where(s => s.eduId == eduId);
            }

            if(cmbBatch.SelectedValue is int batch && batch > 0)
            {
                var matchIds = _eduInfos.Where(e => e.batchNumber == batch)
                                        .Select(e => e.eduId).ToHashSet();
                result = result.Where(s => matchIds.Contains(s.eduId ?? ""));
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
                result = result.Where(s => s.studentName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

            _filtered = result.ToList();
        }
    }
}
