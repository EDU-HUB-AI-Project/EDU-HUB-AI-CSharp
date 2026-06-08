using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    /// <summary>교육생 등록/수정 모달 — AppModal 기반.</summary>
    public class StudentEditModal : AppModal
    {
        private readonly StudentDto? _source;
        private readonly TextField _txtName;
        private readonly TextField _txtBirth;
        private readonly TextField _txtPhone;
        private readonly ComboBox _cmbEdu;
        private readonly CheckBox _chkDorm;

        private readonly AdminEduInfoController _eduInfoController = new();

        public StudentDto? Result { get; private set; }

        public StudentEditModal(StudentDto? source)
        {
            _source = source;
            ModalTitle = source == null ? "교육생 등록" : "교육생 수정";
            ConfirmText = "저장";

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

            _txtName = AddField(stack, "이름", source?.studentName, "교육생 이름", 0);
            _txtBirth = AddField(stack, "생년월일", source?.birthDate, "YY-MM-DD", 1);
            _txtPhone = AddField(stack, "연락처", source?.phoneNumber, "010-0000-0000", 2);
            _cmbEdu = AddComboField(stack, "교육과정", 3);
            _chkDorm = AddCheckField(stack, "생활관 신청 여부", source?.dormYn == "Y", 4);

            Body.Controls.Add(stack);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadEduInfos();
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

        public static StudentDto? Show(IWin32Window owner, StudentDto? source)
        {
            using var modal = new StudentEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            var name = _txtName.Text.Trim();
            var birth = NormalizeBirthDate(_txtBirth.Text);
            var phone = _txtPhone.Text.Trim().Replace("-", "").Replace(" ", "");
            var eduId = _cmbEdu.SelectedValue?.ToString();

            if(string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("이름을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(birth == null)
            {
                MessageBox.Show("생년월일 형식이 올바르지 않습니다.\n예) 2000-01-01 / 00-01-01 / 000101", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var validPrefixes = new[] {"010", "011" };
            if(!string.IsNullOrEmpty(phone) && (!phone.All(char.IsDigit) || phone.Length != 11 || !validPrefixes.Any(p => phone.StartsWith(p))))
            {
                MessageBox.Show("유효한 연락처를 입력해주세요.\n예) 010-1234-5678", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(string.IsNullOrEmpty(eduId))
            {
                MessageBox.Show("교육과정을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            

            Result = _source != null ? CopyOf(_source) : new StudentDto();
            Result.studentName = name;
            Result.birthDate = birth;
            Result.phoneNumber = phone;
            Result.eduId = eduId;
            Result.dormYn = _chkDorm.Checked ? "Y" : "N";
            Result.attendYn = Result.attendYn ?? "N";
            base.OnConfirm();
        }

        // ====== 유효성 검사 ======
        private static string? NormalizeBirthDate(string input)
        {
            var s = input.Trim().Replace("-", "").Replace(" ", "");

            if (s.Length == 8 && s.All(char.IsDigit))
            {
                s = s.Substring(2);
            }
            if (s.Length == 6 && s.All(char.IsDigit))
            {
                var year = int.Parse("20" + s.Substring(0, 2));
                var month = int.Parse(s.Substring(2, 2));
                var day = int.Parse(s.Substring(4, 2));

                if (month < 1 || month > 12 || day < 1)
                {
                    return null;
                }
                if(day > DateTime.DaysInMonth(year, month))
                {
                    return null;
                }
                return s;
            }
            return null;
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
                Text = "생활관 배정",
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

        // ====== 리턴 ======
        private static StudentDto CopyOf(StudentDto s) => new()
        {
            studentId = s.studentId,
            studentName = s.studentName,
            birthDate = s.birthDate,
            eduId = s.eduId,
            attendYn = s.attendYn,
            dormYn = s.dormYn,
            dormitoryId = s.dormitoryId,
            delYn = s.delYn,
            createdAt = s.createdAt,
            phoneNumber = s.phoneNumber
        };
    }
}
