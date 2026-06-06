using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public partial class AttendEditModal : AppModal
    {
        private readonly AttendDto? _source;
        private readonly TextField _txtStudentId;
        private readonly TextField _txtEduId;
        private readonly TextField _txtStatus;
        private readonly TextField _txtMessage;

        public AttendDto? Result { get; private set; }

        public AttendEditModal(AttendDto? source)
        {
            _source = source;
            ModalTitle = source == null ? "출석부 등록" : "출석부 수정";
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

            _txtStudentId = AddField(stack, "학생id", source?.studentId, "STU_xxxxx", 0);
            _txtEduId = AddField(stack, "교육id", source?.eduId, "EDU_xxx", 1);
            _txtStatus = AddField(stack, "상태", source?.status, "출석, 지각, 조퇴, 결석", 2);
            _txtMessage = AddField(stack, "사유", source?.message, "조퇴, 지각, 출석일 경우 입력", 3);

            Body.Controls.Add(stack);
        }

        public static AttendDto? Show(IWin32Window owner, AttendDto? source)
        {
            using var modal = new AttendEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            Result = _source != null ? CopyOf(_source) : new AttendDto();
            Result.studentId = _txtStudentId.Text.Trim();  
            Result.eduId = _txtEduId.Text.Trim();
            Result.status = _txtStatus.Text.Trim();
            Result.message = _txtMessage.Text.Trim();
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

        private static AttendDto CopyOf(AttendDto s) => new()
        {
            attendanceId = s.attendanceId,
            studentId = s.studentId,
            studentName = s.studentName,
            eduId = s.eduId,
            eduName = s.eduName,
            status = s.status,
            createdAt = s.createdAt,
            updatedAt = s.updatedAt,
            attendDate = s.attendDate
        };
    }
}
