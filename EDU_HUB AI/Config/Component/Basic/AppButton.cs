using System.ComponentModel;
using EDU_HUB_AI.Config.Component.Common;

namespace EDU_HUB_AI.Config.Component.Basic
{
    /// <summary>
    /// 디자이너 도구상자에서 끌어다 쓰는 공통 버튼.
    /// Variant(색상 종류), Small(작은 크기), IconName(lucide 아이콘)을 속성 그리드에서 지정.
    /// </summary>
    [ToolboxItem(true)]
    public class AppButton : Button
    {
        private ButtonVariant _variant = ButtonVariant.Primary;
        private bool _small;
        private string _iconName = "";

        public AppButton()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ApplyStyle();
        }

        [Category("EDU-HUB")]
        [DefaultValue(ButtonVariant.Primary)]
        [Description("버튼 색상 종류 (Primary / Secondary / Ghost / Danger)")]
        public ButtonVariant Variant
        {
            get => _variant;
            set { _variant = value; ApplyStyle(); }
        }

        [Category("EDU-HUB")]
        [DefaultValue(false)]
        [Description("작은 크기 버튼")]
        public bool Small
        {
            get => _small;
            set { _small = value; ApplyStyle(); }
        }

        [Category("EDU-HUB")]
        [DefaultValue("")]
        [Description("lucide 아이콘 이름 (예: plus, save, search). 비우면 아이콘 없음")]
        public string IconName
        {
            get => _iconName;
            set { _iconName = value ?? ""; ApplyIcon(); }
        }

        private void ApplyStyle()
        {
            ButtonStyles.Apply(this, _variant, _small);
            ApplyIcon();
        }

        private void ApplyIcon()
        {
            if (string.IsNullOrWhiteSpace(_iconName))
            {
                Image = null;
                return;
            }

            Image = IconHelper.Get(_iconName, _small ? 14 : 16, ForeColor);
            TextImageRelation = TextImageRelation.ImageBeforeText;
            ImageAlign = ContentAlignment.MiddleLeft;
        }
    }
}
