using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    /// <summary>교육생 등록/수정 모달 — AppModal 기반.</summary>
    public class ClassroomEditModal : AppModal
    {
        private readonly ClassroomDto? _source;
        private readonly TextField _txtClassroomName;

        public ClassroomDto? Result { get; private set; }

        public ClassroomEditModal(ClassroomDto? source)
        {
            _source = source;
            ModalTitle = "강의실 수정";
            ConfirmText = "저장";

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

            _txtClassroomName = AddField(stack, "강의실명", source?.classroomName, "강의실명 입력", 0);
            AddReadOnlyField(stack, "층", source?.floor.ToString(), 1);
            AddReadOnlyField(stack, "SVG ID", source?.imageId, 2);
            AddReadOnlyField(stack, "이미지 경로", source?.imagePath, 3);
            Body.Controls.Add(stack);
            SetCardWidth(440);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            FitCardSize();
        }

        public static ClassroomDto? Show(IWin32Window owner, ClassroomDto? source)
        {
            using var modal = new ClassroomEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            var classroomName = _txtClassroomName.Text.Trim();

            if(string.IsNullOrWhiteSpace(classroomName))
            {
                MessageBox.Show("강의실명을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmModal.Show(Owner, "수정 확인", "수정하시겠습니까?", "수정", ButtonVariant.Primary))
                return;

            Result = CopyOf(_source!);
            Result.classroomName = classroomName;
            base.OnConfirm();
        }

        // ====== UI 헬퍼 ======
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

        private static void AddReadOnlyField(TableLayoutPanel parent, string label, string? value, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var field = new TextField
            {
                FieldLabel = label,
                Text = value ?? "",
                Enabled = false,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };
            parent.Controls.Add(field, 0, row);
        }

        // ====== 리턴 ======
        private static ClassroomDto CopyOf(ClassroomDto c) => new()
        {
            classroomId = c.classroomId,
            classroomName = c.classroomName,
            floor = c.floor,
            imageId = c.imageId,
            imagePath = c.imagePath,
            delYn = c.delYn,
            createdAt = c.createdAt,
            updatedAt = c.updatedAt,
        };
    }
}
