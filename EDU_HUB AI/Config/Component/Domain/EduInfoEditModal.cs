using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
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
        private readonly DateTimePicker _dtpStartDate;
        private readonly DateTimePicker _dtpEndDate;
        private readonly TextField _txtBatchNumber;
        private readonly TextField _txtCapacity;

        public EduInfoDto? Result { get; private set; }

        public EduInfoEditModal(EduInfoDto? source)
        {
            _source = source;
            ModalTitle = source == null ? "교육과정 등록" : "교육과정 수정";
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

            _txtEduName = AddField(stack, "과정명", source?.eduName, "과정명 입력", 0);
            _dtpStartDate = AddDateField(stack, "시작일", source?.startDate, 1);
            _dtpEndDate = AddDateField(stack, "종료일", source?.endDate, 2);
            _txtBatchNumber = AddField(stack, "기수", source?.batchNumber.ToString(), "숫자 입력", 3);
            _txtCapacity = AddField(stack, "정원", source?.capacity.ToString(), "숫자 입력", 4);

            Body.Controls.Add(stack);
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
            var batchNumber = _txtBatchNumber.Text.Trim();
            var capacity = _txtCapacity.Text.Trim();

            if(string.IsNullOrWhiteSpace(eduName))
            {
                MessageBox.Show("과정명을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(!int.TryParse(batchNumber, out var batch) || batch <= 0)
            {
                MessageBox.Show("기수는 1 이상의 숫자를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(!int.TryParse(capacity, out var cap) || cap <= 0)
            {
                MessageBox.Show("정원은 1 이상의 숫자를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Result = _source != null ? CopyOf(_source) : new EduInfoDto();
            Result.eduName = eduName;
            Result.startDate = startDate;
            Result.endDate = endDate;
            Result.batchNumber = batch;
            Result.capacity = cap;
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

        private static DateTimePicker AddDateField(TableLayoutPanel parent, string label, string? value, int row)
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
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Top
            };

            if (!string.IsNullOrEmpty(value) && value.Length == 6 
                &&
                DateTime.TryParseExact("20" + value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dt))
            {
                dtp.Value = dt;
            }


            panel.Controls.Add(dtp);
            panel.Controls.Add(lbl);
            parent.Controls.Add(panel, 0, row);
            return dtp;
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
