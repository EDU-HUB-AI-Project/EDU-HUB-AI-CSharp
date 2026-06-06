using System.ComponentModel;
using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Layout
{
    /// <summary>
    /// 테이블 상단 액션 버튼 영역 — 자식 컨트롤을 항상 우측 정렬.
    /// 도구상자에서 끌어다 놓거나, 디자이너에서 이 패널 안에 AppButton을 드래그해 추가한다.
    /// </summary>
    [ToolboxItem(true)]
    public class ActionBar : FlowLayoutPanel
    {
        public ActionBar()
        {
            Dock = DockStyle.Top;
            Height = 48;
            FlowDirection = FlowDirection.RightToLeft;
            WrapContents = false;
            AutoSize = false;
            AutoScroll = false;
            Padding = new Padding(0, 4, 0, 8);
            Margin = new Padding(0);
            BackColor = ThemeColors.Background;
        }
    }
}
