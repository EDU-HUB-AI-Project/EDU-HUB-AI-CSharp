using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    /// <summary>교육생 등록/수정 모달 — AppModal 기반.</summary>
    public class EduInfoEditModal : AppModal
    {
        private readonly EduInfoDto? _source;
        private readonly TextField _txtEduName;
        private readonly DateField _dtpStartDate;
        private readonly DateField _dtpEndDate;
        private readonly TextField _txtBatchNumber;
        private readonly TextField _txtCapacity;

        public EduInfoDto? Result { get; private set; }

        public EduInfoEditModal(EduInfoDto? source)
        {
            _source = source;
            ModalTitle = source == null ? "교육과정 등록" : "교육과정 수정";
            ConfirmText = "저장";

            _txtEduName = new TextField
            {
                FieldLabel = "과정명",
                Text = source?.eduName ?? "",
                Placeholder = "과정명 입력",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            _dtpStartDate = new DateField
            {
                FieldLabel = "시작일",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 14)
            };
            _dtpEndDate = new DateField
            {
                FieldLabel = "종료일",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };
            _dtpStartDate.SetYyMMdd(source?.startDate);
            _dtpEndDate.SetYyMMdd(source?.endDate);

            _txtBatchNumber = new TextField
            {
                FieldLabel = "기수",
                Text = source?.batchNumber > 0 ? source.batchNumber.ToString() : "",
                Placeholder = "숫자 입력",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 14)
            };
            _txtCapacity = new TextField
            {
                FieldLabel = "정원",
                Text = source?.capacity > 0 ? source.capacity.ToString() : "",
                Placeholder = "숫자 입력",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            // 시작일 | 종료일
            var rowDate = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Margin = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            rowDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowDate.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rowDate.Controls.Add(_dtpStartDate, 0, 0);
            rowDate.Controls.Add(_dtpEndDate, 1, 0);

            // 기수 | 정원
            var rowNumbers = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Margin = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            rowNumbers.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowNumbers.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rowNumbers.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rowNumbers.Controls.Add(_txtBatchNumber, 0, 0);
            rowNumbers.Controls.Add(_txtCapacity, 1, 0);

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.Controls.Add(_txtEduName, 0, 0);
            stack.Controls.Add(rowDate, 0, 1);
            stack.Controls.Add(rowNumbers, 0, 2);

            Body.Controls.Add(stack);
            SetCardWidth(480);

            _txtEduName.Required = true;
            _txtEduName.TextChanged += (_, _) => _txtEduName.HasError = false;

            _txtBatchNumber.Required = true;
            _txtBatchNumber.TextChanged += (_, _) => _txtBatchNumber.HasError = false;

            _txtCapacity.Required = true;
            _txtCapacity.TextChanged += (_, _) => _txtCapacity.HasError = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            FitCardSize();
        }

        public static EduInfoDto? Show(IWin32Window owner, EduInfoDto? source)
        {
            using var modal = new EduInfoEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            var eduName = _txtEduName.Text.Trim();
            var startDate = _dtpStartDate.Value.ToString("yyMMdd");
            var endDate = _dtpEndDate.Value.ToString("yyMMdd");
            var batchStr = _txtBatchNumber.Text.Trim();
            var capStr = _txtCapacity.Text.Trim();

            if (string.IsNullOrWhiteSpace(eduName))
            {
                _txtEduName.HasError = true;
                MessageBox.Show("과정명을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _txtEduName.HasError = false;

            if (!int.TryParse(batchStr, out var batch) || batch <= 0)
            {
                _txtBatchNumber.HasError = true;
                MessageBox.Show("기수는 1 이상의 숫자를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _txtBatchNumber.HasError = false;

            if (!int.TryParse(capStr, out var cap) || cap <= 0)
            {
                _txtCapacity.HasError = true;
                MessageBox.Show("정원은 1 이상의 숫자를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _txtCapacity.HasError = false;

            bool isEdit = _source != null;
            if (!ConfirmModal.Show(Owner,
                isEdit ? "수정 확인" : "등록 확인",
                isEdit ? "수정하시겠습니까?" : "등록하시겠습니까?",
                isEdit ? "수정" : "등록",
                ButtonVariant.Primary))
                return;

            Result = _source != null ? CopyOf(_source) : new EduInfoDto();
            Result.eduName = eduName;
            Result.startDate = startDate;
            Result.endDate = endDate;
            Result.batchNumber = batch;
            Result.capacity = cap;
            base.OnConfirm();
        }

        // ====== 리턴 ======
        private static EduInfoDto CopyOf(EduInfoDto e) => new()
        {
            eduId = e.eduId,
            eduName = e.eduName,
            startDate = e.startDate,
            endDate = e.endDate,
            batchNumber = e.batchNumber,
            capacity = e.capacity,
            delYn = e.delYn,
            createdAt = e.createdAt,
            updatedAt = e.updatedAt
        };
    }
}
