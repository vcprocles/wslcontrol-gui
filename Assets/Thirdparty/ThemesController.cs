using System;
using System.Windows;

namespace REghZyFramework.Themes
{
    public static class ThemesController
    {
        public enum ThemeTypes
        {
            Light,
            Dark
        }

        public static ThemeTypes CurrentTheme { get; set; }

        private static ResourceDictionary ThemeDictionary
        {
            get { return Application.Current.Resources.MergedDictionaries[0]; }
            set { Application.Current.Resources.MergedDictionaries[0] = value; }
        }

        private static void ChangeTheme(Uri uri)
        {
            ThemeDictionary = new ResourceDictionary() { Source = uri };
        }
        public static void SetTheme(ThemeTypes theme)
        {
            string themeName="";//the original dev was assigning null, recipe for disaster and new C# versions malding
            CurrentTheme = theme;
            switch (theme)
            {
                case ThemeTypes.Dark: themeName = "DarkTheme"; break;
                case ThemeTypes.Light: themeName = "LightTheme"; break;
            }
            if (!string.IsNullOrEmpty(themeName))
                    ChangeTheme(new Uri($"Assets/Thirdparty/{themeName}.xaml", UriKind.Relative)); 
            //the original dev was doing nothing in try-catch???
        }
    }
}
