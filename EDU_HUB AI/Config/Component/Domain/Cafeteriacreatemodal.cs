using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public class CafeteriaCreateModal : AppModal
    {
        private static readonly string[] MealTypes = { "BREAKFAST", "LUNCH", "DINNER" };
        private static readonly string[] MealLabels = { "조식", "중식", "석식" };

        private const int InputHeight = 40;
        private const int LabelGap = 25;

        // 날짜 피커
        private DateField _datePicker;

        // 식사별 TextBox / 휴무 CheckBox
        private readonly List<TextBox> _mealInputs = new();
        private readonly List<CheckBox> _closedChecks = new();

        // 이미 등록된 날짜 목록 (중복 방지용)
        private readonly HashSet<string> _existingDates;

        public List<CafeteriaDto> Result { get; private set; } = new();

        // ─────────────────────────────────────────────────────────────────
        public CafeteriaCreateModal(IEnumerable<string> existingDates)
        {
            _existingDates = new HashSet<string>(existingDates);

            ModalTitle = "식단 추가";
            ConfirmText = "추가";

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 4,   // 날짜 + 조식 + 중식 + 석식
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // ── 날짜 선택 필드 ──
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.Controls.Add(CreateDateField(), 0, 0);

            // ── 식사별 필드 ──
            for (int m = 0; m < 3; m++)
            {
                stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                stack.Controls.Add(CreateMealField(MealLabels[m], m), 0, m + 1);
            }

            Body.Controls.Add(stack);
            SetCardWidth(500);
        }

        // ─────────────────────────────────────────────────────────────────
        // 날짜 선택 필드
        // ─────────────────────────────────────────────────────────────────
        private DateField CreateDateField()
        {
            _datePicker = new DateField
            {
                FieldLabel = "날짜",
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Value = DateTime.Today,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 14)
            };
            return _datePicker;
        }

        // ─────────────────────────────────────────────────────────────────
        // 식사별 입력 필드 (메뉴 TextBox + 휴무 CheckBox)
        // ─────────────────────────────────────────────────────────────────
        private Panel CreateMealField(string labelText, int index)
        {
            var container = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0),
                Margin = new Padding(0, 0, 0, 14)
            };

            // ① 라벨 + 체크박스를 가로로 나란히 배치할 패널
            var labelRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = ThemeColors.Surface,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 4)
            };

            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Font = ThemeFonts.FieldLabel,
                ForeColor = ThemeColors.TextMuted,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 2, 8, 0)
            };

            var chkClosed = new CheckBox
            {
                Text = "휴무",
                Font = ThemeFonts.FieldLabel,
                ForeColor = ThemeColors.TextMuted,
                BackColor = ThemeColors.Surface,
                AutoSize = true,
                Margin = new Padding(0, 2, 0, 0)
            };

            labelRow.Controls.Add(label);
            labelRow.Controls.Add(chkClosed);

            // ② 입력 shell
            var shell = new Panel
            {
                Height = InputHeight,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(12, 7, 12, 7),
                Margin = new Padding(0),
                Cursor = Cursors.IBeam,
                Dock = DockStyle.Top
            };

            var txtInput = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.Text,
                BackColor = ThemeColors.Surface,
                Dock = DockStyle.Fill,
                Multiline = true,
                PlaceholderText = "메뉴를 입력하세요 (쉼표로 구분)"
            };

            chkClosed.CheckedChanged += (_, _) =>
            {
                txtInput.Enabled = !chkClosed.Checked;
                txtInput.BackColor = chkClosed.Checked ? ThemeColors.Background : ThemeColors.Surface;
                shell.BackColor = txtInput.BackColor;
                shell.Invalidate();
                if (chkClosed.Checked) txtInput.Clear();
            };

            bool focused = false;
            shell.Paint += (_, e) =>
            {
                var bounds = new Rectangle(0, 0, shell.Width - 1, shell.Height - 1);
                using var fill = new SolidBrush(shell.BackColor);
                e.Graphics.FillRectangle(fill, bounds);
                var borderColor = focused ? ThemeColors.Primary : ThemeColors.Border;
                using var pen = new Pen(borderColor);
                e.Graphics.DrawRectangle(pen, bounds);
            };
            txtInput.Enter += (_, _) => { focused = true; shell.Invalidate(); };
            txtInput.Leave += (_, _) => { focused = false; shell.Invalidate(); };
            shell.Click += (_, _) => txtInput.Focus();

            shell.Controls.Add(txtInput);
            _mealInputs.Add(txtInput);
            _closedChecks.Add(chkClosed);

            // Dock.Top 역순 추가
            container.Controls.Add(shell);
            container.Controls.Add(labelRow);

            return container;
        }

        // ─────────────────────────────────────────────────────────────────
        // 저장 (OnConfirm)
        // ─────────────────────────────────────────────────────────────────
        protected override void OnConfirm()
        {
            string selectedDate = _datePicker.Value.ToString("yyyy-MM-dd");

            // 1. 중복 날짜 체크
            if (_existingDates.Contains(selectedDate))
            {
                MessageBox.Show($"'{selectedDate}' 날짜는 이미 등록된 식단입니다.", "알림");
                return;
            }

            // 2. 메뉴 유효성 검사 (휴무가 아닌 경우만)
            var invalidMeals = new List<string>();
            for (int m = 0; m < 3; m++)
            {
                if (_closedChecks[m].Checked) continue;
                string menuText = _mealInputs[m].Text.Trim();
                if (!string.IsNullOrWhiteSpace(menuText) && !IsValidMenu(menuText))
                    invalidMeals.Add(MealLabels[m]);
            }

            if (invalidMeals.Count > 0)
            {
                MessageBox.Show(
                    $"{string.Join(", ", invalidMeals)} 메뉴에 허용되지 않는 특수문자가 포함되어 있습니다.",
                    "알림");
                return;
            }

            // 3. 확인 모달
            if (!ConfirmModal.Show(Owner, "추가 확인", $"'{selectedDate}' 식단을 추가하시겠습니까?", "추가", ButtonVariant.Primary))
                return;

            Result = BuildResult(selectedDate);
            base.OnConfirm();
        }

        // ─────────────────────────────────────────────────────────────────
        // 헬퍼
        // ─────────────────────────────────────────────────────────────────
        private List<CafeteriaDto> BuildResult(string date)
        {
            var result = new List<CafeteriaDto>();

            for (int m = 0; m < 3; m++)
            {
                bool isClosed = _closedChecks[m].Checked;
                string menuText = _mealInputs[m].Text.Trim();

                result.Add(new CafeteriaDto
                {
                    mealDate = date,
                    mealType = MealTypes[m],
                    menu = isClosed ? "[]" : TextToJson(menuText),
                    mealClosed = isClosed ? "Y" : "N"
                });
            }

            return result;
        }

        private bool IsValidMenu(string menuText) =>
            System.Text.RegularExpressions.Regex.IsMatch(menuText, @"^[가-힣a-zA-Z0-9\s,]+$");

        private string TextToJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "[]";
            var quoted = text.Split(',')
                             .Select(i => i.Trim())
                             .Where(i => !string.IsNullOrWhiteSpace(i))
                             .Select(i => $"\"{i}\"");
            return "[" + string.Join(", ", quoted) + "]";
        }
    }
}