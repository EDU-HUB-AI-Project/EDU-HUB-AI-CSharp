using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;
using System.Diagnostics;

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
    public partial class TableTemplateForm : Form
    {
        private List<StudentDto> _all = new();
        private List<StudentDto> _pageItems = new();

        public TableTemplateForm()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;
            FixDockOrder();

            pageHeader1.SyncClicked += (_, _) => LoadAndRender();
            btnCreate.Click += OnCreate;
            pagination1.PageChanged += (_, page) => RenderPage(page);

            navigation1.ActiveMenu = MenuKey.Trainees; // 현재 메뉴 활성화
            navigation1.MenuSelected += NavigateTo;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            LoadAndRender();
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(grid, 0);
            bodyPanel.Controls.SetChildIndex(actionPanel, 1);
            bodyPanel.Controls.SetChildIndex(pagination1, 2);
        }

        // ===== 데이터 연동 지점 (여기만 바꾸면 됨) =====
        private async Task<List<StudentDto>> LoadData()
        {
            // [실제 API] 아래 두 줄 주석을 풀고 목업 return 을 지우기
            //   var res = await new AdminStudentController().GetStudents();
            //   return res?.Data ?? new List<StudentDto>();

            await Task.CompletedTask; // 목업: 비동기 흉내
            return MockStudents();
        }

        private async void LoadAndRender()
        {
            _all = await LoadData();
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
            pagination1.TotalCount = _all.Count;
            var size = pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_all.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination1.PageIndex = page;

            _pageItems = _all.Skip((page - 1) * size).Take(size).ToList();

            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach (var s in _pageItems)
                grid.Rows.Add(s.studentName, s.birthDate, s.phoneNumber, s.eduId, DormLabel(s.dormYn));
            grid.ResumeLayout();
        }

        // ===== CRUD =====
        /// <summary>등록 버튼 Click 이벤트에 연결 (디자이너에서 AppButton 추가 후 연결)</summary>
        protected void OnCreate(object? sender, EventArgs e)
        {
            var created = StudentEditModal.Show(this, null);
            if (created == null) return;

            // TODO: API 등록 — await new AdminStudentController().InsertStudent(created);
            if (string.IsNullOrWhiteSpace(created.studentId))
                created.studentId = Guid.NewGuid().ToString("N")[..8];
            _all.Add(created);
            RenderPage(int.MaxValue); // 마지막 페이지로 이동해 추가된 행 표시
        }

        private void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                var edited = StudentEditModal.Show(this, target);
                if (edited == null) return;

                // TODO: API 수정 — await new AdminStudentController().UpdateStudent(target.studentId, edited);
                var idx = _all.IndexOf(target);
                if (idx >= 0) _all[idx] = edited;
                RenderPage(pagination1.PageIndex);
            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(this, "삭제 확인", $"'{target.studentName}'을(를) 삭제할까요?"))
                    return;

                // TODO: API 삭제 — await new AdminStudentController().DeleteStudent(target.studentId);
                _all.Remove(target);
                RenderPage(pagination1.PageIndex);
            }
        }

        // ===== 헬퍼 =====
        private static string DormLabel(string? dormYn) =>
            string.Equals(dormYn, "Y", StringComparison.OrdinalIgnoreCase) ? "배정" : "미배정";

        private static List<StudentDto> MockStudents()
        {
            var names = new[] { "김민수", "이지은", "박서준", "최유나", "정해인", "한소희", "오정세", "윤아름",
                "장도윤", "임수정", "강하늘", "신예은", "백승호", "문가영", "노태현", "서지수",
                "권나라", "유재석", "송지효", "전소민", "양세찬", "지석진", "하동훈" };
            var courses = new[] { "AI 실무", "클라우드 기초", "백엔드 심화", "데이터 분석" };
            var list = new List<StudentDto>();
            for (var i = 0; i < names.Length; i++)
            {
                list.Add(new StudentDto
                {
                    studentId = (i + 1).ToString("D4"),
                    studentName = names[i],
                    birthDate = $"19{90 + (i % 10):D2}-{(i % 12) + 1:D2}-{(i % 27) + 1:D2}",
                    phoneNumber = $"010-{1000 + i:D4}-{5000 + i:D4}",
                    eduId = courses[i % courses.Length],
                    dormYn = i % 2 == 0 ? "Y" : "N"
                });
            }
            return list;
        }

        private void ehButton１_Click(object sender, EventArgs e)
        {

        }
        private void NavigateTo(object? sender, MenuKey key)
        {
            switch (key)
            {
                case MenuKey.Trainees:
                    break;
                case MenuKey.Attendance:
                    //new AttendForm().Show();
                    break;
                case MenuKey.Dormitory:
                    // new DormitoryForm().Show();
                    // this.Close();
                    break;
                case MenuKey.Facilities:
                    
                    break;
            }
        }
    }
}
