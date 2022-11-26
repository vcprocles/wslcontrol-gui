using System.Runtime.InteropServices;
namespace wslcontrol_gui
{
    static class ThemeResolver
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        private static extern bool ShouldSystemUseDarkMode();
        public static void SetTheme()
        {
            bool bRet = ShouldSystemUseDarkMode();
            if (bRet) REghZyFramework.Themes.ThemesController.SetTheme(REghZyFramework.Themes.ThemesController.ThemeTypes.Dark);
            else REghZyFramework.Themes.ThemesController.SetTheme(REghZyFramework.Themes.ThemesController.ThemeTypes.Light);
        }
    }
}