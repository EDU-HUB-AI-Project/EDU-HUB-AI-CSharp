using System.Reflection;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Data
{


    public enum TableAction
    {
        Edit,
        Delete,
        Custom,
        New
    }

    public enum PageNavigation
    {
        Next,
        Prev,
        First,
        Last
    }

    public class TableActionEventArgs : EventArgs
    {
        public TableActionEventArgs(int rowIndex, TableAction action, string? customKey = null, object? tag = null)
        {
            RowIndex = rowIndex;
            Action = action;
            CustomKey = customKey;
            Tag = tag;
        }

        public int RowIndex { get; }
        public TableAction Action { get; }
        public string? CustomKey { get; }
        public object? Tag { get; }
    }

    public class AppDataGrid : DataGridView
    {
        public const string EditColumnName = "__eh_edit";
        public const string DeleteColumnName = "__eh_delete";
        public const string CustomLinkPrefix = "__eh_link_";

        public string EmptyMessage { get; set; } = "해당 데이터가 존재하지 않습니다.";

        public event EventHandler<TableActionEventArgs>? ActionClicked;
        public event EventHandler<PageNavigation>? PageNavigationRequested;
        public event EventHandler<(string Column, bool Ascending)>? SortChanged;

        private string? _sortColumn;
        private bool _sortAscending = true;

        // 공통 날짜 형식
        private static readonly string[] DateFormats =
        {
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm:ss",
            "yyyyMMdd",
            "yyMMdd",
            "yy-MM-dd",
            "yyyy. MM. dd.",
            "yyyy/MM/dd"
        };


        public AppDataGrid()
        {
            EnableDoubleBuffering();
            ApplyDefaultStyle();
            CellContentClick += OnCellContentClick;
            KeyDown += OnGridKeyDown;
        }

        private void EnableDoubleBuffering()
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, this, new object[] { true });
        }

        public void ApplyDefaultStyle(bool compact = false, bool striped = true)
        {
            BorderStyle = BorderStyle.None;
            BackgroundColor = ThemeColors.Surface;
            GridColor = ThemeColors.TableBorder;
            EnableHeadersVisualStyles = false;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ReadOnly = true;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            ShowCellToolTips = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            RowHeadersVisible = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight = compact ? 32 : 38;
            RowTemplate.Height = compact ? 32 : 56;
            Font = ThemeFonts.TableCell;
            StandardTab = true;

            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ThemeColors.TableHeader,
                ForeColor = ThemeColors.Text,
                Font = ThemeFonts.TableHeader,
                SelectionBackColor = ThemeColors.TableHeader,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(12, 0, 8, 0)
            };

            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ThemeColors.Surface,
                ForeColor = ThemeColors.Text,
                SelectionBackColor = ThemeColors.InfoBg,
                SelectionForeColor = ThemeColors.InfoText,
                Padding = new Padding(12, 0, 8, 0)
            };

            AlternatingRowsDefaultCellStyle = striped
                ? new DataGridViewCellStyle
                {
                    BackColor = ThemeColors.TableStripe,
                    SelectionBackColor = ThemeColors.InfoBg,
                    SelectionForeColor = ThemeColors.InfoText
                }
                : DefaultCellStyle;
        }

        public void AddTextActionColumns(bool includeEdit = true, bool includeDelete = true)
        {
            if (includeEdit)
            {
                Columns.Add(new DataGridViewLinkColumn
                {
                    Name = EditColumnName,
                    HeaderText = "관리",
                    Text = "수정",
                    UseColumnTextForLinkValue = true,
                    LinkColor = ThemeColors.Link,
                    ActiveLinkColor = ThemeColors.PrimaryHover,
                    VisitedLinkColor = ThemeColors.Link,
                    TrackVisitedState = false,
                    Width = 90,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                });
            }

            if (includeDelete)
            {
                Columns.Add(new DataGridViewLinkColumn
                {
                    Name = DeleteColumnName,
                    HeaderText = includeEdit ? "" : "관리",
                    Text = "삭제",
                    UseColumnTextForLinkValue = true,
                    LinkColor = ThemeColors.LinkDanger,
                    ActiveLinkColor = ThemeColors.Danger,
                    VisitedLinkColor = ThemeColors.LinkDanger,
                    TrackVisitedState = false,
                    Width = 90,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                });
            }
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);
            var name = e.Column.Name;
            e.Column.SortMode = (name == EditColumnName || name == DeleteColumnName || name.StartsWith(CustomLinkPrefix))
                              ? DataGridViewColumnSortMode.NotSortable
                              : DataGridViewColumnSortMode.Programmatic;
        }

        public void AddCustomLinkColumn(string key, string headerText, string linkText)
        {
            Columns.Add(new DataGridViewLinkColumn
            {
                Name = CustomLinkPrefix + key,
                HeaderText = headerText,
                Text = linkText,
                UseColumnTextForLinkValue = true,
                LinkColor = ThemeColors.Link,
                ActiveLinkColor = ThemeColors.PrimaryHover,
                VisitedLinkColor = ThemeColors.Link,
                TrackVisitedState = false,
                Width = 72,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            });
        }

        // 데이터 형태에 따른 정렬
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Value != null)
            {
                Type valueType = e.Value.GetType();
                string valueString = e.Value.ToString()!.Trim();

                if (valueType == typeof(int) || valueType == typeof(double) || valueType == typeof(decimal))
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (valueType == typeof(DateTime))
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    e.Value = ((DateTime)e.Value).ToString("yyyy.MM.dd");
                }
                else if ( DateTime.TryParseExact(valueString, DateFormats,
                             System.Globalization.CultureInfo.InvariantCulture,
                             System.Globalization.DateTimeStyles.None, out DateTime parseDate))
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    e.Value = parseDate.ToString("yyyy.MM.dd");
                }
                else
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
            }
            base.OnCellFormatting(e);
        }
        // 데이터가 존재하지 않을 경우 메시지 표시
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Rows.Count > 0)
            {
                return;
            }

            var emptyRect = new Rectangle(
                0,
                ColumnHeadersHeight,
                Width,
                Height - ColumnHeadersHeight);

            if (emptyRect.Height <= 0)
            {
                return;
            }

            using var backBrush = new SolidBrush(ThemeColors.Surface);
            e.Graphics.FillRectangle(backBrush, emptyRect);

            TextRenderer.DrawText(
                e.Graphics,
                EmptyMessage,
                ThemeFonts.PageTitle,
                emptyRect,
                ThemeColors.TextMuted, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnColumnHeaderMouseClick(e);
            var col = Columns[e.ColumnIndex];

            if(col.SortMode == DataGridViewColumnSortMode.NotSortable)
            {
                return;
            }
            if(_sortColumn == col.Name)
            {
                _sortAscending = !_sortAscending;
            }
            else
            {
                _sortColumn = col.Name;
                _sortAscending = true;
            }

            foreach(DataGridViewColumn c in Columns)
            {
                c.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
            col.HeaderCell.SortGlyphDirection = _sortAscending ? SortOrder.Ascending : SortOrder.Descending;

            SortChanged?.Invoke(this, (_sortColumn, _sortAscending));
        }

        public void SetInitialSort(string columnName, bool ascending = true)
        {
            _sortColumn = columnName;
            _sortAscending = ascending;
            if(Columns.Contains(columnName))
            {
                Columns[columnName].HeaderCell.SortGlyphDirection = ascending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if(e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                e.Paint(e.ClipBounds, e.PaintParts & ~DataGridViewPaintParts.Focus);
                e.Handled = true;
                return;
            }
            // ========== 관리 칼럼헤더 병합 ========
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                var col = Columns[e.ColumnIndex];

                // 수정 칼럼 헤더 → 삭제 칼럼 헤더까지 합쳐서 그림
                if (col.Name == EditColumnName && Columns.Contains(DeleteColumnName))
                {
                    var deleteCol = Columns[DeleteColumnName];
                    if (deleteCol.Visible)
                    {
                        var mergedRect = new Rectangle(
                            e.CellBounds.Left,
                            e.CellBounds.Top,
                            e.CellBounds.Width + deleteCol.Width,
                            e.CellBounds.Height);

                        using (var backBrush = new SolidBrush(ThemeColors.TableHeader))
                            e.Graphics.FillRectangle(backBrush, mergedRect);

                        TextRenderer.DrawText(
                            e.Graphics,
                            "관리",
                            ThemeFonts.TableHeader,
                            mergedRect,
                            ThemeColors.Text,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                        using (var borderPen = new Pen(ThemeColors.TableBorder))
                            e.Graphics.DrawRectangle(borderPen,
                                mergedRect.Left, mergedRect.Top,
                                mergedRect.Width - 1, mergedRect.Height - 1);

                        e.Handled = true;
                        return;
                    }
                }
                // 삭제 칼럼 헤더는 위에서 이미 그렸으므로 아무것도 안 그림
                else if (col.Name == DeleteColumnName && Columns.Contains(EditColumnName))
                {
                    e.Handled = true;
                    return;
                }
            }
            base.OnCellPainting(e);
        }

        // ── 키보드 핸들러 ─────────────────────────────────
        private void OnGridKeyDown(object? sender, KeyEventArgs e)
        {
            if(e.Control)
            {
                switch(e.KeyCode)
                {
                    case Keys.N:
                        ActionClicked?.Invoke(this, new TableActionEventArgs(-1, TableAction.New));
                        e.Handled = true; e.SuppressKeyPress = true;
                        return;
                    case Keys.C when CurrentRow != null:
                        CopyRowToClipboard();
                        e.Handled = true; e.SuppressKeyPress = true;
                        return;
                }
            }

            var row = CurrentRow?.Index ?? -1;
            if(row < 0)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Enter when !e.Control && !e.Shift && Columns.Contains(EditColumnName):
                    ActionClicked?.Invoke(this, new TableActionEventArgs(row, TableAction.Edit, tag: Rows[row].Tag));
                    e.Handled = true; e.SuppressKeyPress = true;
                    break;
                case Keys.F2 when Columns.Contains(EditColumnName):
                    ActionClicked?.Invoke(this, new TableActionEventArgs(row, TableAction.Edit, tag: Rows[row].Tag));
                    e.Handled = true;
                    break;
                case Keys.Delete when Columns.Contains(DeleteColumnName):
                    ActionClicked?.Invoke(this, new TableActionEventArgs(row, TableAction.Delete, tag: Rows[row].Tag));
                    e.Handled = true;
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.Right:
                    PageNavigationRequested?.Invoke(this, PageNavigation.Next);
                    return true;
                case Keys.Control | Keys.Left:
                    PageNavigationRequested?.Invoke(this, PageNavigation.Prev);
                    return true;
                case Keys.Control | Keys.Home:
                    PageNavigationRequested?.Invoke(this, PageNavigation.First);
                    return true;
                case Keys.Control | Keys.End:
                    PageNavigationRequested?.Invoke(this, PageNavigation.Last);
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CopyRowToClipboard()
        {
            if(CurrentRow == null)
            {
                return;
            }

            var values = new List<string>();
            foreach(DataGridViewCell cell in CurrentRow.Cells)
            {
                var name = Columns[cell.ColumnIndex].Name;
                if(name == EditColumnName || name == DeleteColumnName || name.StartsWith(CustomLinkPrefix))
                {
                    continue;
                }
                values.Add(cell.FormattedValue?.ToString() ?? "");
            }

            if(values.Count > 0)
            {
                Clipboard.SetText(string.Join("\t", values));
            }
        }

        private void OnCellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var col = Columns[e.ColumnIndex];
            if (col is not DataGridViewLinkColumn) return;

            TableAction? action = col.Name switch
            {
                EditColumnName => TableAction.Edit,
                DeleteColumnName => TableAction.Delete,
                _ when col.Name.StartsWith(CustomLinkPrefix, StringComparison.Ordinal) => TableAction.Custom,
                _ => null
            };

            if (action == null) return;

            var customKey = action == TableAction.Custom
                ? col.Name[CustomLinkPrefix.Length..]
                : null;

            ActionClicked?.Invoke(this, new TableActionEventArgs(e.RowIndex, action.Value, customKey, Rows[e.RowIndex].Tag));
        }
    }
}