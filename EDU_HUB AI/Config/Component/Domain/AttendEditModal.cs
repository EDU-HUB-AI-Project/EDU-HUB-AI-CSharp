using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private readonly ComboBox _cmbEduId;
        private readonly DateTimePicker _dtpAttendDate;
        private readonly ComboBox _cmbStatus;
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
            _cmbEduId = AddComboField(stack, "교육id", 1);
            _dtpAttendDate = AddDTPField(stack, "출석일자", 2);
            _cmbStatus = AddComboField(stack, "출석상태", 3);
            _txtMessage = AddField(stack, "사유", source?.message, "조퇴, 지각, 결석일 경우 입력", 4);

            Body.Controls.Add(stack);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadCmb();
            LoadCmbStatus();
        }

        private async Task LoadCmb()
        {
            var response = await new AdminAttendaceController().GetAttend(null, null, null, null);
            if (response?.Status == 200)
            {
                // 교육과정 콤보박스
                var edus = response.Data
                    .Select(x => new { x.eduId, x.eduName })
                    .DistinctBy(x => x.eduId)
                    .ToList();
                _cmbEduId.DataSource = edus;
                _cmbEduId.DisplayMember = "eduName";
                _cmbEduId.ValueMember = "eduId";
            }
        }

        private void LoadCmbStatus()
        {
            _cmbStatus.Items.Add("출석");
            _cmbStatus.Items.Add("결석");
            _cmbStatus.Items.Add("지각");
            _cmbStatus.Items.Add("조퇴");
            _cmbStatus.SelectedIndex = 0;
            if (_source?.status != null) _cmbStatus.SelectedItem = _source.status;
            else _cmbStatus.SelectedIndex = 0;
        }
        
        public static AttendDto? Show(IWin32Window owner, AttendDto? source)
        {
            using var modal = new AttendEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            var studentId = _txtStudentId.Text.Trim();
            var attendDate = _dtpAttendDate.Value;
            var status = _cmbStatus.SelectedItem.ToString();
            var msg = _txtMessage.Text.Trim();
            Debug.WriteLine($"선택한 날짜: {attendDate.Date}");
            Debug.WriteLine($"오늘 날짜: {DateTime.Today}");

            if (studentId == null || studentId == "")
            {
                MessageBox.Show("학생 Id를 입력해주세요", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (attendDate.Date > DateTime.Today)
            {
                MessageBox.Show("오늘 이후의 날짜는 입력할 수 없습니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 출석상태가 출석이 아니면서 사유가 존재하지 않는 경우
            if(!status.Equals("출석") && (msg == null || msg == ""))
            {
                MessageBox.Show("해당하는 사유를 입력해주세요", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Result = _source != null ? CopyOf(_source) : new AttendDto();
            Result.studentId = studentId;  
            Result.eduId = _cmbEduId.SelectedValue.ToString();
            Result.attendDate = attendDate.ToString("yyyy-MM-dd");
            Result.status = status;
            Result.message = msg;
            base.OnConfirm();
        }

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

            private DateTimePicker AddDTPField(TableLayoutPanel parent, string label, int row)
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
                    Dock = DockStyle.Top,
                    Font = ThemeFonts.Body,
                    BackColor = ThemeColors.Surface,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "yyyy-MM-dd"
                };
                panel.Controls.Add(dtp);
                panel.Controls.Add(lbl);
                parent.Controls.Add(panel, 0, row);
                return dtp;
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
