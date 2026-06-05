using System.ComponentModel;

namespace EDU_HUB_AI.Config.Component.Common
{
    public static class DesignTimeHelper
    {
        public static bool IsDesignMode(Control? control)
        {
            if (control?.Site?.DesignMode == true)
                return true;
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }
    }
}
