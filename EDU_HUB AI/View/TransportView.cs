using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.View
{
    public partial class TransportView : UserControl
    {
        private List<TransportDto> _all = [];
        private List<TransportDto> _filtered = [];
        private List<TransportDto> _pageItems = [];
        private readonly AdminTransportController _controller = new();

        private string? _sortColumn = null;
        private bool _sortAscending = true;

        private readonly KpiSummaryBar _kpiBar = new(); 
        private string _selectedType = "";
        private bool _suppressTypeFilter;

        public TransportView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            grid.SortChanged += (_, s) =>
            {
                _sortColumn = s.Column;
                _sortAscending = s.Ascending;
                ApplyFilter();
                RenderPage(1);
            };

            SetupTypeFilter();

            actionPanel.Resize += (_, _) => AlignCreateButton();

            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;

            bodyPanel.Controls.Add(_kpiBar);
            _kpiBar.SetCards(
                ("KTX", ThemeColors.Primary),
                ("SRT", ThemeColors.Primary),
                ("고속·시외버스", ThemeColors.Ok),
                ("공항", ThemeColors.Warn),
                ("셔틀버스", ThemeColors.Sync)
            );
            _kpiBar.CardClicked += (_, index) =>
            {
                string[] map = { "KTX", "SRT", "EXBUS", "AIRPORT", "SHUTTLE" };
                if (index < map.Length)
                    SelectTypeFilter(map[index]);
            };

            tableCard.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, tableCard.Width - 1, tableCard.Height - 1);
            };
            filterCard.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, filterCard.Width - 1, filterCard.Height - 1);
            };

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender();
            btnCreate.Click += OnCreate;
            pagination1.PageChanged += (_, page) => RenderPage(page);

            grid.PageNavigationRequested += (_, nav) =>
            {
                switch (nav)
                {
                    case PageNavigation.Next: pagination1.GoToNext(); break;
                    case PageNavigation.Prev: pagination1.GoToPrev(); break;
                    case PageNavigation.First: pagination1.GoToFirst(); break;
                    case PageNavigation.Last: pagination1.GoToLast(); break;
                }
            };
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            AlignCreateButton();
            grid.SetInitialSort("departTime", true);
            await LoadAndRender();
            grid.Focus();
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(tableCard, 0);
            bodyPanel.Controls.SetChildIndex(gapPanel, 1);
            bodyPanel.Controls.SetChildIndex(filterCard, 2);
            bodyPanel.Controls.SetChildIndex(_kpiBar, 3);
            bodyPanel.Controls.SetChildIndex(pagination1, 4);
        }

        private void SetupTypeFilter()
        {
            _suppressTypeFilter = true;
            try
            {
                rbTypeAll.CheckedChanged += (_, _) => OnTypeRadioChanged();
                rbTypeKtx.CheckedChanged += (_, _) => OnTypeRadioChanged();
                rbTypeSrt.CheckedChanged += (_, _) => OnTypeRadioChanged();
                rbTypeExbus.CheckedChanged += (_, _) => OnTypeRadioChanged();
                rbTypeAirport.CheckedChanged += (_, _) => OnTypeRadioChanged();
                rbTypeShuttle.CheckedChanged += (_, _) => OnTypeRadioChanged();

                SelectTypeFilter("");
            }
            finally
            {
                _suppressTypeFilter = false;
            }
        }

        private void OnTypeRadioChanged()
        {
            if (_suppressTypeFilter) return;
            if (!IsHandleCreated) return;

            _selectedType =
                rbTypeKtx.Checked ? "KTX" :
                rbTypeSrt.Checked ? "SRT" :
                rbTypeExbus.Checked ? "EXBUS" :
                rbTypeAirport.Checked ? "AIRPORT" :
                rbTypeShuttle.Checked ? "SHUTTLE" :
                "";

            ApplyFilter();
            RenderPage(1);
        }

        private void SelectTypeFilter(string type)
        {
            _suppressTypeFilter = true;
            try
            {
                _selectedType = type ?? "";

                rbTypeAll.Checked = string.IsNullOrEmpty(_selectedType);
                rbTypeKtx.Checked = string.Equals(_selectedType, "KTX", StringComparison.OrdinalIgnoreCase);
                rbTypeSrt.Checked = string.Equals(_selectedType, "SRT", StringComparison.OrdinalIgnoreCase);
                rbTypeExbus.Checked = string.Equals(_selectedType, "EXBUS", StringComparison.OrdinalIgnoreCase);
                rbTypeAirport.Checked = string.Equals(_selectedType, "AIRPORT", StringComparison.OrdinalIgnoreCase);
                rbTypeShuttle.Checked = string.Equals(_selectedType, "SHUTTLE", StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                _suppressTypeFilter = false;
            }

            ApplyFilter();
            RenderPage(1);
        }

        private void AlignCreateButton()
        {
            btnCreate.PerformLayout();
            pnlButtonHost.Width = btnCreate.Width + pnlButtonHost.Padding.Horizontal;
            btnCreate.Location = new Point(
                pnlButtonHost.ClientSize.Width - btnCreate.Width,
                (pnlButtonHost.ClientSize.Height - btnCreate.Height) / 2);
        }

        private void SetupGrid()
        {
            grid.Columns.Add("typeLabel", "교통수단");
            grid.Columns.Add("departLocation", "출발지");
            grid.Columns.Add("destination", "목적지");
            grid.Columns.Add("departTime", "출발");
            grid.Columns.Add("arriveTime", "도착");

            grid.Columns["typeLabel"].FillWeight = 120;
            grid.Columns["departLocation"].FillWeight = 200;
            grid.Columns["destination"].FillWeight = 200;
            grid.Columns["departTime"].FillWeight = 90;
            grid.Columns["arriveTime"].FillWeight = 90;

            grid.AddTextActionColumns();
            grid.ActionClicked += OnRowAction;
        }

        // ===== 데이터 =====

        private async Task<List<TransportDto>> LoadData()
        {
            var res = await _controller.GetTransportList();
            return res?.Data ?? [];
        }

        private async Task LoadAndRender(bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩 중...") : null;
            _controller.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                _all = await LoadData();
                UpdateKpi();
                ApplyFilter();
                RenderPage(1);
            }
            finally
            {
                _controller.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        private void UpdateKpi()
        {
            _kpiBar.SetValues(
                $"{_all.Count(t => t.type == "KTX")}건",
                $"{_all.Count(t => t.type == "SRT")}건",
                $"{_all.Count(t => t.type == "EXBUS")}건",
                $"{_all.Count(t => t.type == "AIRPORT")}건",
                $"{_all.Count(t => t.type == "SHUTTLE")}건"
            );
        }

        // ===== 필터 / 렌더 =====

        private void ApplyFilter()
        {
            var type = _selectedType;
            var result = string.IsNullOrEmpty(type)
                ? _all.AsEnumerable()
                : _all.Where(t => string.Equals(t.type, type, StringComparison.OrdinalIgnoreCase));

            if (_sortColumn != null)
            {
                Func<TransportDto, object?> key = _sortColumn switch
                {
                    "typeLabel" => t => t.type,
                    "departLocation" => t => t.departLocation,
                    "destination" => t => t.destination,
                    "departTime" => t => t.departTime,
                    "arriveTime" => t => t.arriveTime,
                    _ => t => null
                };
                _filtered = _sortAscending ? result.OrderBy(key).ToList() : result.OrderByDescending(key).ToList();
            }
            else
            {
                _filtered = string.IsNullOrEmpty(type)
                    ? result.OrderBy(t => t.type).ThenBy(t => t.departTime).ToList()
                    : result.OrderBy(t => t.departTime).ToList();
            }
        }

        private void RenderPage(int page)
        {
            pagination1.TotalCount = _filtered.Count;
            var size = pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_filtered.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination1.PageIndex = page;

            _pageItems = _filtered.Skip((page - 1) * size).Take(size).ToList();

            grid.SuspendLayout();
            grid.Rows.Clear();

            foreach (var t in _pageItems)
            {
                var idx = grid.Rows.Add(
                    TransportTypes.GetLabel(t.type),
                    t.departLocation ?? "-",
                    t.destination ?? "-",
                    t.departTime ?? "-",
                    TransportTypes.UsesArriveTime(t.type) ? (t.arriveTime ?? "-") : "-");
                grid.Rows[idx].Tag = t;
            }

            grid.ResumeLayout();
        }

        // ===== CRUD =====

        private async void OnCreate(object? sender, EventArgs e)
        {
            var created = TransportEditModal.Show(FindForm(), null);
            if (created == null) return;

            await ExecuteWithOverlay("등록 중...", async _ =>
            {
                var res = await _controller.InsertTransport(created);
                if (res?.Status == 200)
                    await LoadAndRender(showOverlay: false);
                else
                    MessageBox.Show(FindForm(), res?.Message ?? "등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.Action == TableAction.New)
            {
                OnCreate(sender, EventArgs.Empty);
                return;
            }

            if (e.Tag is not TransportDto target) return;

            if (e.Action == TableAction.Edit)
                await DoEdit(target);
            else if (e.Action == TableAction.Delete)
                await DoDelete(target);
        }

        private async Task DoEdit(TransportDto target)
        {
            var edited = TransportEditModal.Show(FindForm(), target);
            if (edited == null || string.IsNullOrWhiteSpace(target.transportId)) return;

            await ExecuteWithOverlay("수정 중...", async _ =>
            {
                var res = await _controller.UpdateTransport(target.transportId, edited);
                if (res?.Status == 200)
                {
                    var idx = _all.IndexOf(target);
                    if (idx >= 0) _all[idx] = edited;
                    ApplyFilter();
                    RenderPage(pagination1.PageIndex);
                }
                else
                    MessageBox.Show(FindForm(), res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private async Task DoDelete(TransportDto target)
        {
            var label = $"{TransportTypes.GetLabel(target.type)} {target.departTime}";
            if (!ConfirmModal.Show(FindForm(), "삭제 확인", $"'{label}' 운행을 삭제할까요?")) return;
            if (string.IsNullOrWhiteSpace(target.transportId)) return;

            await ExecuteWithOverlay("삭제 중...", async _ =>
            {
                var res = await _controller.DeleteTransport(target.transportId);
                if (res?.Status == 200)
                {
                    _all.Remove(target);
                    ApplyFilter();
                    RenderPage(pagination1.PageIndex);
                }
                else
                    MessageBox.Show(FindForm(), res?.Message ?? "삭제에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        // ===== 공통 =====

        private async Task ExecuteWithOverlay(string message, Func<LoadingOverlay, Task> action)
        {
            var overlay = LoadingOverlay.Create(bodyPanel, message);
            _controller.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try { await action(overlay); }
            catch (ApiException ex) { MessageBox.Show(FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception ex) { MessageBox.Show(FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { _controller.OnRetry = null; overlay.Close(); overlay.Dispose(); }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.Right: pagination1.GoToNext(); return true;
                case Keys.Control | Keys.Left: pagination1.GoToPrev(); return true;
                case Keys.Control | Keys.Home: pagination1.GoToFirst(); return true;
                case Keys.Control | Keys.End: pagination1.GoToLast(); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}