using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.View
{
    public partial class CafeteriaView : UserControl
    {
        private List<Dictionary<string, object>> _all = new List<Dictionary<string, object>>();
        private List<Dictionary<string, object>> _pageItems = new List<Dictionary<string, object>>();
        private readonly AdminCafeteriaController _adminCafeteriaController = new AdminCafeteriaController();

        public CafeteriaView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender();
            btnCreate.Click += OnCreate;
            pagination1.PageChanged += (_, page) => RenderPage(page);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            await LoadAndRender();
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(grid, 0);
            bodyPanel.Controls.SetChildIndex(actionPanel, 1);
            bodyPanel.Controls.SetChildIndex(pagination1, 2);
        }

        private async Task<List<Dictionary<string, object>>> LoadData()
        {
            string today = DateTime.Now.ToString("yyyy-MM");
            var res = await _adminCafeteriaController.GetCafeteriaSummary(today);
            return res?.Data ?? new List<Dictionary<string, object>>();
        }

        private async Task LoadAndRender()
        {
            _all = await LoadData();
            RenderPage(1);
        }

        private void SetupGrid()
        {
            grid.Columns.Add("mealDate", "날짜");
            grid.Columns.Add("breakfast", "조식");
            grid.Columns.Add("lunch", "점심");
            grid.Columns.Add("dinner", "석식");
            grid.AddTextActionColumns();
            grid.ActionClicked += OnRowAction;
            grid.CellDoubleClick += OnCellDoubleClick;
        }

        private void RenderPage(int page)
        {
            pagination1.TotalCount = _all.Count;
            var size = pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_all.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination1.PageIndex = page;

            _pageItems = new List<Dictionary<string, object>>();
            int startIndex = (page - 1) * size;
            int endIndex = Math.Min(startIndex + size, _all.Count);
            for (int i = startIndex; i < endIndex; i++)
            {
                _pageItems.Add(_all[i]);
            }

            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach (var s in _pageItems)
            {
                string mealDate = "";
                string breakfast = "X";
                string lunch = "X";
                string dinner = "X";

                if (s.ContainsKey("mealDate")) mealDate = s["mealDate"].ToString();
                if (s.ContainsKey("BREAKFAST")) breakfast = s["BREAKFAST"].ToString();
                if (s.ContainsKey("LUNCH")) lunch = s["LUNCH"].ToString();
                if (s.ContainsKey("DINNER")) dinner = s["DINNER"].ToString();

                if (breakfast == "0") breakfast = "O";
                if (lunch == "0") lunch = "O";
                if (dinner == "0") dinner = "O";

                grid.Rows.Add(mealDate, breakfast, lunch, dinner);
            }
            grid.ResumeLayout();
        }

        private async void OnCreate(object? sender, EventArgs e)
        {
            bool result = CafeteriaEditModal.Show(this.FindForm());
            if (result) await LoadAndRender();
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                string mealDate = target.ContainsKey("mealDate") ? target["mealDate"].ToString() : "";
                bool result = CafeteriaEditModal.Show(this.FindForm(), mealDate);
                if (result) await LoadAndRender();
            }
            else if (e.Action == TableAction.Delete)
            {
                string mealDate = target.ContainsKey("mealDate") ? target["mealDate"].ToString() : "";
                if (!ConfirmModal.Show(this.FindForm(), "삭제 확인", $"'{mealDate}' 식단을 삭제할까요?"))
                    return;
                await DeleteByDate(mealDate);
                await LoadAndRender();
            }
        }

        private async void OnCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];
            string mealDate = target.ContainsKey("mealDate") ? target["mealDate"].ToString() : "";
            bool result = CafeteriaEditModal.Show(this.FindForm(), mealDate);
            if (result) await LoadAndRender();
        }

        private async Task DeleteByDate(string mealDate)
        {
            var res = await _adminCafeteriaController.GetCafeteriaDetail(mealDate);
            var list = res?.Data ?? new List<CafeteriaDto>();

            foreach (var item in list)
            {
                if (item.cafeteriaId == null) continue;

                await _adminCafeteriaController.DeleteCafeteria(item.cafeteriaId);
            }
        }
    }
}