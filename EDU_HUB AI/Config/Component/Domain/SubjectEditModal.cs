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
        private readonly DateTimePicker _dtpStartDate;
        private readonly DateTimePicker _dtpEndDate;
        private readonly CheckBox _chkEndYn;

        private readonly ComboBox _cmbEdu;
        private readonly ComboBox _cmbClassroom;

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

            _txtSubjectName = AddField(stack, "과목명", source?.subjectName, "과목명 입력", 0);
            _cmbEdu = AddComboField(stack, "교육과정", 1);
            _cmbClassroom = AddComboField(stack, "강의실", 2);
            _dtpStartDate = AddDateField(stack, "시작일", source?.startDate, 3);
            _dtpEndDate = AddDateField(stack, "종료일", source?.endDate, 4);
            _chkEndYn = AddCheckField(stack, "종료여부", source?.endYn == "Y", 5);

            Body.Controls.Add(stack);
        }

        // ComboBox 등 초기값 로딩
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadEduInfos();
            await LoadClassrooms();
        }

        private async Task LoadEduInfos()
        {
            var res = await _eduInfoController.GetEduInfos();
            var list = res?.Data ?? new List<EduInfoDto>();

            _cmbEdu.DataSource = list;
            _cmbEdu.DisplayMember = "eduName";
            _cmbEdu.ValueMember = "eduId";

            if (_source?.eduId != null)
            {
                var match = list.FirstOrDefault(e => e.eduId == _source.eduId);
                if (match != null)
                {
                    _cmbEdu.SelectedValue = match.eduId;
                }
            }
        }

        private async Task LoadClassrooms()
        {
            var res = await _classroomController.GetClassrooms();
            var list = res?.Data ?? new List<ClassroomDto>();

            _cmbClassroom.DataSource = list;
            _cmbClassroom.DisplayMember = "classroomName";
            _cmbClassroom.ValueMember = "classroomId";

            if(_source?.classroomId != null)
            {
                var match = list.FirstOrDefault(c => c.classroomId == _source.classroomId);
                if(match != null)
                {
                    _cmbClassroom.SelectedValue = match.classroomId;
                }
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
            var startDate = _dtpStartDate.Value.ToString("yyMMdd");
            var endDate = _dtpEndDate.Value.ToString("yyMMdd");

            if (string.IsNullOrWhiteSpace(subjectName))
            {
                MessageBox.Show("과목이름 을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(string.IsNullOrEmpty(eduId))
            {
                MessageBox.Show("교육과정을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(string.IsNullOrEmpty(classroomId))
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
            Result.endYn = _chkEndYn.Checked ? "Y" : "N";
            base.OnConfirm();
        }

        // ====== UI 헬퍼 ======
        private ComboBox AddComboField(TableLayoutPanel parent, string label, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 14)
            };

            var lbl = new Label
            {
                Text = label,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Dock = DockStyle.Top
            };

            var cmb = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = ThemeFonts.Body,
                BackColor = ThemeColors.Surface
            };

            panel.Controls.Add(cmb);
            panel.Controls.Add(lbl);
            parent.Controls.Add(panel, 0, row);
            return cmb;
        }

        private static TextField AddField(TableLayoutPanel parent, string label, string? value, string placeholder, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var field = new TextField
            {
                FieldLabel = label,
                Text = value ?? "",
                Placeholder = placeholder,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };
            parent.Controls.Add(field, 0, row);
            return field;
        }

        private CheckBox AddCheckField(TableLayoutPanel parent, string label, bool isChecked, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 14)
            };

            var lbl = new Label
            {
                Text = label,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Dock = DockStyle.Top
            };

            var chk = new CheckBox
            {
                Text = "종료 여부",
                Checked = isChecked,
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.Text,
                AutoSize = true,
                Dock = DockStyle.Top
            };

            panel.Controls.Add(chk);
            panel.Controls.Add(lbl);
            parent.Controls.Add(panel, 0, row);
            return chk;
        }

        private static DateTimePicker AddDateField(TableLayoutPanel parent, string label, string? value, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 14)
            };

            var lbl = new Label
            {
                Text = label,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Dock = DockStyle.Top
            };

            var dtp = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Top
            };

            if (!string.IsNullOrEmpty(value) && value.Length == 6
                &&
                DateTime.TryParseExact("20" + value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dt))
            {
                dtp.Value = dt;
            }


            panel.Controls.Add(dtp);
            panel.Controls.Add(lbl);
            parent.Controls.Add(panel, 0, row);
            return dtp;
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
