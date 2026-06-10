using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public class TransportTypeCard : UserControl
    {
        private readonly string _typeCode;
        private readonly Label _lblTitle;
        private readonly Label _lblRoute;
        private readonly AppDataGrid _grid;
        private readonly AppButton _btnCreate;
        private List<TransportDto> _items = [];

        public event EventHandler? CreateRequested;
        public event EventHandler<TableActionEventArgs>? ActionRequested;

        public string TypeCode => _typeCode;

        public TransportTypeCard(string typeCode)
        {
            _typeCode = typeCode;

            BackColor = ThemeColors.Surface;
            Padding = new Padding(0);
            MinimumSize = new Size(360, 280);
            Margin = new Padding(0, 0, 16, 16);

            // ── 헤더 ───────────────────────────────────────────
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(16, 12, 16, 0)
            };

            _lblTitle = new Label
            {
                Text = TransportTypes.GetLabel(typeCode),
                Font = new Font(ThemeFonts.Body.FontFamily, 11F, FontStyle.Bold),
                ForeColor = ThemeColors.Text,
                AutoSize = true,
                Location = new Point(16, 12)
            };

            _lblRoute = new Label
            {
                Text = "-",
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = false,
                Location = new Point(16, 38),
                Size = new Size(400, 22)
            };

            _btnCreate = new AppButton
            {
                Text = "등록",
                Variant = ButtonVariant.Primary,
                Small = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(280, 12)
            };
            _btnCreate.Click += (_, _) => CreateRequested?.Invoke(this, EventArgs.Empty);

            // 헤더 하단 구분선
            var divider = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = ThemeColors.TableBorder
            };

            header.Controls.Add(_lblTitle);
            header.Controls.Add(_lblRoute);
            header.Controls.Add(_btnCreate);
            header.Controls.Add(divider);

            // ── 그리드 ─────────────────────────────────────────
            _grid = new AppDataGrid
            {
                Dock = DockStyle.Fill,
                RowTemplate = { Height = 44 },
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ThemeColors.Surface,
                BorderStyle = BorderStyle.None,
                GridColor = ThemeColors.TableBorder,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeight = 38,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ThemeColors.TableHeader,
                    ForeColor = ThemeColors.Text,
                    Font = ThemeFonts.TableHeader,
                    SelectionBackColor = ThemeColors.TableHeader,
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(12, 0, 8, 0)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ThemeColors.Surface,
                    ForeColor = ThemeColors.Text,
                    Font = ThemeFonts.TableCell,
                    SelectionBackColor = ThemeColors.TableHover,
                    SelectionForeColor = ThemeColors.Text,
                    Padding = new Padding(12, 0, 8, 0),
                    WrapMode = DataGridViewTriState.False
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ThemeColors.TableStripe
                }
            };

            SetupGrid();

            Controls.Add(_grid);
            Controls.Add(header);

            Resize += (_, _) => _btnCreate.Left = Math.Max(0, Width - _btnCreate.Width - 16);
        }

        private void SetupGrid()
        {
            _grid.Columns.Add("departTime", "출발 시간");
            _grid.Columns["departTime"].FillWeight = 60;

            if (TransportTypes.UsesArriveTime(_typeCode))
            {
                _grid.Columns.Add("arriveTime", "도착 시간");
                _grid.Columns["arriveTime"].FillWeight = 60;
            }

            _grid.Columns.Add("departLocation", "출발지");
            _grid.Columns.Add("destination", "도착지");

            _grid.AddTextActionColumns();
            _grid.ActionClicked += (s, e) => ActionRequested?.Invoke(this, e);
        }

        public void BindItems(IEnumerable<TransportDto> items)
        {
            _items = items
                .Where(i => string.Equals(i.type, _typeCode, StringComparison.OrdinalIgnoreCase))
                .OrderBy(i => i.departTime)
                .ToList();

            var first = _items.FirstOrDefault();
            _lblRoute.Text = first == null
                ? "등록된 운행이 없습니다."
                : $"출발: {first.departLocation}  →  목적지: {first.destination}";

            _grid.SuspendLayout();
            _grid.Rows.Clear();
            foreach (var item in _items)
            {
                if (TransportTypes.UsesArriveTime(_typeCode))
                {
                    _grid.Rows.Add(
                        item.departTime ?? "-",
                        string.IsNullOrWhiteSpace(item.arriveTime) ? "-" : item.arriveTime,
                        item.departLocation ?? "-",
                        item.destination ?? "-");
                }
                else
                {
                    _grid.Rows.Add(
                        item.departTime ?? "-",
                        item.departLocation ?? "-",
                        item.destination ?? "-");
                }
            }
            _grid.ResumeLayout();
        }

        public TransportDto? GetItemAt(int rowIndex) =>
            rowIndex >= 0 && rowIndex < _items.Count ? _items[rowIndex] : null;
    }
}