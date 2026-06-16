using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public partial class DormMaxCntEditModal : AppModal
    {
        private readonly DormitoryDto? _source;
        private readonly TextField _txtDormitoryID;
        private readonly TextField _txtRoomMaxCnt;

        public DormitoryDto? Result { get; private set; }
        public DormMaxCntEditModal(DormitoryDto? source)
        {
            _source = source;
            ModalTitle = "최대 인원 변경";
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
            var roomName = source?.dormitoryRoomName != null ? source.dormitoryRoomName + "호" : "미지정";
            _txtDormitoryID = AddField(stack, "호실명", roomName, "", 0);
            _txtDormitoryID.Enabled = false;
            _txtRoomMaxCnt = AddField(stack, "최대 인원", source?.maxCount.ToString() ?? "미정", "", 1);
            _txtRoomMaxCnt.Required = true;
            _txtRoomMaxCnt.TextChanged += (_, _) => _txtRoomMaxCnt.HasError = false;

            Body.Controls.Add(stack);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public static DormitoryDto? Show(IWin32Window owner, DormitoryDto? source)
        {
            using var modal = new DormMaxCntEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            _txtRoomMaxCnt.HasError = false;

            var maxCnt = _txtRoomMaxCnt.Text.Trim();

            if (string.IsNullOrWhiteSpace(maxCnt))
            {
                _txtRoomMaxCnt.HasError = true;
                MessageBox.Show("변경하려는 최대 인원을 입력해주세요", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(!int.TryParse(maxCnt, out int maxCount))
            {
                _txtRoomMaxCnt.HasError = true;
                MessageBox.Show("숫자만 입력해주세요", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(Convert.ToInt32(maxCnt) < _source?.currentCount)
            {
                _txtRoomMaxCnt.HasError = true;
                MessageBox.Show("현재 배정인원보다 작은 인원은 입력할 수 없습니다", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmModal.Show(Owner, "수정 확인", "수정하시겠습니까?", "수정", ButtonVariant.Primary))
                return;

            Result = _source != null ? CopyOf(_source) : new DormitoryDto();
            Result.maxCount= Convert.ToInt32(maxCnt);
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

        private static DormitoryDto CopyOf(DormitoryDto d) => new()
        {
            dormitoryId = d.dormitoryId,
            eduId = d.eduId,
            currentCount = d.currentCount,
            maxCount = d.maxCount,
            delYn = d.delYn,
            dormitoryRoomName = d.dormitoryRoomName
        };
    }
}
