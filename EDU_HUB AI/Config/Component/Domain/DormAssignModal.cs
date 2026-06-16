using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;
using System.Data;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public partial class DormAssignModal : AppModal
    {
        private readonly DormAssignDto? _source;
        private readonly TextField _txtDormRoomName;
        private readonly ComboBox _cmbDormitory;
        private readonly AdminDormitoryController _adminDormitoryController = new();

        public DormAssignDto? Result { get; private set; }
        public DormAssignModal(DormAssignDto? source)
        {
            _source = source;
            ModalTitle = source?.dormitoryId == null ? "생활관 배정" : "생활관 변경";
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
            _txtDormRoomName.Enabled = false;
            _cmbDormitory = AddComboField(stack, "호실 선택", 1, required: true);

            Body.Controls.Add(stack);
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
            bool isEdit = _source?.dormitoryId != null;
            if (!ConfirmModal.Show(Owner,
                isEdit ? "변경 확인" : "배정 확인",
                isEdit ? "생활관을 변경하시겠습니까?" : "생활관을 배정하시겠습니까?",
                isEdit ? "변경" : "배정",
                ButtonVariant.Primary))
                return;

            Result = _source != null ? CopyOf(_source) : new DormAssignDto();
            var dormId = _cmbDormitory.SelectedValue?.ToString();
            if (string.IsNullOrWhiteSpace(dormId))
            {
                MessageBox.Show("호실을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Result.dormitoryId = dormId;
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
        private ComboBox AddComboField(TableLayoutPanel parent, string label, int row, bool required = false)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 14)
            };

            var lblPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Dock = DockStyle.Top
            };
            lblPanel.Controls.Add(new Label
            {
                Text = label,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Margin = new Padding(0)
            });

            if(required)
            {
                lblPanel.Controls.Add(new Label
                {
                    Text = " *",
                    Font = ThemeFonts.BodySm,
                    ForeColor = ThemeColors.Danger,
                    AutoSize = true,
                    Margin = new Padding(0)
                });
            }

            var cmb = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = ThemeFonts.Body,
                BackColor = ThemeColors.Surface
            };

            panel.Controls.Add(cmb);
            panel.Controls.Add(lblPanel);
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
