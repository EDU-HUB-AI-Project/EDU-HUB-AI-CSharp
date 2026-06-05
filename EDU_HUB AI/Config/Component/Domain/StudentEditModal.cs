using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
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
        private readonly TextField _txtEdu;
        private readonly TextField _txtDorm;

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
            _txtBirth = AddField(stack, "생년월일", source?.birthDate, "YYYY-MM-DD", 1);
            _txtPhone = AddField(stack, "연락처", source?.phoneNumber, "010-0000-0000", 2);
            _txtEdu = AddField(stack, "과정", source?.eduId, "과정 ID", 3);
            _txtDorm = AddField(stack, "생활관 (Y/N)", source?.dormYn, "Y 또는 N", 4);

            Body.Controls.Add(stack);
        }

        public static StudentDto? Show(IWin32Window owner, StudentDto? source)
        {
            using var modal = new StudentEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            Result = _source != null ? CopyOf(_source) : new StudentDto();
            Result.studentName = _txtName.Text.Trim();
            Result.birthDate = _txtBirth.Text.Trim();
            Result.phoneNumber = _txtPhone.Text.Trim();
            Result.eduId = _txtEdu.Text.Trim();
            Result.dormYn = _txtDorm.Text.Trim();
            base.OnConfirm();
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
