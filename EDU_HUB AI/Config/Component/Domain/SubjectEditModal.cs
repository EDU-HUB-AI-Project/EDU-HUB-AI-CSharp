using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    /// <summary>과목 등록/수정 모달 — AppModal 기반.</summary>
    public class SubjectEditModal : AppModal
    {
        private readonly SubjectDto? _source;
        private readonly TextField _txtSubjectName;
        private readonly DateField _dtpStartDate;
        private readonly DateField _dtpEndDate;
        private readonly ToggleSwitch _togEndYn;
        private readonly ComboField _cmbEdu;
        private readonly ComboField _cmbClassroom;

        private readonly AdminClassroomController _classroomController = new AdminClassroomController();
        private readonly AdminEduInfoController _eduInfoController = new AdminEduInfoController();

        public SubjectDto? Result { get; private set; }

        public SubjectEditModal(SubjectDto? source)
        {
            _source = source;
            ModalTitle = source == null ? "과목 등록" : "과목 수정";
            ConfirmText = "저장";

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for(int i = 0; i < 6; i++)
            {
                stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

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
                Margin = new Padding(0, 0, 0, 14)
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
                Margin = new Padding(0, 0, 0, 14)
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

            stack.Controls.Add(_txtSubjectName, 0, 0);
            stack.Controls.Add(_cmbEdu, 0, 1);
            stack.Controls.Add(_cmbClassroom, 0, 2);
            stack.Controls.Add(_dtpStartDate, 0, 3);
            stack.Controls.Add(_dtpEndDate, 0, 4);
            stack.Controls.Add(_togEndYn, 0, 5);

            Body.Controls.Add(stack);
            SetCardWidth(500);
        }

        // ComboBox 등 초기값 로딩
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

            if(_source?.eduId != null && list.Any(e => e.eduId == _source.eduId))
            {
                _cmbEdu.SelectedValue = _source.eduId;
            }
        }

        private async Task LoadClassrooms()
        {
            var res = await _classroomController.GetClassrooms();
            var list = res?.Data ?? new List<ClassroomDto>();

            _cmbClassroom.DataSource = list;
            _cmbClassroom.DisplayMember = "classroomName";
            _cmbClassroom.ValueMember = "classroomId";

            if(_source?.classroomId != null && list.Any(c => c.classroomId == _source.classroomId))
            {
                _cmbClassroom.SelectedValue = _source.classroomId;
            }
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


            Result = _source != null ? CopyOf(_source) : new SubjectDto();
            Result.subjectName = subjectName;
            Result.eduId = eduId;
            Result.classroomId = classroomId;
            Result.startDate = startDate;
            Result.endDate = endDate;
            Result.endYn = _togEndYn.Checked ? "Y" : "N";
            base.OnConfirm();
        }

        // ====== 리턴 ======
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
