using System.Runtime.InteropServices;
namespace wslcontrol_gui
{
    class ThemeResolver
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();
        public static void SetTheme()
        {
            bool bRet = ShouldSystemUseDarkMode();
            if (bRet) REghZyFramework.Themes.ThemesController.SetTheme(REghZyFramework.Themes.ThemesController.ThemeTypes.Dark);
            else REghZyFramework.Themes.ThemesController.SetTheme(REghZyFramework.Themes.ThemesController.ThemeTypes.Light);
        }
    }
}