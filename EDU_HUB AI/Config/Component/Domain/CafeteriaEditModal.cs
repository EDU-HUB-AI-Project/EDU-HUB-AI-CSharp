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

        private TableLayoutPanel _mealGrid;
        private List<TextBox> _mealInputList = new List<TextBox>();

        private static readonly string[] MealTypes = { "BREAKFAST", "LUNCH", "DINNER" };
        private static readonly string[] MealLabels = { "조식", "중식", "석식" };

        public List<CafeteriaDto> Result { get; private set; } = new List<CafeteriaDto>();

        public CafeteriaEditModal(string date)
        {
            _date = date;
            ModalTitle = $"{date} 식단 수정";
            ConfirmText = "저장";

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0)
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _mealGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 2,
                RowCount = 3
            };
            _mealGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            _mealGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));

            for (int m = 0; m < 3; m++)
            {
                _mealGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                _mealGrid.Controls.Add(CreateHeaderCell(MealLabels[m]), 0, m);

                var txtInput = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Font = ThemeFonts.Body,
                    BackColor = ThemeColors.Surface,
                    BorderStyle = BorderStyle.None,
                    Multiline = true,
                    Height = 50,
                    Margin = new Padding(4)
                };

                _mealInputList.Add(txtInput);
                _mealGrid.Controls.Add(txtInput, 1, m);
            }

            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.Controls.Add(_mealGrid, 0, 0);

            Body.Controls.Add(stack);
            FitCardSize();
        }

        public void LoadExistingData(List<CafeteriaDto> existingList)
        {
            _existingList = existingList;

            foreach (var item in _existingList)
            {
                int mealIndex = -1;
                if (item.mealType == "BREAKFAST") mealIndex = 0;
                if (item.mealType == "LUNCH") mealIndex = 1;
                if (item.mealType == "DINNER") mealIndex = 2;
                if (mealIndex < 0) continue;

                _mealInputList[mealIndex].Text = JsonToText(item.menu);
            }

            FitCardSize();
        }

        protected override void OnConfirm()
        {
            Result = BuildResult();
            base.OnConfirm();
        }

        private List<CafeteriaDto> BuildResult()
        {
            var result = new List<CafeteriaDto>();

            for (int m = 0; m < 3; m++)
            {
                string mealType = MealTypes[m];
                string menuText = _mealInputList[m].Text.Trim();

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
                    if (existing != null)
                    {
                        existing.delYn = "Y";
                        result.Add(existing);
                    }
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

        public static bool Show(IWin32Window owner, string date)
        {
            using var modal = new CafeteriaEditModal(date);
            return modal.ShowDialog(owner) == DialogResult.OK;
        }

        private Label CreateHeaderCell(string text)
        {
            return new Label
            {
                Text = text,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                BackColor = ThemeColors.Background,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Margin = new Padding(2)
            };
        }

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