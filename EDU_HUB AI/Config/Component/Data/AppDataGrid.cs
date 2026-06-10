using System.Reflection;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Data
{
    public enum TableAction
    {
        Edit,
        Delete,
        Custom
    }

    public class TableActionEventArgs : EventArgs
    {
        public TableActionEventArgs(int rowIndex, TableAction action, string? customKey = null)
        {
            RowIndex = rowIndex;
            Action = action;
            CustomKey = customKey;
        }

        public int RowIndex { get; }
        public TableAction Action { get; }
        public string? CustomKey { get; }
    }

    public class AppDataGrid : DataGridView
    {
        public const string EditColumnName = "__eh_edit";
        public const string DeleteColumnName = "__eh_delete";
        public const string CustomLinkPrefix = "__eh_link_";

        public AppDataGrid()
        {
            EnableDoubleBuffering();
            ApplyDefaultStyle();
            CellContentClick += OnCellContentClick;
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
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            RowHeadersVisible = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight = compact ? 32 : 38;
            RowTemplate.Height = compact ? 32 : 56;
            Font = ThemeFonts.TableCell;

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
                SelectionBackColor = ThemeColors.TableHover,
                SelectionForeColor = ThemeColors.Text,
                Padding = new Padding(12, 0, 8, 0)
            };

            AlternatingRowsDefaultCellStyle = striped
                ? new DataGridViewCellStyle { BackColor = ThemeColors.TableStripe }
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

        public event EventHandler<TableActionEventArgs>? ActionClicked;

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

            ActionClicked?.Invoke(this, new TableActionEventArgs(e.RowIndex, action.Value, customKey));
        }
    }
}