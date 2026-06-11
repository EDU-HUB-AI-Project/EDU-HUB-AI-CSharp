using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
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
        private readonly ComboField _cmbEdu;
        private readonly ToggleSwitch _togDorm;

        private readonly AdminEduInfoController _eduInfoController = new();

        public StudentDto? Result { get; private set; }

        public StudentEditModal(StudentDto? source)
        {
            _source = source;
            ModalTitle = source == null ? "교육생 등록" : "교육생 수정";
            ConfirmText = "저장";

            _txtName = new TextField
            {
                FieldLabel = "이름",
                Text = source?.studentName ?? "",
                Placeholder = "교육생 이름",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            _txtBirth = new TextField
            {
                FieldLabel = "생년월일",
                Text = source?.birthDate ?? "",
                Placeholder = "YYYY-MM-DD",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 14)
            };

            _txtPhone = new TextField
            {
                FieldLabel = "연락처",
                Text = source?.phoneNumber ?? "",
                Placeholder = "010-0000-0000",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            _cmbEdu = new ComboField
            {
                FieldLabel = "교육과정",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            _togDorm = new ToggleSwitch
            {
                FieldLabel = "생활관 신청 여부",
                InlineLabel = "생활관 배정",
                Checked = source?.dormYn == "Y",
                Margin = new Padding(0, 0, 0, 4)
            };

            var rowBirthPhone = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Margin = new Padding(0),
                BackColor = ThemeColors.Surface
            };

            rowBirthPhone.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowBirthPhone.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowBirthPhone.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rowBirthPhone.Controls.Add(_txtBirth, 0, 0);
            rowBirthPhone.Controls.Add(_txtPhone, 1, 0);

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
            stack.Controls.Add(_txtName, 0, 0);
            stack.Controls.Add(rowBirthPhone, 0, 1);
            stack.Controls.Add(_cmbEdu, 0, 2);
            stack.Controls.Add(_togDorm, 0, 3);

            Body.Controls.Add(stack);
            SetCardWidth(500);
        }

        // ── 데이터 ─────────────────────────────
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadEduInfos();
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
            {
                _cmbEdu.SelectedValue = _source.eduId;
            }
        }

        // ── 저장 ─────────────────────────────
        protected override void OnConfirm()
        {
            var name = _txtName.Text.Trim();
            var birth = DateHelper.NormalizeBirthDate(_txtBirth.Text);
            var phone = _txtPhone.Text.Trim().Replace("-", "").Replace(" ", "");
            var eduId = _cmbEdu.SelectedValue?.ToString();

            if(string.IsNullOrWhiteSpace(name))
            {
                _txtName.HasError = true;
                MessageBox.Show("이름을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _txtName.HasError = false;
            if(birth == null)
            {
                _txtBirth.HasError = true;
                MessageBox.Show("생년월일 형식이 올바르지 않습니다.\n예) 2000-01-01 / 00-01-01 / 000101", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _txtBirth.HasError = false;

            var validPrefixes = new[] {"010", "011" };
            if(!string.IsNullOrEmpty(phone) && (!phone.All(char.IsDigit) || phone.Length != 11 || !validPrefixes.Any(p => phone.StartsWith(p))))
            {
                _txtPhone.HasError = true;
                MessageBox.Show("유효한 연락처를 입력해주세요.\n예) 010-1234-5678", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _txtPhone.HasError = false;

            if(string.IsNullOrEmpty(eduId))
            {
                MessageBox.Show("교육과정을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isEdit = _source != null;
            if (!ConfirmModal.Show(Owner,
                isEdit ? "수정 확인" : "등록 확인",
                isEdit ? "수정하시겠습니까?" : "등록하시겠습니까?",
                isEdit ? "수정" : "등록",
                ButtonVariant.Primary))
                return;

            Result = _source != null ? CopyOf(_source) : new StudentDto();
            Result.studentName = name;
            Result.birthDate = birth;
            Result.phoneNumber = phone;
            Result.eduId = eduId;
            Result.dormYn = _togDorm.Checked ? "Y" : "N";
            Result.attendYn ??= "N";
            base.OnConfirm();
        }

        // ── 팩토리 ─────────────────────────────
        public static StudentDto? Show(IWin32Window owner, StudentDto? source)
        {
            using var modal = new StudentEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        // ── 헬퍼 ─────────────────────────────
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
