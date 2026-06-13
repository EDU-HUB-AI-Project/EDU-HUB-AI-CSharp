using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public partial class AttendEditModal : AppModal
    {
        private readonly AttendDto? _source;
        //private readonly TextField _txtStudentId;
        private readonly ComboField _cmbStudent;
        private readonly ComboField _cmbEduId;
        private readonly DateField _dtpAttendDate;
        private readonly ComboField _cmbStatus;
        private readonly TextField _txtMessage;

        private readonly AdminEduInfoController _eduInfoController = new AdminEduInfoController();
        private readonly AdminStudentController _adminStudentController = new AdminStudentController();

        public AttendDto? Result { get; private set; }

        public AttendEditModal(AttendDto? source)
        {
            _source = source;
            ModalTitle = source == null ? "출석부 등록" : "출석부 수정";
            ConfirmText = "저장";

            //_txtStudentId = new TextField
            //{
            //    FieldLabel = source == null ? "학생 ID" : "학생",
            //    Text = source == null ? "" : (source.studentName ?? source.studentId ?? ""),
            //    Placeholder = source == null ? "STU_xxxxx" : "",
            //    ReadOnly = source != null,
            //    Dock = DockStyle.Fill,
            //    Margin = new Padding(0, 0, 0, 14)
            //};

            _cmbStudent = new ComboField
            {
                FieldLabel = "학생",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14),
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.CustomSource
            };
            
            _cmbEduId = new ComboField
            {
                FieldLabel = "교육과정",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            _dtpAttendDate = new DateField
            {
                FieldLabel = "출석일자",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            _cmbStatus = new ComboField
            {
                FieldLabel = "출석상태",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };
            _cmbStatus.SelectedIndexChanged += OnStatusChanged;
            _txtMessage = new TextField
            {
                FieldLabel = "사유",
                Text = source?.message ?? "",
                Placeholder = "조퇴, 지각, 결석일 경우 입력",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 5; i++)
                stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            //stack.Controls.Add(_txtStudentId, 0, 0);
            stack.Controls.Add(_cmbStudent, 0, 0);
            stack.Controls.Add(_cmbEduId, 0, 1);
            stack.Controls.Add(_dtpAttendDate, 0, 2);
            stack.Controls.Add(_cmbStatus, 0, 3);
            stack.Controls.Add(_txtMessage, 0, 4);

            Body.Controls.Add(stack);
            SetCardWidth(500);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadCmbStudent();
            await LoadCmb();
            LoadCmbStatus();

            if(_source?.attendDate != null && DateTime.TryParse(_source.attendDate, out var dt))
            {
                _dtpAttendDate.Value = dt;
            }
            // 초기 상태 동기화
            OnStatusChanged(null, EventArgs.Empty);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            FitCardSize();
        }

        private async Task LoadCmb()
        {
            var response = await _eduInfoController.GetEduInfos();
            var list = response?.Data ?? new List<EduInfoDto>();

            _cmbEduId.DataSource = list;
            _cmbEduId.DisplayMember = "eduName";
            _cmbEduId.ValueMember = "eduId";

            if (_source?.eduId != null && list.Any(e => e.eduId == _source.eduId))
            {
                _cmbEduId.SelectedValue = _source.eduId;
            }
        }

        private async Task LoadCmbStudent()
        {
            var response = await _adminStudentController.GetStudents();
            var list = response?.Data ?? new List<StudentDto>();

            var options = list
            .Select(s => new 
                {
                    StudentId = s.studentId,
                    DisplayName = $"{s.studentName}({s.phoneNumber})"
                })
                .ToList();

            _cmbStudent.DataSource = options;
            _cmbStudent.DisplayMember = "DisplayName";
            _cmbStudent.ValueMember = "StudentId";

            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(options.Select(o => o.DisplayName).ToArray());
            _cmbStudent.AutoCompleteCustomSource = autoComplete;

            if (_source != null)
            {
                _cmbStudent.SelectedValue = _source.studentId;
                _cmbStudent.Enabled = false;  // 수정 모드에서는 학생 변경 불가
            }
        }

        private void LoadCmbStatus()
        {
            _cmbStatus.DataSource = new[] { "출석", "결석", "지각", "조퇴" };
            _cmbStatus.SelectedIndex = 0;
            if (_source?.status != null)
                _cmbStatus.SelectedItem = _source.status;
        }
        
        public static AttendDto? Show(IWin32Window owner, AttendDto? source)
        {
            using var modal = new AttendEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            var studentId = _source != null ? _source.studentId : _cmbStudent.SelectedValue?.ToString();
            var eduId = _cmbEduId.SelectedValue?.ToString();
            var status = _cmbStatus.SelectedItem?.ToString();
            var msg = _txtMessage.Text.Trim();

            if(string.IsNullOrWhiteSpace(studentId))
            {
                if(_source == null)
                {
                    _cmbStudent.HasError = true;
                }
                MessageBox.Show("학생 ID를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(_source == null)
            {
                _cmbStudent.HasError = false;
            }

            if (string.IsNullOrEmpty(eduId))
            {
                MessageBox.Show("교육과정을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_dtpAttendDate.Value.Date > DateTime.Today)
            {
                MessageBox.Show("오늘 이후의 날짜는 입력할 수 없습니다.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (status != "출석" && string.IsNullOrWhiteSpace(msg))
            {
                MessageBox.Show("해당하는 사유를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isEdit = _source != null;
            if (!ConfirmModal.Show(Owner,
                isEdit ? "수정 확인" : "등록 확인",
                isEdit ? "수정하시겠습니까?" : "등록하시겠습니까?",
                isEdit ? "수정" : "등록",
                ButtonVariant.Primary))
                return;

            Result = _source != null ? CopyOf(_source) : new AttendDto();
            Result.studentId = studentId;
            Result.eduId = eduId;
            Result.attendDate = _dtpAttendDate.Value.ToString("yyyy-MM-dd");
            Result.status = status;
            Result.message = msg;
            base.OnConfirm();
        }
        // 상태가 출석이 선택된 경우 사유 입력 비활성화
        private void OnStatusChanged(object? sender, EventArgs e)
        {
            bool isPresent = _cmbStatus.SelectedItem?.ToString() == "출석";
            _txtMessage.Enabled = !isPresent;

            if (isPresent)
            {
                _txtMessage.Text = "";   // 출석으로 바꾸면 기존 사유도 비워줌
            }
        }

        private static AttendDto CopyOf(AttendDto s) => new()
        {
            attendanceId = s.attendanceId,
            studentId = s.studentId,
            studentName = s.studentName,
            phone = s.phone,
            eduId = s.eduId,
            eduName = s.eduName,
            status = s.status,
            message = s.message,
            attendDate = s.attendDate,
            createdAt = s.createdAt,
            updatedAt = s.updatedAt
        };
    }
}
