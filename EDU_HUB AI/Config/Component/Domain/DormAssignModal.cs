using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
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
    public partial class DormAssignModal : AppModal
    {
        private readonly DormAssignDto? _source;
        private readonly TextField _txtDormRoomName;
        private readonly ComboBox _cmbDormitory;
        private readonly AdminDormitoryController _adminDormitoryController = new();
        private bool _isCancelAssign = false;

        public DormAssignDto? Result { get; private set; }
        public DormAssignModal(DormAssignDto? source)
        {
            _source = source;
            ModalTitle = source?.dormitoryId == null ? "생활관 배정" : "생활관 변경 및 취소";
            ConfirmText = "저장";

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _txtDormRoomName = AddField(stack, "현재호실", source?.dormitoryRoomName ?? "미배정", "", 0);
            _txtDormRoomName.ReadOnly = true;
            _cmbDormitory = AddComboField(stack, "호실 선택", 1);

            Body.Controls.Add(stack);
            if (source?.dormitoryId != null)
            {
                var btnCancel = new AppButton
                {
                    Text = "배정 취소",
                    Dock = DockStyle.Top
                };
                btnCancel.Click += (_, _) =>
                {
                    if (MessageBox.Show("배정을 취소하시겠습니까?", "확인",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        _isCancelAssign = true;  // 배정 취소시 여기서 처리
                        Result = CopyOf(source);
                        Result.dormitoryId = null;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                };
                Body.Controls.Add(btnCancel);
            }
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadCmb();
        }

        private async Task LoadCmb()
        {
            var response = await _adminDormitoryController.GetCmbDorm();
            if (response?.Status == 200)
            {
                // 생활관 호실
                var dorms = response.Data
                    .Select(x => new { x.dormitoryId, x.dormitoryRoomName })
                    .DistinctBy(x => x.dormitoryId)
                    .ToList();
                _cmbDormitory.DataSource = dorms;
                _cmbDormitory.DisplayMember = "dormitoryRoomName";
                _cmbDormitory.ValueMember = "dormitoryId";
            }
        }

        public static DormAssignDto? Show(IWin32Window owner, DormAssignDto? source)
        {
            using var modal = new DormAssignModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            if (_isCancelAssign) return; // 배정 취소는 앞에서 이미 처리됨을 알림
            Result = _source != null ? CopyOf(_source) : new DormAssignDto();
            Result.dormitoryId= _cmbDormitory.SelectedValue.ToString();
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

        private static DormAssignDto CopyOf(DormAssignDto d) => new()
        {
            studentName = d.studentName,
            studentId = d.studentId,
            eduId = d.eduId,
            phone = d.phone,
            dormitoryId = d.dormitoryId,
            dormitoryRoomName = d.dormitoryRoomName,
            assignStatus = d.assignStatus
        };
    }
}
