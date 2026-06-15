using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Basic
{
    /// <summary>
    /// 각 View 상단에 올라가는 KPI 요약 카드 바.
    /// SetCards()로 구성, SetValues()로 값 갱신.
    /// </summary>
    public partial class KpiSummaryBar : UserControl
    {

        private readonly TableLayoutPanel _table;
        private KpiCard[] _cards;

        /// <summary>카드 인덱스를 인자로 전달. 뷰에서 구독해 필터 연동.</summary>
        public event EventHandler<int>? CardClicked;

        public KpiSummaryBar()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Top;
            Height = 128;
            BackColor = ThemeColors.Background;
            Padding = new Padding(0, 0, 0, 8);

            _table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            _table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            Controls.Add(_table);
        }

        /// <summary>
        /// 카드 구성. params 배열로 (제목, 강조색) 전달.
        /// </summary>
        public void SetCards(params(string title, Color accent)[] defs)
        {
            _table.Controls.Clear();
            _table.ColumnStyles.Clear();
            _table.ColumnCount = defs.Length;
            _cards = new KpiCard[defs.Length];

            for(int i = 0; i < defs.Length; i++)
            {
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / defs.Length));
            }

            for(int i = 0; i < defs.Length; i++)
            {
                var idx = i;
                var card = new KpiCard
                {
                    Title = defs[i].title,
                    Value = "-",
                    Accent = defs[i].accent,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0, 0, i < defs.Length - 1 ? 8 : 0, 0),
                    Cursor = Cursors.Hand
                };
                WireClickRecursive(card, i);

                _cards[i] = card;
                _table.Controls.Add(card, i, 0);
            }
        }

        /// <summary>단일 카드 값 업데이트.</summary>
        public void SetValue(int index, string value)
        {
            if(_cards != null && (uint)index < (uint)_cards.Length)
            {
                _cards[index].Value = value;
            }
        }

        /// <summary>모든 카드 값을 순서대로 일괄 업데이트.</summary>
        public void SetValues(params string[] values)
        {
            if(_cards == null)
            {
                return;
            }
            for(int i = 0; i < values.Length && i < _cards.Length; i++)
            {
                _cards[i].Value = values[i];
            }
        }

        public void SetSubtitle(int index, string subtitle)
        {
            if((uint)index < (uint)_cards.Length)
            {
                _cards[index].Subtitle = subtitle;
            }
        }

        private void WireClickRecursive(Control c, int index)
        {
            c.Click += (_, _) => CardClicked?.Invoke(this, index);
            foreach(Control child in c.Controls)
            {
                WireClickRecursive(child, index);
            }
        }
    }
}
