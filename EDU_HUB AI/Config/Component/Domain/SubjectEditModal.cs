using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public class SubjectEditModal : AppModal
    {
        private readonly SubjectDto? _source;
        private readonly TextField _txtSubjectName;
        private readonly DateField _dtpStartDate;
        private readonly DateField _dtpEndDate;
        private readonly ToggleSwitch _togEndYn;
        private readonly ComboField _cmbEdu;
        private readonly ComboField _cmbClassroom;

        private readonly AdminClassroomController _classroomController = new();
        private readonly AdminEduInfoController _eduInfoController = new();

        public SubjectDto? Result { get; private set; }

        public SubjectEditModal(SubjectDto? source)
        {
            _source = source;
            ModalTitle = source == null ? "과목 등록" : "과목 수정";
            ConfirmText = "저장";

            _txtSubjectName = new TextField
            {
                FieldLabel = "과목명",
                Text = source?.subjectName ?? "",
                Placeholder = "과목명 입력",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            _cmbEdu = new ComboField
            {
                FieldLabel = "교육과정",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 14)
            };
            _cmbClassroom = new ComboField
            {
                FieldLabel = "강의실",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            _dtpStartDate = new DateField
            {
                FieldLabel = "시작일",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 14)
            };
            _dtpEndDate = new DateField
            {
                FieldLabel = "종료일",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };
            _dtpStartDate.SetYyMMdd(source?.startDate);
            _dtpEndDate.SetYyMMdd(source?.endDate);

            _togEndYn = new ToggleSwitch
            {
                FieldLabel = "종료 여부",
                InlineLabel = "종료됨",
                Checked = source?.endYn == "Y",
                Margin = new Padding(0, 0, 0, 4)
            };

            // 교육과정 | 강의실
            var rowCombo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Margin = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            rowCombo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowCombo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowCombo.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rowCombo.Controls.Add(_cmbEdu, 0, 0);
            rowCombo.Controls.Add(_cmbClassroom, 1, 0);

            // 시작일 | 종료일
            var rowDate = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Margin = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            rowDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowDate.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rowDate.Controls.Add(_dtpStartDate, 0, 0);
            rowDate.Controls.Add(_dtpEndDate, 1, 0);

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.Controls.Add(_txtSubjectName, 0, 0);
            stack.Controls.Add(rowCombo, 0, 1);
            stack.Controls.Add(rowDate, 0, 2);
            stack.Controls.Add(_togEndYn, 0, 3);

            Body.Controls.Add(stack);
            SetCardWidth(500);

            _txtSubjectName.Required = true;
            _txtSubjectName.TextChanged += (_, _) => _txtSubjectName.HasError = false;
            _cmbEdu.Required = true;
            _cmbClassroom.Required = true;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadEduInfos();
            await LoadClassrooms();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            FitCardSize();
        }

        private async Task LoadEduInfos()
        {
            var res = await _eduInfoController.GetEduInfos();
            var list = res?.Data ?? new List<EduInfoDto>();

            _cmbEdu.DataSource = list;
            _cmbEdu.DisplayMember = "eduName";
            _cmbEdu.ValueMember = "eduId";

            if (_source?.eduId != null && list.Any(e => e.eduId == _source.eduId))
                _cmbEdu.SelectedValue = _source.eduId;
        }

        private async Task LoadClassrooms()
        {
            var res = await _classroomController.GetClassrooms();
            var list = res?.Data ?? new List<ClassroomDto>();

            _cmbClassroom.DataSource = list;
            _cmbClassroom.DisplayMember = "classroomName";
            _cmbClassroom.ValueMember = "classroomId";

            if (_source?.classroomId != null && list.Any(c => c.classroomId == _source.classroomId))
                _cmbClassroom.SelectedValue = _source.classroomId;
        }

        public static SubjectDto? Show(IWin32Window owner, SubjectDto? source)
        {
            using var modal = new SubjectEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            var subjectName = _txtSubjectName.Text.Trim();
            var eduId = _cmbEdu.SelectedValue?.ToString();
            var classroomId = _cmbClassroom.SelectedValue?.ToString();
            var startDate = _dtpStartDate.ToYyMMdd();
            var endDate = _dtpEndDate.ToYyMMdd();

            if (string.IsNullOrWhiteSpace(subjectName))
            {
                _txtSubjectName.HasError = true;
                MessageBox.Show("과목명을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(subjectName, @"^[가-힣a-zA-Z0-9\s]+$") || subjectName.Length < 2)
            {
                _txtSubjectName.HasError = true;
                MessageBox.Show("과목명은 한글·영문·숫자만 입력 가능하며, 최소 2자 이상이어야 합니다.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _txtSubjectName.HasError = false;

            if (string.IsNullOrEmpty(eduId))
            {
                MessageBox.Show("교육과정을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(classroomId))
            {
                MessageBox.Show("강의실을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.Compare(endDate, startDate) < 0)
            {
                MessageBox.Show("종료일은 시작일보다 앞설 수 없습니다.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isEdit = _source != null;
            if (!ConfirmModal.Show(Owner,
                isEdit ? "수정 확인" : "등록 확인",
                isEdit ? "수정하시겠습니까?" : "등록하시겠습니까?",
                isEdit ? "수정" : "등록",
                ButtonVariant.Primary))
                return;

            Result = _source != null ? CopyOf(_source) : new SubjectDto();
            Result.subjectName = subjectName;
            Result.eduId = eduId;
            Result.classroomId = classroomId;
            Result.startDate = startDate;
            Result.endDate = endDate;
            Result.endYn = _togEndYn.Checked ? "Y" : "N";
            base.OnConfirm();
        }

        private static SubjectDto CopyOf(SubjectDto s) => new()
        {
            subjectId = s.subjectId,
            subjectName = s.subjectName,
            eduId = s.eduId,
            classroomId = s.classroomId,
            startDate = s.startDate,
            endDate = s.endDate,
            endYn = s.endYn,
            delYn = s.delYn,
            createdAt = s.createdAt,
            updatedAt = s.updatedAt
        };
    }
}