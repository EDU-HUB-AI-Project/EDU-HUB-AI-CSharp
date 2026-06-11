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

        public event EventHandler<TableActionEventArgs>? ActionClicked;
        public event EventHandler<PageNavigation>? PageNavigationRequested;

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
                Alignment = DataGridViewContentAlignment.MiddleLeft,
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

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if(e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                e.Paint(e.ClipBounds, e.PaintParts & ~DataGridViewPaintParts.Focus);
                e.Handled = true;
                return;
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