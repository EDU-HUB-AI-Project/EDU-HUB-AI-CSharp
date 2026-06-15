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
    public partial class FacilityLocationView : UserControl
    {
        private List<FacilityInfoDto> _all = new();
        private List<FacilityInfoDto> _filtered = new();
        private List<FacilityInfoDto> _pageItems = new();
        private readonly AdminFacilityInfoController _controller = new();

        private string _sortColumn = "type";
        private bool _sortAscending = true;

        private readonly KpiSummaryBar _kpiBar = new();
        public FacilityLocationView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            grid.SortChanged += (_, s) =>
            {
                _sortColumn = s.Column;
                _sortAscending = s.Ascending;
                ApplyTypeFilter();
                RenderPage(1);
            };

            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;

            bodyPanel.Controls.Add(_kpiBar);
            _kpiBar.SetCards(
                ("전체 시설", ThemeColors.Primary),
                ("내부 시설", ThemeColors.Ok),
                ("외부 시설", ThemeColors.Warn),
                ("이미지 미등록", ThemeColors.Danger)
            );
            _kpiBar.CardClicked += (_, index) =>
            {
                switch (index)
                {
                    case 0: cmbTypeFilter.SelectedValue = ""; break;
                    case 1: cmbTypeFilter.SelectedValue = "INNER"; break;
                    case 2: cmbTypeFilter.SelectedValue = "OUTER"; break;
                }
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

            SetupTypeFilter();

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender(1);
            btnCreate.Click += OnCreate;
            pagination1.PageChanged += (_, page) => RenderPage(page);
            cmbTypeFilter.SelectedIndexChanged += (_, _) => { ApplyTypeFilter(); RenderPage(1); };

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
            grid.SetInitialSort(_sortColumn, _sortAscending);
            await LoadAndRender(1);
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

        private async Task<List<FacilityInfoDto>> LoadData()
        {
                var res = await _controller.GetFacilityList();
                return res?.Data ?? new List<FacilityInfoDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩 중...") : null;
            _controller.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                _all = await LoadData();
                UpdateKpi();
                ApplyTypeFilter();
                RenderPage(page);
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
                _all.Count.ToString(),
                _all.Count(f => f.facilityType == "INNER").ToString(),
                _all.Count(f => f.facilityType == "OUTER").ToString(),
                _all.Count(f => string.IsNullOrEmpty(f.imagePath) && f.facilityType != "OUTER").ToString()  // ← OUTER 제외
            );
        }

        private void SetupGrid()
        {
            grid.RowTemplate.Height = 56;
            grid.Columns.Add("type", "구분");
            grid.Columns.Add("name", "시설명");
            grid.Columns.Add("location", "위치");
            grid.Columns.Add("detail", "층수 / 좌표");
            grid.Columns.Add("image", "이미지");
            grid.Columns["image"].MinimumWidth = 160;
            grid.Columns["image"].FillWeight = 140;
            grid.Columns.Add("description", "안내");
            grid.AddTextActionColumns();
            grid.CellPainting += OnImageCellPainting;
            grid.ActionClicked += OnRowAction;
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
            ClearGridImages();
            grid.Rows.Clear();
            foreach (var f in _pageItems)
            {
                var rowIndex = grid.Rows.Add(
                    TypeLabel(f.facilityType),
                    f.name,
                    f.location,
                    DetailLabel(f),
                    ImageCellData(f),
                    f.description ?? "");

                grid.Rows[rowIndex].Tag = f;

                var imageCell = grid.Rows[rowIndex].Cells["image"];
                if (imageCell.Value is FacilityImageCell imgCell && !string.IsNullOrEmpty(imgCell.FullPath))
                    imageCell.ToolTipText = imgCell.FullPath;
            }
            grid.ResumeLayout();
        }

        private async void OnCreate(object? sender, EventArgs e)
        {
            var created = FacilityEditModal.Show(FindForm(), null);
            if (created == null) return;

            var overlay = LoadingOverlay.Create(bodyPanel, "등록 중...");
            _controller.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                var res = await _controller.InsertFacility(created);
                if (res?.Status == 200)
                    await LoadAndRender(int.MaxValue, showOverlay: false);
                else
                    MessageBox.Show(FindForm(), res?.Message ?? "등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApiException ex) { MessageBox.Show(FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception ex) { MessageBox.Show(FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { _controller.OnRetry = null; overlay.Close(); overlay.Dispose(); }
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            var target = e.Tag as FacilityInfoDto;
            if(target == null)
            {
                return;
            }

            if (e.Action == TableAction.Edit)
            {
                var edited = FacilityEditModal.Show(FindForm(), target);
                if (edited == null) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _controller.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _controller.UpdateFacility(target.facilityId, edited);
                    if (res?.Status == 200)
                    {
                        var idx = _all.IndexOf(target);
                        if (idx >= 0)
                        {
                            _all[idx] = edited;
                        }
                        ApplyTypeFilter();
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                        MessageBox.Show(FindForm(), res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ApiException ex) { MessageBox.Show(FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                catch (Exception ex) { MessageBox.Show(FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { _controller.OnRetry = null; overlay.Close(); overlay.Dispose(); }
            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(FindForm(), "삭제 확인", $"'{target.name}'을(를) 삭제할까요?"))
                    return;

                var overlay = LoadingOverlay.Create(bodyPanel, "삭제 중...");
                _controller.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _controller.DeleteFacility(target.facilityId);
                    if (res?.Status == 200)
                    {
                        _all.Remove(target);
                        ApplyTypeFilter();
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                        MessageBox.Show(FindForm(), res?.Message ?? "삭제에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ApiException ex) { MessageBox.Show(FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                catch (Exception ex) { MessageBox.Show(FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { _controller.OnRetry = null; overlay.Close(); overlay.Dispose(); }
            }
        }

        private void SetupTypeFilter()
        {
            cmbTypeFilter.DataSource = new[]
            {
                new { Value = "", Label = "전체" },
                new { Value = "INNER", Label = "내부" },
                new { Value = "OUTER", Label = "외부" }
            };
            cmbTypeFilter.DisplayMember = "Label";
            cmbTypeFilter.ValueMember = "Value";
            cmbTypeFilter.SelectedIndex = 0;
        }

        private void ApplyTypeFilter()
        {
            var type = cmbTypeFilter.SelectedValue?.ToString();
            _filtered = string.IsNullOrEmpty(type)
                ? _all.ToList()
                : _all.Where(f => string.Equals(f.facilityType, type, StringComparison.OrdinalIgnoreCase)).ToList();
            ApplySort();
        }

        private void ApplySort()
        {
            if (string.IsNullOrEmpty(_sortColumn)) return;
            Func<FacilityInfoDto, object?> key = _sortColumn switch
            {
                "type" => f => f.facilityType,
                "name" => f => f.name,
                "location" => f => f.location,
                "detail" => f => DetailLabel(f),
                _ => f => null
            };
            _filtered = _sortAscending ? _filtered.OrderBy(key).ToList() : _filtered.OrderByDescending(key).ToList();
        }

        private static string DetailLabel(FacilityInfoDto f)
        {
            if (string.Equals(f.facilityType, "INNER", StringComparison.OrdinalIgnoreCase))
                return f.floor.HasValue ? $"{f.floor}층" : "-";

            if (string.Equals(f.facilityType, "OUTER", StringComparison.OrdinalIgnoreCase))
                return f.mapX.HasValue && f.mapY.HasValue ? $"{f.mapX:F6}, {f.mapY:F6}" : "-";

            return "-";
        }

        private static string TypeLabel(string? type) =>
            string.Equals(type, "INNER", StringComparison.OrdinalIgnoreCase) ? "내부" :
            string.Equals(type, "OUTER", StringComparison.OrdinalIgnoreCase) ? "외부" : type ?? "";

        private static FacilityImageCell? ImageCellData(FacilityInfoDto f)
        {
            if (!string.Equals(f.facilityType, "INNER", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(f.imagePath))
                return null;

            return new FacilityImageCell(
                FacilityImageStore.TryLoadThumbnail(f.imagePath, 40),
                f.imagePath);
        }

        // 이미지 셀 — 썸네일 + 경로 말줄임
        private void OnImageCellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (grid.Columns[e.ColumnIndex].Name != "image") return;

            var bounds = e.CellBounds;
            var selected = (e.State & DataGridViewElementStates.Selected) != 0;
            var style = e.CellStyle ?? grid.DefaultCellStyle;

            var backColor = selected
                ? style.SelectionBackColor
                : e.RowIndex % 2 == 1
                    ? grid.AlternatingRowsDefaultCellStyle.BackColor
                    : style.BackColor;

            using (var brush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

            var data = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as FacilityImageCell;
            const int pad = 8;
            const int thumb = 40;
            var thumbRect = new Rectangle(
                bounds.X + pad,
                bounds.Y + (bounds.Height - thumb) / 2,
                thumb,
                thumb);

            if (data?.Thumbnail != null)
            {
                e.Graphics.DrawImage(data.Thumbnail, thumbRect);
                using var imgPen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(imgPen, thumbRect.X, thumbRect.Y, thumbRect.Width - 1, thumbRect.Height - 1);
            }

            var textAreaWidth = bounds.Width - thumb - pad * 3;
            var text = EllipsizePathKeepEnd(data?.FullPath, style.Font, textAreaWidth);
            var textRect = new Rectangle(thumbRect.Right + pad, bounds.Y, textAreaWidth, bounds.Height);
            var foreColor = selected ? style.SelectionForeColor : style.ForeColor;
            TextRenderer.DrawText(
                e.Graphics,
                text,
                style.Font,
                textRect,
                foreColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            using var gridPen = new Pen(grid.GridColor);
            e.Graphics.DrawLine(gridPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            e.Graphics.DrawLine(gridPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);

            e.Handled = true;
        }

        private static string EllipsizePathKeepEnd(string? path, Font font, int maxPixelWidth)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "-";

            var normalized = path.Trim().Replace('\\', '/');
            var fileName = Path.GetFileName(normalized.TrimEnd('/'));
            if (string.IsNullOrEmpty(fileName))
                fileName = normalized;

            var preferred = "..." + fileName;
            if (TextRenderer.MeasureText(preferred, font).Width <= maxPixelWidth)
                return preferred;

            for (var len = fileName.Length; len > 1; len--)
            {
                var candidate = "..." + fileName[^len..];
                if (TextRenderer.MeasureText(candidate, font).Width <= maxPixelWidth)
                    return candidate;
            }

            return "..." + fileName[^Math.Min(4, fileName.Length)..];
        }

        private void ClearGridImages()
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells["image"]?.Value is FacilityImageCell cell)
                    cell.Dispose();
            }
        }

        private sealed class FacilityImageCell : IDisposable
        {
            public Image? Thumbnail { get; }
            public string FullPath { get; }

            public FacilityImageCell(Image? thumbnail, string fullPath)
            {
                Thumbnail = thumbnail;
                FullPath = fullPath;
            }

            public void Dispose() => Thumbnail?.Dispose();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ActiveControl is TextBox or ComboBox)
                return base.ProcessCmdKey(ref msg, keyData);

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
