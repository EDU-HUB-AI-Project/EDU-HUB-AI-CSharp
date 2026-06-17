using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.View
{
    public partial class DormitoryView : UserControl
    {
        // ── 데이터 ──────────────────────────────────────────────
        private List<DormAssignDto> _assignData = new();
        private List<DormInOutDto> _waitingData = new();
        private List<DormInOutDto> _inOutData = new();
        private List<DormitoryDto> _rooms = new();

        private List<DormAssignDto> _assignPage = new();
        private List<DormInOutDto> _waitingPage = new();
        private List<DormInOutDto> _inOutPage = new();

        private readonly AdminEduInfoController _eduInfoController = new();
        private Dictionary<string, string> _eduMap = new();

        // ── 상태 ────────────────────────────────────────────────
        private string? _selectedDormId = null;
        private string _activeTabKey = "assign";
        private bool _isLoading = false;

        // ── 그리드 / 페이징 ─────────────────────────────────────
        private AppDataGrid _assignGrid = null!;
        private AppDataGrid _waitingGrid = null!;
        private AppDataGrid _inOutGrid = null!;
        private Pagination _assignPg = null!;
        private Pagination _waitingPg = null!;
        private Pagination _inOutPg = null!;

        // ── 컴포넌트 ──────────────────────────────
        private TabStrip _tabStrip = null!;
        private TextField _searchField = null!;
        private Panel _roomListPanel = null!;

        private readonly AdminDormitoryController _adminDormitoryController = new();

        // ── 정렬 ──────────────────────────────
        private string _assignSortCol = "studentName"; private bool _assignSortAsc = true;
        private string _waitingSortCol = "studentName"; private bool _waitingSortAsc = true;
        private string _inOutSortCol = "studentName"; private bool _inOutSortAsc = true;

        // ── KPI ──────────────────────────────
        private readonly KpiSummaryBar _kpiBar = new();

        public DormitoryView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;
            bodyPanel.BackColor = ThemeColors.Background;
            pageHeader1.SyncClicked += (_, _) => _ = LoadAll();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BuildLayout();
            BeginInvoke(() => _ = LoadAll());
        }

        // ── 레이아웃 빌드 ──────────────────────────────
        private void BuildLayout()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 1,
                BackColor = ThemeColors.Border,
                FixedPanel = FixedPanel.Panel1
            };
            BuildLeftPanel(split.Panel1);
            BuildRightPanel(split.Panel2);
            bodyPanel.Controls.Add(split);
            bodyPanel.Controls.Add(_kpiBar);
            split.SplitterDistance = 280;
        }

        // ── 왼쪽 ──────────────────────────────
        private void BuildLeftPanel(SplitterPanel parent)
        {
            parent.BackColor = ThemeColors.Surface;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = ThemeColors.Surface
            };
            header.Controls.Add(new Label
            {
                Text = "호실 목록",
                Font = ThemeFonts.Panel,
                ForeColor = ThemeColors.Text,
                AutoSize = true,
                Location = new Point(16, 16)
            });
            header.Paint += (_, e2) =>
            {
                using var pen = new Pen(ThemeColors.Border, 1);
                e2.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            var searchWrap = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(12, 10, 12, 10)
            };
            _searchField = new TextField
            {
                Dock = DockStyle.Fill,
                FieldLabel = "",
                Placeholder = "호실 검색..."
            };
            _searchField.TextChanged += (_, _) => RenderRoomList();
            searchWrap.Controls.Add(_searchField);

            _roomListPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ThemeColors.Surface
            };
            _roomListPanel.Resize += (_, _) => RelayoutRoomItems();

            parent.Controls.Add(_roomListPanel);
            parent.Controls.Add(searchWrap);
            parent.Controls.Add(header);
        }

        // ──────── 오른쪽 패널 ──────────────────────────────────
        private void BuildRightPanel(SplitterPanel parent)
        {
            parent.BackColor = ThemeColors.Background;

            _tabStrip = new TabStrip { Dock = DockStyle.Fill };
            _tabStrip.AddTab("assign", "배정 현황", BuildGridPanel("assign"));
            _tabStrip.AddTab("waiting", "입실 대기", BuildGridPanel("waiting"));
            _tabStrip.AddTab("inout", "입/퇴실 현황", BuildGridPanel("inout"));
            _tabStrip.TabChanged += (_, key) =>
            {
                _activeTabKey = key;
                RefreshActiveTab();
            };

            parent.Controls.Add(_tabStrip);
        }

        private Panel BuildGridPanel(string tabKey)
        {
            var grid = new AppDataGrid();
            var pg = new Pagination
            {
                Dock = DockStyle.Bottom,
                Height = 76
            };

            switch (tabKey)
            {
                case "assign":
                    grid.Columns.Add("studentName", "이름");
                    grid.Columns.Add("eduId", "교육과정");
                    grid.Columns.Add("phone", "연락처");
                    grid.Columns.Add("dormitoryRoomName", "호실");
                    grid.Columns.Add("assignStatus", "배정상태");
                    grid.AddTextActionColumns(true, false);
                    if (grid.Columns[AppDataGrid.EditColumnName] is DataGridViewLinkColumn c0) c0.Text = "배정";
                    grid.SetInitialSort("studentName");
                    grid.SortChanged += (_, s) =>
                    {
                        _assignSortCol = s.Column;
                        _assignSortAsc = s.Ascending;
                        RefreshActiveTab();
                    };
                    grid.ActionClicked += OnRowActionAssign;
                    grid.CellFormatting += OnCellFormatting;
                    _assignGrid = grid; _assignPg = pg;
                    pg.PageChanged += (_, p2) => _assignPage = RenderGrid(SortAssign(_FilterAssign()), _assignPg, p2, _assignGrid,
                        r => _assignGrid.Rows.Add(r.studentName, _eduMap.GetValueOrDefault(r.eduId, r.eduId), PhoneHelper.Format(r.phone), r.dormitoryRoomName, r.assignStatus));
                    break;

                case "waiting":
                    grid.Columns.Add("studentName", "이름");
                    grid.Columns.Add("dormitoryRoomName", "호실");
                    grid.AddTextActionColumns(true, false);
                    if (grid.Columns[AppDataGrid.EditColumnName] is DataGridViewLinkColumn c1) c1.Text = "입실";
                    grid.SetInitialSort("studentName");
                    grid.SortChanged += (_, s) =>
                    {
                        _waitingSortCol = s.Column;
                        _waitingSortAsc = s.Ascending;
                        RefreshActiveTab();
                    };
                    grid.ActionClicked += OnRowActionWaiting;
                    grid.CellFormatting += OnCellFormatting;
                    _waitingGrid = grid; _waitingPg = pg;
                    pg.PageChanged += (_, p2) => _waitingPage = RenderGrid(SortInOut(_FilterWaiting(), _waitingSortCol, _waitingSortAsc), _waitingPg, p2, _waitingGrid,
                        r => _waitingGrid.Rows.Add(r.studentName, r.dormitoryRoomName));
                    break;

                case "inout":
                    grid.Columns.Add("studentName", "이름");
                    grid.Columns.Add("dormitoryRoomName", "호실");
                    grid.Columns.Add("checkIn", "입실일");
                    grid.Columns.Add("checkOut", "퇴실일");
                    grid.AddTextActionColumns(true, false);
                    if (grid.Columns[AppDataGrid.EditColumnName] is DataGridViewLinkColumn c2) c2.Text = "퇴실";
                    grid.SetInitialSort("studentName");
                    grid.SortChanged += (_, s) =>
                    {
                        _inOutSortCol = s.Column;
                        _inOutSortAsc = s.Ascending;
                        RefreshActiveTab();
                    };
                    grid.ActionClicked += OnRowActionDormOut;
                    grid.CellFormatting += OnCellFormatting;
                    _inOutGrid = grid; _inOutPg = pg;
                    pg.PageChanged += (_, p2) => _inOutPage = RenderGrid(SortInOut(_FilterInOut(), _inOutSortCol, _inOutSortAsc), _inOutPg, p2, _inOutGrid,
                        r => _inOutGrid.Rows.Add(r.studentName, r.dormitoryRoomName, r.checkIn, r.checkOut));
                    break;
            }

            grid.Dock = DockStyle.Fill;
            var panel = new Panel { Dock = DockStyle.Fill };
            panel.Controls.Add(pg);
            panel.Controls.Add(grid);
            return panel;
        }


        // ──────── 데이터 로드 ──────────────────────────────────
        private async Task LoadAll(bool showOverlay = true)
        {
            if(IsDisposed || !IsHandleCreated)
            {
                return;
            }
            if(_isLoading)
            {
                return;
            }
            _isLoading = true;

            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩중...") : null;

            try
            {
                _adminDormitoryController.OnRetry = (a, m) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {a}/{m}");

                var t1 = _adminDormitoryController.GetDormAssign();
                var t2 = _adminDormitoryController.GetDormWaiting();
                var t3 = _adminDormitoryController.GetDormInOut();
                var t4 = _adminDormitoryController.GetDormRoomAssignStatus(null);
                var t5 = _eduInfoController.GetEduInfos();

                await Task.WhenAll(t1, t2, t3, t4, t5);

                if (IsDisposed || !IsHandleCreated) return;

                if (t1.Result?.Status == 200) _assignData = t1.Result.Data;
                if (t2.Result?.Status == 200) _waitingData = t2.Result.Data;
                if (t3.Result?.Status == 200) _inOutData = t3.Result.Data;
                if (t4.Result?.Status == 200) _rooms = t4.Result.Data;
                if (t5.Result?.Status == 200)
                    _eduMap = t5.Result.Data.ToDictionary(e => e.eduId, e => e.eduName);

                UpdateKpi();
                RenderRoomList();
                RefreshActiveTab();
            }
            catch (ApiException ex)
            {
                if (!IsDisposed) MessageBox.Show(ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                if (!IsDisposed) MessageBox.Show($"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminDormitoryController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
                _isLoading = false;
            }
        }

        private void UpdateKpi()
        {
            var total = _rooms.Count;
            var assigned = _assignData.Count;
            var empty = Math.Max(0, total - assigned);
            var rate = total > 0 ? (int)Math.Round((double)empty / total * 100) : 0;

            _kpiBar.SetCards(
                ("전체 호실", ThemeColors.Primary),
                ("배정 완료", ThemeColors.Ok),
                ("공실", ThemeColors.Warn),
                ("공실률", ThemeColors.Danger),
                ("입실 대기", ThemeColors.InfoText)
            );
            _kpiBar.SetValues(
                $"{total}호실",
                $"{assigned}호실",
                $"{empty}호실",
                $"{rate}%",
                $"{_waitingData.Count}명"
            );
        }

        // ──────── 호실 목록 렌더 ──────────────────────────────────
        private void RenderRoomList()
        {
            string kw = _searchField?.Text.Trim() ?? "";
            var list = string.IsNullOrEmpty(kw)
                ? _rooms
                : _rooms.Where(r => r.dormitoryRoomName?.Contains(kw, StringComparison.OrdinalIgnoreCase) == true).ToList();

            _roomListPanel.SuspendLayout();
            foreach (Control c in _roomListPanel.Controls) c.Dispose();
            _roomListPanel.Controls.Clear();

            _roomListPanel.Controls.Add(MakeRoomItem("전체 학생", -1, -1, _selectedDormId == null, () => SelectRoom(null)));

            foreach (var room in list)
            {
                string id = room.dormitoryId;
                _roomListPanel.Controls.Add(MakeRoomItem(
                    room.dormitoryRoomName, room.currentCount, room.maxCount,
                    _selectedDormId == room.dormitoryId, () => SelectRoom(id)));
            }

            RelayoutRoomItems();
            _roomListPanel.ResumeLayout();
        }

        private void RelayoutRoomItems()
        {
            int y = 0;
            int w = _roomListPanel.ClientSize.Width;
            foreach (Control c in _roomListPanel.Controls)
            {
                c.SetBounds(0, y, w, c.Height);
                y += c.Height;
            }
            _roomListPanel.AutoScrollMinSize = new Size(0, y);
        }

        private Panel MakeRoomItem(string name, int cur, int max, bool selected, Action onSelect)
        {
            bool hasBar = max > 0;
            var item = new Panel
            {
                Height = hasBar ? 80 : 52,
                BackColor = selected ? ThemeColors.InfoBg : ThemeColors.Surface,
                Cursor = Cursors.Hand
            };
            item.Paint += (_, e2) =>
            {
                using var pen = new Pen(ThemeColors.Border, 1);
                e2.Graphics.DrawLine(pen, 0, item.Height - 1, item.Width, item.Height - 1);
                if (selected)
                {
                    using var accent = new SolidBrush(ThemeColors.Primary);
                    e2.Graphics.FillRectangle(accent, 0, 0, 4, item.Height);
                }
            };

            var lblName = new Label
            {
                Text = name,
                Font = selected ? new Font(ThemeFonts.Body, FontStyle.Bold) : ThemeFonts.Body,
                ForeColor = selected ? ThemeColors.Primary : ThemeColors.Text,
                AutoSize = false,
                Height = 24,
                Location = new Point(16, hasBar ? 8 : 14),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                AutoEllipsis = true
            };
            item.Controls.Add(lblName);
            item.Resize += (_, _) => lblName.Width = item.Width - 32;

            if (hasBar)
            {
                float ratio = max > 0 ? Math.Min(1f, (float)cur / max) : 0f;
                Color barColor = ratio >= 1f ? ThemeColors.Danger : ratio >= 0.8f ? ThemeColors.Warn : ThemeColors.Ok;

                var lblCnt = new Label
                {
                    Text = $"{cur} / {max}명",
                    Font = ThemeFonts.BodySm,
                    ForeColor = ratio >= 1f ? ThemeColors.Danger : ThemeColors.TextMuted,
                    AutoSize = true,
                    Location = new Point(16, 34)
                };

                var bar = new Panel
                {
                    Location = new Point(16, 62),
                    Height = 5,
                    BackColor = ThemeColors.Border,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                };
                bar.Width = Math.Max(1, item.Width - 32);
                bar.Paint += (_, e2) =>
                {
                    int filled = (int)(bar.Width * ratio);
                    if (filled > 0)
                    {
                        using var b = new SolidBrush(barColor);
                        e2.Graphics.FillRectangle(b, 0, 0, filled, bar.Height);
                    }
                };
                item.Resize += (_, _) => { bar.Width = Math.Max(1, item.Width - 32); bar.Invalidate(); };

                item.Controls.Add(lblCnt);
                item.Controls.Add(bar);
            }

            item.Click += (_, _) => onSelect();
            foreach (Control c in item.Controls)
            {
                c.Click += (_, _) => onSelect();
                c.Cursor = Cursors.Hand;
            }
            return item;
        }

        // ──────── 호실 선택 ──────────────────────────────────
        private void SelectRoom(string? dormId)
        {
            _selectedDormId = dormId;

            RenderRoomList();
            RefreshActiveTab();
        }

        private void RefreshActiveTab()
        {
            if (_assignGrid == null) return;
            switch (_activeTabKey)
            {
                case "assign":
                    _assignPage = RenderGrid(SortAssign(_FilterAssign()), _assignPg, 1, _assignGrid,
                        r => _assignGrid.Rows.Add(r.studentName, _eduMap.GetValueOrDefault(r.eduId, r.eduId), PhoneHelper.Format(r.phone), r.dormitoryRoomName, r.assignStatus));
                    break;
                case "waiting":
                    _waitingPage = RenderGrid(SortInOut(_FilterWaiting(), _waitingSortCol, _waitingSortAsc), _waitingPg, 1, _waitingGrid,
                        r => _waitingGrid.Rows.Add(r.studentName, r.dormitoryRoomName));
                    break;
                case "inout":
                    _inOutPage = RenderGrid(SortInOut(_FilterInOut(), _inOutSortCol, _inOutSortAsc), _inOutPg, 1, _inOutGrid,
                        r => _inOutGrid.Rows.Add(r.studentName, r.dormitoryRoomName, r.checkIn, r.checkOut));
                    break;
            }
        }

        private List<DormAssignDto> _FilterAssign() =>
            _selectedDormId == null ? _assignData
            : _assignData.Where(r => r.dormitoryId == _selectedDormId).ToList();

        private List<DormInOutDto> _FilterWaiting() =>
            _selectedDormId == null ? _waitingData
            : _waitingData.Where(r => r.dormitoryId == _selectedDormId).ToList();

        private List<DormInOutDto> _FilterInOut() =>
            _selectedDormId == null ? _inOutData
            : _inOutData.Where(r => r.dormitoryId == _selectedDormId).ToList();

        // ──────── 공통 그리드 렌더 ──────────────────────────────────
        private static List<T> RenderGrid<T>(List<T> data, Pagination pg, int page, AppDataGrid grid, Action<T> addRow)
        {
            pg.TotalCount = data.Count;
            int size = pg.PageSize;
            int total = Math.Max(1, (int)Math.Ceiling(data.Count / (double)size));
            page = Math.Clamp(page, 1, total);
            pg.PageIndex = page;

            var items = data.Skip((page - 1) * size).Take(size).ToList();
            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach (var item in items) addRow(item);
            grid.ResumeLayout();
            return items;
        }

        // ──────── Row 액션 핸들러 ──────────────────────────────────
        private async void OnRowActionAssign(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _assignPage.Count || e.Action != TableAction.Edit) return;
            var target = _assignPage[e.RowIndex];

            var edited = DormAssignModal.Show(this.FindForm(), target);
            if (edited == null) return;

            await RunWithOverlay("수정 중...", async () =>
            {
                var res = await _adminDormitoryController.UpdateDormId(target.studentId, edited);
                if (res?.Status == 200)
                {
                    var idx = _assignData.IndexOf(target);
                    if (idx >= 0) _assignData[idx] = edited;
                    _ = LoadAll(showOverlay: false);
                }
                else MessageBox.Show(res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private async void OnRowActionWaiting(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _waitingPage.Count || e.Action != TableAction.Edit) return;
            var target = _waitingPage[e.RowIndex];

            if (!ConfirmModal.Show(this.FindForm(), "입실 확인",
                    $"{target.studentName} 학생을 입실 처리하시겠습니까?", "입실", ButtonVariant.Primary))
                return;

            await RunWithOverlay("처리 중...", async () =>
            {
                var res = await _adminDormitoryController.UpdateDormCurrentCnt(target.studentId,
                              new DormitoryDto { dormitoryId = target.dormitoryId });
                if (res?.Status == 200) _ = LoadAll(showOverlay: false);
                else MessageBox.Show(res?.Message ?? "처리에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private async void OnRowActionDormOut(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _inOutPage.Count || e.Action != TableAction.Edit) return;
            var target = _inOutPage[e.RowIndex];

            if (!ConfirmModal.Show(this.FindForm(), "퇴실 확인",
                    $"{target.studentName} 학생을 퇴실 처리하시겠습니까?", "퇴실", ButtonVariant.Danger))
                return;

            await RunWithOverlay("처리 중...", async () =>
            {
                var res = await _adminDormitoryController.UpdateDormCurrentCntDown(target.studentId,
                              new DormitoryDto { dormitoryId = target.dormitoryId });
                if (res?.Status == 200) _ = LoadAll(showOverlay: false);
                else MessageBox.Show(res?.Message ?? "처리에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        // ──────── 공통 오버레이 래퍼 ──────────────────────────────────
        private async Task RunWithOverlay(string msg, Func<Task> action)
        {
            var overlay = LoadingOverlay.Create(bodyPanel, msg);
            _adminDormitoryController.OnRetry = (a, m) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {a}/{m}");
            try
            {
                await action();
            }
            catch (ApiException ex) { if (!IsDisposed) MessageBox.Show(ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception ex) { if (!IsDisposed) MessageBox.Show($"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally
            {
                _adminDormitoryController.OnRetry = null;
                overlay.Close();
                overlay.Dispose();
            }
        }

        // ──────── 셀 포멧 ──────────────────────────────────
        private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || sender is not AppDataGrid grid) return;
            var col = grid.Columns[e.ColumnIndex].Name;
            if (col is "dormitoryRoomName" or "phone")
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            if (col == "dormitoryRoomName" && e.Value is string room && !string.IsNullOrEmpty(room))
            {
                e.Value = room + "호";
                e.FormattingApplied = true;
            }
        }

        // ──────── 정렬 ──────────────────────────────────
        private List<DormAssignDto> SortAssign(List<DormAssignDto> data)
        {
            Func<DormAssignDto, object?> key = _assignSortCol switch
            {
                "studentName" => d => d.studentName,
                "eduId" => d => d.eduId,
                "phone" => d => d.phone,
                "dormitoryRoomName" => d => d.dormitoryRoomName,
                "assignStatus" => d => d.assignStatus,
                _ => d => d.studentName
            };
            return _assignSortAsc ? data.OrderBy(key).ToList() : data.OrderByDescending(key).ToList();
        }

        private static List<DormInOutDto> SortInOut(List<DormInOutDto> data, string col, bool asc)
        {
            Func<DormInOutDto, object?> key = col switch
            {
                "studentName" => d => d.studentName,
                "dormitoryRoomName" => d => d.dormitoryRoomName,
                "checkIn" => d => d.checkIn,
                "checkOut" => d => d.checkOut,
                _ => d => d.studentName
            };
            return asc ? data.OrderBy(key).ToList() : data.OrderByDescending(key).ToList();
        }
    }
}
