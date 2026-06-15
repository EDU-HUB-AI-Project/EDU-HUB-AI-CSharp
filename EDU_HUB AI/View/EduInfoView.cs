using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using System.Globalization;

namespace EDU_HUB_AI.View
{
    public partial class EduInfoView : UserControl, ISearchFocusable
    {
        private List<EduInfoDto> _all = new();
        private List<EduInfoDto> _pageItems = new();
        private readonly AdminEduInfoController _adminEduInfoController = new AdminEduInfoController();

        private List<EduInfoDto> _filtered = new();

        private string _sortColumn = "";
        private bool _sortAscending = false;

        public EduInfoView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupFilterSource();
            SetupGrid();

            grid.SortChanged += (_, s) => 
            {
                _sortColumn = s.Column; 
                _sortAscending = s.Ascending; 
                ApplyFilter(); 
                RenderPage(1); 
            };

            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;

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

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender(1);
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

            cmbStatus.SelectedIndexChanged += (_, _) => { ApplyFilter(); RenderPage(1); };
            txtSearch.TextChanged += (_, _) => { ApplyFilter(); RenderPage(1); };
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            await LoadAndRender(1);
            grid.Focus();
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(tableCard, 0);
            bodyPanel.Controls.SetChildIndex(gapPanel, 1);
            bodyPanel.Controls.SetChildIndex(filterCard, 2);
            bodyPanel.Controls.SetChildIndex(pagination1, 3);
        }

        // ===== 데이터 연동 지점 =====
        private async Task<List<EduInfoDto>> LoadData()
        {
            var res = await _adminEduInfoController.GetEduInfos();
            return res?.Data ?? new List<EduInfoDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩 중...") : null;

            _adminEduInfoController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                _all = await LoadData();
                ApplyFilter();
                RenderPage(page);
            }
            finally
            {
                _adminEduInfoController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        // ===== 그리드 =====
        private void SetupGrid()
        {
            grid.Columns.Add("eduName", "과정명");
            grid.Columns.Add("startDate", "시작일");
            grid.Columns.Add("endDate", "종료일");
            grid.Columns.Add("batchNumber", "기수");
            grid.Columns.Add("capacity", "정원");
            grid.Columns.Add("status", "상태");
            grid.AddTextActionColumns();

            grid.Columns["eduName"].FillWeight = 300;
            grid.Columns["startDate"].FillWeight = 120;
            grid.Columns["endDate"].FillWeight = 120;
            grid.Columns["batchNumber"].FillWeight = 70;
            grid.Columns["capacity"].FillWeight = 70;
            grid.Columns["status"].FillWeight = 80;

            grid.ActionClicked += OnRowAction;
            grid.CellFormatting += OnCellFormatting;
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

            foreach(var edu in _pageItems)
            {
                var idx = grid.Rows.Add(
                    edu.eduName, edu.startDate, edu.endDate,
                    $"{edu.batchNumber}기", edu.capacity, StatusLabel(edu));
                grid.Rows[idx].Tag = edu;
            }
            grid.ResumeLayout();
        }

        // ===== CRUD =====
        protected async void OnCreate(object? sender, EventArgs e)
        {
            var created = EduInfoEditModal.Show(this.FindForm(), null);
            if (created == null) return;

            var overlay = LoadingOverlay.Create(bodyPanel, "등록 중...");
            _adminEduInfoController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중... \n재시도 {attempt}/{max}");

            try
            {
                var res = await _adminEduInfoController.InsertEduInfo(created);
                if (res?.Status == 200)
                {
                    await LoadAndRender(int.MaxValue, showOverlay: false);
                }
                else
                {
                    MessageBox.Show(this.FindForm(), res?.Message ?? "등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(ApiException ex)
            {
                MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminEduInfoController.OnRetry = null;
                overlay.Close();
                overlay.Dispose();
            }
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if(e.Action == TableAction.New)
            {
                OnCreate(sender, EventArgs.Empty);
                return;
            }

            var target = e.Tag as EduInfoDto;
            if(target == null)
            {
                return;
            }

            if (e.Action == TableAction.Edit)
            {
                var edited = EduInfoEditModal.Show(this.FindForm(), target);
                if (edited == null) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _adminEduInfoController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _adminEduInfoController.UpdateEduInfo(target.eduId, edited);
                    if (res?.Status == 200)
                    {
                        var idx = _all.IndexOf(target);
                        if (idx >= 0) _all[idx] = edited;
                        ApplyFilter();
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                    {
                        MessageBox.Show(this.FindForm(), res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (ApiException ex)
                {
                    MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _adminEduInfoController.OnRetry = null;
                    overlay.Close();
                    overlay.Dispose();
                }
            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(this.FindForm(), "삭제 확인", $"'{target.eduName}'을(를) 삭제할까요?"))
                {
                    return;
                }

                var overlay = LoadingOverlay.Create(bodyPanel, "삭제 중...");
                _adminEduInfoController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _adminEduInfoController.DeleteEduInfo(target.eduId);
                    if (res?.Status == 200)
                    {
                        _all.Remove(target);
                        ApplyFilter();
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                    {
                        MessageBox.Show(this.FindForm(), res?.Message ?? "삭제에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (ApiException ex)
                {
                    MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _adminEduInfoController.OnRetry = null;
                    overlay.Close();
                    overlay.Dispose();
                }
            }
        }

        // ===== 셀 포매팅 =====
        private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || grid.Columns[e.ColumnIndex].Name != "status") return;
            switch (e.Value?.ToString())
            {
                case "진행중":
                    e.CellStyle.ForeColor = ThemeColors.OkText;
                    e.CellStyle.BackColor = ThemeColors.OkBg;
                    e.CellStyle.SelectionForeColor = ThemeColors.TableSelectedText;
                    e.CellStyle.SelectionBackColor = ThemeColors.TableSelected;
                    break;
                case "예정":
                    e.CellStyle.ForeColor = ThemeColors.InfoText;
                    e.CellStyle.BackColor = ThemeColors.InfoBg;
                    e.CellStyle.SelectionForeColor = ThemeColors.TableSelectedText;
                    e.CellStyle.SelectionBackColor = ThemeColors.TableSelected;
                    break;
                case "종료":
                    e.CellStyle.ForeColor = ThemeColors.TextMuted;
                    e.CellStyle.SelectionForeColor = ThemeColors.TextMuted;
                    break;
            }
            e.FormattingApplied = true;
        }

        // ================= 필터 =================
        private void ApplyFilter()
        {
            var today = DateTime.Today;
            var result = _all.AsEnumerable();

            switch (cmbStatus.SelectedValue?.ToString())
            {
                case "ACTIVE":
                    result = result.Where(e =>
                        DateTime.TryParseExact(e.startDate, "yyMMdd", null, DateTimeStyles.None, out var s) &&
                        DateTime.TryParseExact(e.endDate, "yyMMdd", null, DateTimeStyles.None, out var en) &&
                        s <= today && en >= today);
                    break;
                case "UPCOMING":
                    result = result.Where(e =>
                        DateTime.TryParseExact(e.startDate, "yyMMdd", null, DateTimeStyles.None, out var s) &&
                        s > today);
                    break;
                case "ENDED":
                    result = result.Where(e =>
                        DateTime.TryParseExact(e.endDate, "yyMMdd", null, DateTimeStyles.None, out var en) &&
                        en < today);
                    break;
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
                result = result.Where(e => e.eduName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

            _filtered = result.ToList();
            ApplySort();
        }

        private void ApplySort()
        {
            if (string.IsNullOrEmpty(_sortColumn))
            {
                _filtered = _filtered
                    .OrderBy(e => StatusOrder(StatusLabel(e)))
                    .ThenByDescending(e => e.startDate)
                    .ToList();
                return;
            }
            Func<EduInfoDto, object?> key = _sortColumn switch
            {
                "eduName" => e => e.eduName,
                "startDate" => e => e.startDate,
                "endDate" => e => e.endDate,
                "batchNumber" => e => e.batchNumber,
                "capacity" => e => e.capacity,
                "status" => e => StatusLabel(e),
                _ => e => null
            };
            _filtered = _sortAscending ? _filtered.OrderBy(key).ToList() : _filtered.OrderByDescending(key).ToList();
        }

        private static int StatusOrder(string status) => status switch
        {
            "진행중" => 0,
            "예정" => 1,
            "종료" => 2,
            _ => 3
        };

        public void SetFilter(string filter)
        {
            cmbStatus.SelectedValue = filter;
        }

        private void SetupFilterSource()
        {
            cmbStatus.DataSource = new[]
            {
                new {Value = "", Label = "전체" },
                new {Value = "ACTIVE", Label = "진행중" },
                new {Value = "UPCOMING", Label = "예정" },
                new {Value = "ENDED", Label = "종료" }
            }.ToList();

            cmbStatus.DisplayMember = "Label";
            cmbStatus.ValueMember = "Value";
            cmbStatus.SelectedIndex = 0;
        }

        // ===== 헬퍼 =====
        private static string StatusLabel(EduInfoDto edu)
        {
            var today = DateTime.Today;
            if(!DateTime.TryParseExact(edu.startDate, "yyMMdd", null, DateTimeStyles.None, out var start) 
                      || !DateTime.TryParseExact(edu.endDate, "yyMMdd", null, DateTimeStyles.None, out var end))
            {
                return "-";
            }

            if(end < today)
            {
                return "종료";
            }
            if(start > today)
            {
                return "예정";
            }
            return "진행중";
        }

        // ===== ISearchFocusable =====
        public void FocusSearch() => txtSearch.Focus();

        // ===== 키보드 이벤트 =====
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(ActiveControl is TextBox or ComboBox)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            switch(keyData)
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
