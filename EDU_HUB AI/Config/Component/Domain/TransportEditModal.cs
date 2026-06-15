using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public class TransportEditModal : AppModal
    {
        private readonly TransportDto? _source;
        private readonly string? _presetType;
        private readonly ComboBox _cmbType;
        private readonly TextField _txtDepartLocation;
        private readonly TextField _txtDestination;
        private readonly TextField _txtDepartTime;
        private readonly TextField _txtArriveTime;

        public TransportDto? Result { get; private set; }

        public TransportEditModal(TransportDto? source, string? presetType = null)
        {
            _source = source;
            _presetType = presetType;
            var isEdit = !string.IsNullOrWhiteSpace(source?.transportId);
            ModalTitle = isEdit ? "운행 수정" : "운행 등록";
            ConfirmText = "저장";

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var row = 0;
            _cmbType = AddComboField(stack, "교통 수단", row++);
            foreach (var code in TransportTypes.All)
                _cmbType.Items.Add(new TypeItem(code, TransportTypes.GetLabel(code)));
            _cmbType.DisplayMember = "Label";
            _cmbType.ValueMember = "Value";
            _cmbType.SelectedIndexChanged += (_, _) => UpdateArrivePanel();

            _txtDepartLocation = AddField(stack, "출발 위치", source?.departLocation, "예) 교육원 후문, 울산역", row++);
            _txtDestination = AddField(stack, "목적지", source?.destination, "예) 서울역, 울산공항", row++);
            _txtDepartTime = AddField(stack, "출발", source?.departTime, "HH:mm", row++);
            _txtArriveTime = AddField(stack, "도착", source?.arriveTime, "HH:mm (선택)", row++);

            SelectType(source?.type ?? presetType);
            _cmbType.Enabled = !isEdit && string.IsNullOrWhiteSpace(presetType);

            _txtDepartLocation.Required = true;
            _txtDepartLocation.TextChanged += (_, _) => _txtDepartLocation.HasError = false;
            _txtDestination.Required = true;
            _txtDestination.TextChanged += (_, _) => _txtDestination.HasError = false;
            _txtDepartTime.Required = true;
            _txtDepartTime.TextChanged += (_, _) => _txtDepartTime.HasError = false;

            Body.Controls.Add(stack);
            UpdateArrivePanel();
        }

        public static TransportDto? Show(IWin32Window owner, TransportDto? source, string? presetType = null)
        {
            using var modal = new TransportEditModal(source, presetType);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            var type = (_cmbType.SelectedItem as TypeItem)?.Value?.Trim();
            var departLocation = _txtDepartLocation.Text.Trim();
            var destination = _txtDestination.Text.Trim();
            var departTime = _txtDepartTime.Text.Trim();
            var arriveTime = _txtArriveTime.Text.Trim();

            if (string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show("교통 수단을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(departLocation))
            {
                _txtDepartLocation.HasError = true;
                MessageBox.Show("출발 위치를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(destination))
            {
                _txtDestination.HasError = true;
                MessageBox.Show("목적지를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(departTime))
            {
                _txtDepartTime.HasError = true;
                MessageBox.Show("출발 시간을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidTime(departTime))
            {
                MessageBox.Show("출발 시간 형식을 확인해주세요. (HH:mm)", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (TransportTypes.UsesArriveTime(type) && !string.IsNullOrWhiteSpace(arriveTime) && !IsValidTime(arriveTime))
            {
                MessageBox.Show("도착 시간 형식을 확인해주세요. (HH:mm)", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isEdit = _source != null;
            if (!ConfirmModal.Show(Owner,
                isEdit ? "수정 확인" : "등록 확인",
                isEdit ? "수정하시겠습니까?" : "등록하시겠습니까?",
                isEdit ? "수정" : "등록",
                ButtonVariant.Primary))
                return;

            Result = _source != null ? CopyOf(_source) : new TransportDto();
            Result.type = type;
            Result.departLocation = departLocation;
            Result.destination = destination;
            Result.departTime = departTime;
            Result.arriveTime = string.IsNullOrWhiteSpace(arriveTime) ? null : arriveTime;
            Result.delYn ??= "N";
            base.OnConfirm();
        }

        private void SelectType(string? type)
        {
            var normalized = type?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(normalized))
            {
                if (_cmbType.Items.Count > 0)
                    _cmbType.SelectedIndex = 0;
                return;
            }

            for (var i = 0; i < _cmbType.Items.Count; i++)
            {
                if (_cmbType.Items[i] is TypeItem item
                    && string.Equals(item.Value, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    _cmbType.SelectedIndex = i;
                    return;
                }
            }

            if (_cmbType.Items.Count > 0)
                _cmbType.SelectedIndex = 0;
        }

        private void UpdateArrivePanel()
        {
            var type = (_cmbType.SelectedItem as TypeItem)?.Value;
            var showArrive = TransportTypes.UsesArriveTime(type);
            _txtArriveTime.Visible = showArrive;
            if (!showArrive)
                _txtArriveTime.Text = "";
            FitCardSize();
        }

        private static bool IsValidTime(string value)
        {
            return TimeSpan.TryParseExact(value, @"hh\:mm", null, out _)
                || TimeSpan.TryParseExact(value, @"h\:mm", null, out TimeSpan _);
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

        private static ComboBox AddComboField(TableLayoutPanel parent, string label, int row)
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

        private static TransportDto CopyOf(TransportDto t) => new()
        {
            transportId = t.transportId,
            type = t.type,
            departLocation = t.departLocation,
            destination = t.destination,
            departTime = t.departTime,
            arriveTime = t.arriveTime,
            createdAt = t.createdAt,
            updatedAt = t.updatedAt,
            delYn = t.delYn
        };

        private sealed class TypeItem(string value, string label)
        {
            public string Value { get; } = value;
            public string Label { get; } = label;
        }
    }
}
