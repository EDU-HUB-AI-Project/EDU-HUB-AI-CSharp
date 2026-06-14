using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Layout
{
    public partial class TabStrip : UserControl
    {
        private readonly Dictionary<Button, string> _buttonsKeys = new();
        private readonly Dictionary<string, Panel> _panels = new();
        private string? _activeKey;
        private bool _runtimeTabs;

        public TabStrip()
        {
            InitializeComponent();
            ApplyTheme();
            if (DesignTimeHelper.IsDesignMode(this))
                SelectTabVisual(btnTabMeal);
        }

        public void AddTab(string key, string title, Control content)
        {
            if (!_runtimeTabs)
            {
                flowTabBar.Controls.Clear();
                panelContent.Controls.Clear();
                _panels.Clear();
                _runtimeTabs = true;
            }

            var wrapper = new Panel { Dock = DockStyle.Fill, Visible = false };
            content.Dock = DockStyle.Fill;
            wrapper.Controls.Add(content);
            _panels[key] = wrapper;
            panelContent.Controls.Add(wrapper);

            var tabBtn = ButtonStyles.Create(title, ButtonVariant.Ghost, small: false);
            tabBtn.AutoSize = true;
            tabBtn.MinimumSize = new Size(100, 32);
            tabBtn.Margin = new Padding(0, 0, 8, 0);
            _buttonsKeys[tabBtn] = key;
            tabBtn.Click += (_, _) => SelectTab(key);
            flowTabBar.Controls.Add(tabBtn);

            if (_activeKey == null)
                SelectTab(key);
        }

        public void SelectTab(string key)
        {
            if (!_panels.ContainsKey(key)) return;
            _activeKey = key;

            foreach (Control c in flowTabBar.Controls)
            {
                if (c is not Button btn) continue;
                if (!_buttonsKeys.TryGetValue(btn, out var tabKey))
                {
                    continue;
                }
                ButtonStyles.Apply(btn, tabKey == key ? ButtonVariant.Primary : ButtonVariant.Ghost, small: false);
            }

            foreach (var (k, panel) in _panels)
                panel.Visible = k == key;

            TabChanged?.Invoke(this, key);
        }

        public event EventHandler<string>? TabChanged;

        private void ApplyTheme()
        {
            panelContent.BackColor = ThemeColors.Surface;
            ButtonStyles.Apply(btnTabMeal, ButtonVariant.Primary, small: true);
            ButtonStyles.Apply(btnTabFacility, ButtonVariant.Ghost, small: true);
            ButtonStyles.Apply(btnTabTraffic, ButtonVariant.Ghost, small: true);
        }

        private void TabDesignButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
                SelectTabVisual(btn);
        }

        private void SelectTabVisual(Button active)
        {
            foreach (Control c in flowTabBar.Controls)
            {
                if (c is Button btn)
                    ButtonStyles.Apply(btn, btn == active ? ButtonVariant.Primary : ButtonVariant.Ghost, small: true);
            }
            lblTabPreview.Text = active.Text switch
            {
                "구내식당" => "구내식당 · 식단표 탭 내용",
                "시설 위치" => "시설 위치 탭 내용",
                _ => "교통 정보 탭 내용"
            };
        }
    }
}
