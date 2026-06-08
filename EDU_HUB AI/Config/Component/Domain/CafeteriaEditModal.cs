using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public class CafeteriaEditModal : AppModal
    {
        private List<CafeteriaDto> _existingList = new List<CafeteriaDto>();

        private TextField _txtDate;
        private readonly bool _isEdit;

        private AppButton _btnBreakfast;
        private AppButton _btnLunch;
        private AppButton _btnDinner;

        private Panel _panelBreakfast;
        private Panel _panelLunch;
        private Panel _panelDinner;

        private TextField _txtBreakfast;
        private TextField _txtLunch;
        private TextField _txtDinner;

        // 조립된 DTO 반환용 (StudentEditModal 패턴)
        public List<CafeteriaDto> Result { get; private set; } = new List<CafeteriaDto>();

        public CafeteriaEditModal(string date = "")
        {
            _isEdit = !string.IsNullOrWhiteSpace(date);
            ModalTitle = _isEdit ? $"{date} 식단 수정" : "식단 등록";
            ConfirmText = "저장";

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0)
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _txtDate = AddField(stack, "날짜", date, "YYYY-MM-DD", 0);
            _txtDate.Enabled = !_isEdit;

            var tabPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 8, 0, 0)
            };

            _btnBreakfast = CreateTabButton("조식");
            _btnLunch = CreateTabButton("중식");
            _btnDinner = CreateTabButton("석식");

            _btnBreakfast.Click += (_, _) => ShowTab("BREAKFAST");
            _btnLunch.Click += (_, _) => ShowTab("LUNCH");
            _btnDinner.Click += (_, _) => ShowTab("DINNER");

            tabPanel.Controls.Add(_btnBreakfast);
            tabPanel.Controls.Add(_btnLunch);
            tabPanel.Controls.Add(_btnDinner);

            _panelBreakfast = CreateTabPanel(out _txtBreakfast, "조식 메뉴 입력 (예: 토스트, 우유)");
            _panelLunch = CreateTabPanel(out _txtLunch, "중식 메뉴 입력 (예: 돈까스, 밥)");
            _panelDinner = CreateTabPanel(out _txtDinner, "석식 메뉴 입력 (예: 된장찌개, 밥)");

            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.Controls.Add(tabPanel, 0, 1);

            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var contentPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ThemeColors.Surface
            };
            contentPanel.Controls.Add(_panelBreakfast);
            contentPanel.Controls.Add(_panelLunch);
            contentPanel.Controls.Add(_panelDinner);
            stack.Controls.Add(contentPanel, 0, 2);

            Body.Controls.Add(stack);

            ShowTab("BREAKFAST");
        }

        // View에서 기존 데이터를 주입 (OnLoad에서 controller 호출 제거)
        public void LoadExistingData(List<CafeteriaDto> existingList)
        {
            _existingList = existingList;

            foreach (var item in _existingList)
            {
                if (item.mealType == "BREAKFAST") _txtBreakfast.Text = JsonToText(item.menu);
                if (item.mealType == "LUNCH") _txtLunch.Text = JsonToText(item.menu);
                if (item.mealType == "DINNER") _txtDinner.Text = JsonToText(item.menu);
            }

            FitCardSize();
        }

        // API 호출 제거 — DTO 조립만 하고 Result에 담아 반환
        protected override void OnConfirm()
        {
            string date = _txtDate.Text.Trim();

            if (string.IsNullOrWhiteSpace(date))
            {
                MessageBox.Show("날짜를 입력해주세요", "알림");
                return;
            }

            Result = BuildResult(date);
            base.OnConfirm();
        }

        private List<CafeteriaDto> BuildResult(string date)
        {
            var result = new List<CafeteriaDto>();
            AddMealToResult(result, date, "BREAKFAST", _txtBreakfast.Text.Trim());
            AddMealToResult(result, date, "LUNCH", _txtLunch.Text.Trim());
            AddMealToResult(result, date, "DINNER", _txtDinner.Text.Trim());
            return result;
        }

        private void AddMealToResult(List<CafeteriaDto> result, string date, string mealType, string menuText)
        {
            CafeteriaDto existing = null;
            foreach (var item in _existingList)
            {
                if (item.mealType == mealType)
                {
                    existing = item;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(menuText))
            {
                // 빈 값 + 기존 항목 있으면 → 삭제 마킹
                if (existing != null)
                {
                    existing.delYn = "Y";
                    result.Add(existing);
                }
                return;
            }

            if (existing != null)
            {
                existing.menu = TextToJson(menuText);
                result.Add(existing);
            }
            else
            {
                result.Add(new CafeteriaDto
                {
                    mealDate = date,
                    mealType = mealType,
                    menu = TextToJson(menuText),
                    mealClosed = "N"
                });
            }
        }

        public static bool Show(IWin32Window owner, string date = "")
        {
            using var modal = new CafeteriaEditModal(date);
            return modal.ShowDialog(owner) == DialogResult.OK;
        }

        // ====== UI 헬퍼 ======
        private AppButton CreateTabButton(string text)
        {
            return new AppButton
            {
                Text = text,
                Variant = ButtonVariant.Ghost,
                Small = true,
                Margin = new Padding(0, 0, 4, 0)
            };
        }

        private Panel CreateTabPanel(out TextField txtField, string placeholder)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Visible = false
            };

            txtField = new TextField
            {
                Placeholder = placeholder,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 8, 0, 8)
            };

            panel.Controls.Add(txtField);
            return panel;
        }

        private void ShowTab(string mealType)
        {
            _panelBreakfast.Visible = false;
            _panelLunch.Visible = false;
            _panelDinner.Visible = false;

            ButtonStyles.Apply(_btnBreakfast, ButtonVariant.Ghost, small: true);
            ButtonStyles.Apply(_btnLunch, ButtonVariant.Ghost, small: true);
            ButtonStyles.Apply(_btnDinner, ButtonVariant.Ghost, small: true);

            if (mealType == "BREAKFAST")
            {
                _panelBreakfast.Visible = true;
                ButtonStyles.Apply(_btnBreakfast, ButtonVariant.Primary, small: true);
            }
            if (mealType == "LUNCH")
            {
                _panelLunch.Visible = true;
                ButtonStyles.Apply(_btnLunch, ButtonVariant.Primary, small: true);
            }
            if (mealType == "DINNER")
            {
                _panelDinner.Visible = true;
                ButtonStyles.Apply(_btnDinner, ButtonVariant.Primary, small: true);
            }

            FitCardSize();
        }

        private static TextField AddField(TableLayoutPanel parent, string label, string value, string placeholder, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var field = new TextField
            {
                FieldLabel = label,
                Text = value,
                Placeholder = placeholder,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };

            parent.Controls.Add(field, 0, row);
            return field;
        }

        // ====== 데이터 변환 ======
        private string JsonToText(string? menu)
        {
            if (string.IsNullOrWhiteSpace(menu)) return "";
            return menu.Trim()
                       .Replace("[", "")
                       .Replace("]", "")
                       .Replace("\"", "")
                       .Trim();
        }

        private string TextToJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "[]";

            var quoted = new List<string>();
            foreach (var item in text.Split(','))
            {
                string trimmed = item.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    quoted.Add($"\"{trimmed}\"");
                }
            }
            return "[" + string.Join(", ", quoted) + "]";
        }
    }
}