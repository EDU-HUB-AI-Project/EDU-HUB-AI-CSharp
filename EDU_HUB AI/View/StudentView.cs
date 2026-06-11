using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System.Data;

namespace EDU_HUB_AI.View
{
    public partial class StudentView : UserControl
    {
        private List<StudentDto> _all = new();
        private List<StudentDto> _pageItems = new();
        private AdminStudentController _adminStudentController = new AdminStudentController();

        private List<EduInfoDto> _eduInfos = new();
        private List<StudentDto> _filtered = new();
        private readonly AdminEduInfoController _adminEduInfoController = new();

        private static readonly string[] PhonePrefixes = { "010", "011" };
        private bool _suppressFilter = false;

        public StudentView()
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

            cmbEdu.SelectedIndexChanged += (_, _) =>
            {
                if (!_suppressFilter)
                {
                    UpdateBatchSource(cmbEdu.SelectedValue?.ToString());
                    ApplyFilter();
                    RenderPage(1);
                }
            };
            cmbBatch.SelectedIndexChanged += (_, _) => { if (!_suppressFilter) { ApplyFilter(); RenderPage(1); } };
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
        private async Task<List<StudentDto>> LoadData()
        {
            var res = await _adminStudentController.GetStudents();
            return res?.Data ?? new List<StudentDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩 중...") : null;
            _adminStudentController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

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
                var prevBatch = cmbBatch.SelectedValue is int b ? b : 0;

                _suppressFilter = true;
                SetupFilterSource();
                if (!string.IsNullOrEmpty(prevEduId))
                {
                    cmbEdu.SelectedValue = prevEduId;
                    UpdateBatchSource(prevEduId);
                }
                if (prevBatch > 0)
                    cmbBatch.SelectedValue = prevBatch;
                _suppressFilter = false;

                ApplyFilter();
                RenderPage(page);
            }
            finally
            {
                _adminStudentController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        // ===== 그리드 =====
        private void SetupGrid()
        {
            grid.AutoGenerateColumns = false;

            grid.Columns.Add("name", "이름");
            grid.Columns.Add("birth", "생년월일");
            grid.Columns.Add("phone", "연락처");
            grid.Columns.Add("edu", "과정");
            grid.Columns.Add("batch", "기수");
            grid.Columns.Add("dorm", "생활관");
            grid.AddTextActionColumns();

            grid.Columns["name"].FillWeight = 130;
            grid.Columns["birth"].FillWeight = 110;
            grid.Columns["phone"].FillWeight = 130;
            grid.Columns["edu"].FillWeight = 280;
            grid.Columns["batch"].FillWeight = 70;
            grid.Columns["dorm"].FillWeight = 90;

            grid.ActionClicked += OnRowAction;
            grid.CellFormatting += OnCellFormatting;
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
                var edu = _eduInfos.FirstOrDefault(e => e.eduId == s.eduId);
                var eduName = edu?.eduName ?? s.eduId;
                var batchLabel = edu?.batchNumber is > 0 ? $"{edu.batchNumber}기" : "-";
                grid.Rows.Add(s.studentName, s.birthDate, s.phoneNumber, eduName, batchLabel, DormLabel(s.dormYn));
            }
            grid.ResumeLayout();
        }

        // ===== CRUD =====
        protected async void OnCreate(object? sender, EventArgs e)
        {
            var created = StudentEditModal.Show(this.FindForm(), null);
            if (created == null) return;

            var overlay = LoadingOverlay.Create(bodyPanel, "등록 중...");
            _adminStudentController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중... \n재시도 {attempt}/{max}");

            try
            {
                var res = await _adminStudentController.InsertStudent(created);
                if (res?.Status == 200)
                    await LoadAndRender(int.MaxValue, showOverlay: false);
                else
                    MessageBox.Show(this.FindForm(), res?.Message ?? "등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApiException ex)
            {
                MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminStudentController.OnRetry = null;
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
                var edited = StudentEditModal.Show(this.FindForm(), target);
                if (edited == null) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _adminStudentController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _adminStudentController.UpdateStudent(target.studentId, edited);
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
                finally { _adminStudentController.OnRetry = null; overlay.Close(); overlay.Dispose(); }
            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(this.FindForm(), "삭제 확인", $"'{target.studentName}'을(를) 삭제할까요?"))
                    return;

                var overlay = LoadingOverlay.Create(bodyPanel, "삭제 중...");
                _adminStudentController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _adminStudentController.DeleteStudent(target.studentId);
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
                finally { _adminStudentController.OnRetry = null; overlay.Close(); overlay.Dispose(); }
            }
        }

        // ===== 헬퍼 =====
        private static string DormLabel(string? dormYn) =>
            string.Equals(dormYn, "Y", StringComparison.OrdinalIgnoreCase) ? "배정" : "미배정";

        private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || grid.Columns[e.ColumnIndex].Name != "dorm") return;
            if (e.Value?.ToString() == "배정")
            {
                e.CellStyle.ForeColor = ThemeColors.OkText;
                e.CellStyle.BackColor = ThemeColors.OkBg;
                e.CellStyle.SelectionForeColor = ThemeColors.OkText;
                e.CellStyle.SelectionBackColor = ThemeColors.OkBg;
            }
            else
            {
                e.CellStyle.ForeColor = ThemeColors.TextMuted;
                e.CellStyle.SelectionForeColor = ThemeColors.TextMuted;
            }
            e.FormattingApplied = true;
        }

        // ===== 엑셀 업로드 =====
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
                students = ImportStudentsFromExcel(dialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.FindForm(), $"파일 파싱 오류 : {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (students.Count == 0)
            {
                MessageBox.Show(this.FindForm(), "등록할 데이터가 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var overlay = LoadingOverlay.Create(bodyPanel, "업로드 중...");
            _adminStudentController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                var res = await _adminStudentController.BatchInsertStudent(students);
                if (res?.Status == 200)
                {
                    MessageBox.Show(this.FindForm(), $"{students.Count} 명이 등록되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadAndRender(int.MaxValue, showOverlay: false);
                }
                else
                    MessageBox.Show(this.FindForm(), res?.Message ?? "일괄 등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApiException ex) { MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception ex) { MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { _adminStudentController.OnRetry = null; overlay.Close(); overlay.Dispose(); }
        }

        private static List<StudentDto> ImportStudentsFromExcel(string filePath)
        {
            var dt = new ExcelImport().ExcelImporter(filePath);
            var errors = new List<string>();
            var result = new List<StudentDto>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try { result.Add(ToStudentDto(dt.Rows[i], i + 2)); }
                catch (Exception ex) { errors.Add(ex.Message); }
            }

            if (errors.Count > 0)
                throw new Exception(string.Join("\n", errors));

            return result;
        }

        private static StudentDto ToStudentDto(DataRow row, int rowNum)
        {
            var name = row[0]?.ToString()?.Trim();
            var rawBirth = row[1]?.ToString()?.Trim();
            var phone = row[2]?.ToString()?.Trim().Replace("-", "").Replace(" ", "");
            var eduId = row[3]?.ToString()?.Trim();
            var dormYn = row[4]?.ToString()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(name))
                throw new Exception($"{rowNum}행 : 이름이 비어있습니다.");

            var birthDate = string.IsNullOrWhiteSpace(rawBirth) ? null : DateHelper.NormalizeBirthDate(rawBirth);
            if (birthDate == null)
                throw new Exception($"{rowNum}행 : 생년월일 형식이 올바르지 않습니다. (예: 2000-01-01 / 000101)");

            if (!string.IsNullOrEmpty(phone) && (!phone.All(char.IsDigit) || phone.Length != 11 || !PhonePrefixes.Any(p => phone.StartsWith(p))))
                throw new Exception($"{rowNum}행 : 유효하지 않은 연락처입니다. (예: 010-1234-5678)");

            dormYn = string.IsNullOrWhiteSpace(dormYn) ? "N" : dormYn;
            if (dormYn != "Y" && dormYn != "N")
                throw new Exception($"{rowNum}행 : 생활관 값은 Y 또는 N만 입력 가능합니다.");

            return new StudentDto
            {
                studentName = name,
                birthDate = birthDate,
                phoneNumber = phone ?? "",
                eduId = eduId ?? "",
                dormYn = dormYn,
            };
        }

        // ===== 필터링 =====
        private void SetupFilterSource()
        {
            var eduList = new List<EduInfoDto> { new() { eduId = "", eduName = "전체" } };
            eduList.AddRange(_eduInfos);
            cmbEdu.DataSource = eduList;
            cmbEdu.DisplayMember = "eduName";
            cmbEdu.ValueMember = "eduId";
            UpdateBatchSource("");
        }

        private void UpdateBatchSource(string? eduId)
        {
            var source = string.IsNullOrEmpty(eduId) ? _eduInfos : _eduInfos.Where(e => e.eduId == eduId).ToList();
            var batchList = new[] { new { Value = 0, Label = "전체" } }
                .Concat(source.Select(e => e.batchNumber).Distinct().OrderBy(b => b)
                    .Select(b => new { Value = b, Label = $"{b}기" }))
                .ToList();

            _suppressFilter = true;
            cmbBatch.DataSource = batchList;
            cmbBatch.DisplayMember = "Label";
            cmbBatch.ValueMember = "Value";
            _suppressFilter = false;
        }

        private void ApplyFilter()
        {
            var result = _all.AsEnumerable();

            var eduId = cmbEdu.SelectedValue?.ToString();
            if (!string.IsNullOrEmpty(eduId))
                result = result.Where(s => s.eduId == eduId);

            if (cmbBatch.SelectedValue is int batch && batch > 0)
            {
                var matchIds = _eduInfos.Where(e => e.batchNumber == batch).Select(e => e.eduId).ToHashSet();
                result = result.Where(s => matchIds.Contains(s.eduId ?? ""));
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
                result = result.Where(s => s.studentName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

            _filtered = result.ToList();
        }
    }
}