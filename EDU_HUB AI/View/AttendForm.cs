using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using Microsoft.VisualBasic;
using NPOI.HSSF.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDU_HUB_AI.View
{
    public partial class AttendForm : Form
    {
        private List<AttendDto> _all = new();
        private List<AttendDto> _pageItems = new();
        private List<AttendDto> _listAttend = new();
        private DataTable _dtAttend = new DataTable();
        private readonly ExcelExport excelExport = new ExcelExport();
        private readonly ExcelImport excelImport = new ExcelImport();

        public AttendForm()
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
            btnSearch.Click += BtnSearch_Click;
            btnExport.Click += BtnExport_Click;
            btnImport.Click += BtnImport_Click;

            navigation1.ActiveMenu = MenuKey.Attendance;
            navigation1.MenuSelected += NavigateTo;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            LoadCmbStatus();
            await LoadCmb();
            LoadAndRender();
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(grid, 0);
            bodyPanel.Controls.SetChildIndex(actionPanel, 1);
            bodyPanel.Controls.SetChildIndex(pagination1, 2);
        }

        // ===== 데이터 연동 지점 (여기만 바꾸면 됨) =====
        private async Task<List<AttendDto>> LoadData()
        {
            // [실제 API] 아래 두 줄 주석을 풀고 목업 return 을 지우기
            string? studentId = cmbStudent.SelectedIndex > 0 ? cmbStudent.SelectedValue.ToString() : null;
            string? eduId = cmbEdu.SelectedIndex > 0 ? cmbEdu.SelectedValue.ToString() : null;
            string? attendDate = dtpDate.Checked ? dtpDate.Value.ToString("yyyy-MM-dd") : null; ;
            string? status = cmbStatus.SelectedIndex > 0 ? cmbStatus.SelectedItem.ToString() : null;
            var res = await new AdminAttendaceController().GetAttend(studentId, eduId, attendDate, status);
            return res?.Data ?? new List<AttendDto>();
        }

        private async void LoadAndRender()
        {
            _all = await LoadData();
            RenderPage(1);
        }

        // ===== 그리드 =====
        private void SetupGrid()
        {
            grid.Columns.Add("studentName", "이름");
            grid.Columns.Add("eduName", "교육과정명");
            grid.Columns.Add("attendDate", "해당일자");
            grid.Columns.Add("status", "상태");
            grid.Columns.Add("message", "사유");
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
            foreach (var a in _pageItems)
                grid.Rows.Add(a.studentName, a.eduName, a.attendDate, a.status, a.message);
            grid.ResumeLayout();

            ConvertToTable(_all);
        }

        // ===== CRUD =====
        /// <summary>등록 버튼 Click 이벤트에 연결 (디자이너에서 AppButton 추가 후 연결)</summary>
        protected async void OnCreate(object? sender, EventArgs e)
        {
            var created = AttendEditModal.Show(this, null);
            if (created == null) return;

            // TODO: API 등록 — await new AdminStudentController().InsertStudent(created);
            await new AdminAttendaceController().InsertAttend(created);
            if (string.IsNullOrWhiteSpace(created.attendanceId))
                created.attendanceId = Guid.NewGuid().ToString("N")[..8];
            _all.Add(created);
            RenderPage(int.MaxValue); // 마지막 페이지로 이동해 추가된 행 표시
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                var edited = AttendEditModal.Show(this, target);
                if (edited == null) return;

                // TODO: API 수정 — await new AdminStudentController().UpdateStudent(target.studentId, edited);
                await new AdminAttendaceController().UpdateAttendMsg(target.studentId, edited);
                var idx = _all.IndexOf(target);
                if (idx >= 0) _all[idx] = edited;
                RenderPage(pagination1.PageIndex);
            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(this, "삭제 확인", $"'{target.studentName}'을(를) 삭제할까요?"))
                    return;

                // TODO: API 삭제 — await new AdminStudentController().DeleteStudent(target.studentId);
                await new AdminAttendaceController().DeleteAttend(target.attendanceId);
                _all.Remove(target);
                RenderPage(pagination1.PageIndex);
            }
        }

        private void ConvertToTable(List<AttendDto> list)
        {
            _dtAttend = new DataTable();
            // dgv의 칼럼이름 사용
            foreach (DataGridViewColumn col in grid.Columns)
            {
                _dtAttend.Columns.Add(col.HeaderText);
            }
            // dt에 row 추가 이때 dgv순서에 맞게 매핑
            foreach (var items in list)
            {
                _dtAttend.Rows.Add(items.studentName, items.eduName, items.attendDate,
                                    items.status, items.message);
            }
        }

        private void LoadCmbStatus()
        {
            cmbStatus.Items.Add("전체");
            cmbStatus.Items.Add("출석");
            cmbStatus.Items.Add("결석");
            cmbStatus.Items.Add("지각");
            cmbStatus.Items.Add("조퇴");
            cmbStatus.SelectedIndex = 0;
        }
        // studentId, eduId에 콤보박스 추가
        private async Task LoadCmb()
        {
            var response = await new AdminAttendaceController().GetAttend(null, null, null, null);
            if (response?.Status == 200)
            {
                // 출석 전체조회 응답 데이터를 활용하여 콤보박스 목록을 구성
                // 별도 API 호출 없이 LINQ로 중복 제거 후 추출
                // 학생 콤보박스
                var students = response.Data
                    .Select(x => new { x.studentId, x.studentName })
                    .DistinctBy(x => x.studentId)
                    .ToList();
                students.Insert(0, new { studentId = "", studentName = "전체" });
                cmbStudent.DataSource = students;
                cmbStudent.DisplayMember = "studentName";
                cmbStudent.ValueMember = "studentId";

                // 교육과정 콤보박스
                var edus = response.Data
                    .Select(x => new { x.eduId, x.eduName })
                    .DistinctBy(x => x.eduId)
                    .ToList();

                edus.Insert(0, new { eduId = "", eduName = "전체" });
                cmbEdu.DataSource = edus;
                cmbEdu.DisplayMember = "eduName";
                cmbEdu.ValueMember = "eduId";
            }
        }

        // 버튼 이벤트 
        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            LoadAndRender();
        }
        // 엑셀로 내보내기
        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files(*.xlsx) | *.xlsx";
                saveFileDialog.DefaultExt = "xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    excelExport.ExcelExporter(_dtAttend, filePath);
                }
            }
        }
        // 엑셀에서 저장하기
        private async void BtnImport_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files(*.xlsx) | *.xlsx";
                openFileDialog.DefaultExt = "xlsx";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = openFileDialog.FileName;
                        DataTable dt = excelImport.ExcelImporter(filePath);
                        List<AttendDto> list = new List<AttendDto>();
                        foreach (DataRow row in dt.Rows)
                        {
                            list.Add(new AttendDto
                            {
                                studentId = row[0]?.ToString(),
                                attendDate = row[1]?.ToString(),
                                status = row[2]?.ToString(),
                                message = row[3]?.ToString()
                            });
                        }
                        var response = await new AdminAttendaceController().InsertAttendList(list);
                        if (response?.Status == 200)
                        {
                            LoadAndRender();
                            MessageBox.Show("저장되었습니다.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"오류가 발생했습니다.\n{ex.Message}");
                    }
                }
            }
        }
        private void NavigateTo(object? sender, MenuKey key)
        {
            switch (key)
            {
                case MenuKey.Trainees:
                    new TableTemplateForm().Show();
                    this.Close();
                    break;
                case MenuKey.Attendance:
                    break; // 현재 화면이므로 무시
                case MenuKey.Dormitory:
                    // new DormitoryForm().Show();
                    // this.Close();
                    break;
            }
        }

        private void actionPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
