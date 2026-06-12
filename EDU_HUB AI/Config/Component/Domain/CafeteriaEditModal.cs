using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public class CafeteriaEditModal : AppModal
    {
        private List<CafeteriaDto> _existingList = new List<CafeteriaDto>();
        private readonly string _date;

        private List<TextBox> _mealInputList = new List<TextBox>();

        private static readonly string[] MealTypes = { "BREAKFAST", "LUNCH", "DINNER" };
        private static readonly string[] MealLabels = { "조식", "중식", "석식" };

        // TextField와 동일한 상수
        private const int InputHeight = 40;   // Multiline이므로 더 높게
        private const int LabelGap = 25;

        public List<CafeteriaDto> Result { get; private set; } = new List<CafeteriaDto>();

        public CafeteriaEditModal(string date)
        {
            _date = date;
            ModalTitle = $"{date} 식단";
            ConfirmText = "저장";

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

            for (int m = 0; m < 3; m++)
            {
                stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                stack.Controls.Add(CreateMealField(MealLabels[m], m), 0, m);
            }

            Body.Controls.Add(stack);
            SetCardWidth(500);
        }

        // ── TextField 구조를 그대로 따라한 Multiline 필드 ──────────────
        private Panel CreateMealField(string labelText, int index)
        {
            // 전체를 감싸는 컨테이너 (Margin으로 필드 간격 부여)
            var container = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0),
                Margin = new Padding(0, 0, 0, 14)  // StudentEditModal과 동일
            };

            // ① Label — TextField._label과 동일 스타일
            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Font = ThemeFonts.FieldLabel,
                ForeColor = ThemeColors.TextMuted,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, LabelGap),
                Dock = DockStyle.Top
            };

            // ② Shell Panel — TextField._inputShell과 동일 구조
            var shell = new Panel
            {
                Height = InputHeight,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(12, 7, 12, 7),
                Margin = new Padding(0),
                Cursor = Cursors.IBeam,
                Dock = DockStyle.Top
            };

            // ③ TextBox — TextField._input과 동일 스타일, Multiline만 추가
            var txtInput = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = ThemeFonts.Body,
                ForeColor = ThemeColors.Text,
                BackColor = ThemeColors.Surface,
                Dock = DockStyle.Fill,
                Multiline = true
            };

            bool focused = false;

            // 포커스 시 테두리 색 변경 — TextField.PaintInputBorder와 동일 로직
            shell.Paint += (_, e) =>
            {
                var bounds = new Rectangle(0, 0, shell.Width - 1, shell.Height - 1);
                using var fill = new SolidBrush(ThemeColors.Surface);
                e.Graphics.FillRectangle(fill, bounds);

                var borderColor = focused ? ThemeColors.Primary : ThemeColors.Border;
                using var pen = new Pen(borderColor);
                e.Graphics.DrawRectangle(pen, bounds);
            };

            txtInput.Enter += (_, _) => { focused = true; shell.Invalidate(); };
            txtInput.Leave += (_, _) => { focused = false; shell.Invalidate(); };
            shell.Click += (_, _) => txtInput.Focus();

            shell.Controls.Add(txtInput);
            _mealInputList.Add(txtInput);

            // Dock.Top은 나중에 추가한 게 위로 올라오므로 역순 추가
            container.Controls.Add(shell);
            container.Controls.Add(label);

            return container;
        }

        // ── 기존 데이터 로드 ──────────────────────────────────────────
        public void LoadExistingData(List<CafeteriaDto> existingList)
        {
            _existingList = existingList;

            foreach (var item in _existingList)
            {
                int mealIndex = item.mealType switch
                {
                    "BREAKFAST" => 0,
                    "LUNCH" => 1,
                    "DINNER" => 2,
                    _ => -1
                };
                if (mealIndex < 0) continue;
                _mealInputList[mealIndex].Text = JsonToText(item.menu);
            }

            FitCardSize();
        }

        // ── 저장 ──────────────────────────────────────────────────────
        protected override void OnConfirm()
        {
            var invalidMeals = new List<string>();
            for (int m = 0; m < 3; m++)
            {
                string menuText = _mealInputList[m].Text.Trim();
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

            if (!ConfirmModal.Show(Owner, "수정 확인", "수정하시겠습니까?", "수정", ButtonVariant.Primary))
                return;

            Result = BuildResult();
            base.OnConfirm();
        }

        // ── 헬퍼 ──────────────────────────────────────────────────────
        private bool IsValidMenu(string menuText) =>
            System.Text.RegularExpressions.Regex.IsMatch(menuText, @"^[가-힣a-zA-Z0-9\s,]+$");

        private List<CafeteriaDto> BuildResult()
        {
            var result = new List<CafeteriaDto>();

            for (int m = 0; m < 3; m++)
            {
                string mealType = MealTypes[m];
                string menuText = _mealInputList[m].Text.Trim();

                var existing = _existingList.FirstOrDefault(x => x.mealType == mealType);

                if (string.IsNullOrWhiteSpace(menuText))
                {
                    if (existing != null) { existing.delYn = "Y"; result.Add(existing); }
                    continue;
                }

                if (existing != null)
                {
                    existing.menu = TextToJson(menuText);
                    existing.mealClosed = "N";
                    result.Add(existing);
                }
                else
                {
                    result.Add(new CafeteriaDto
                    {
                        mealDate = _date,
                        mealType = mealType,
                        menu = TextToJson(menuText),
                        mealClosed = "N"
                    });
                }
            }

            return result;
        }

        private string JsonToText(string? menu)
        {
            if (string.IsNullOrWhiteSpace(menu)) return "";
            return menu.Trim().Replace("[", "").Replace("]", "").Replace("\"", "").Trim();
        }

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