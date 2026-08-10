using System;
using System.Linq;
using System.Windows;

namespace NimbusChat
{
    public static class ThemeManager
    {
        public static bool IsDarkTheme
        {
            get
            {
                var currentTheme = Application.Current.Resources.MergedDictionaries
                    .FirstOrDefault(d =>
                        d.Source != null &&
                        d.Source.OriginalString.Contains("Theme.xaml"));

                return currentTheme?.Source?.OriginalString.Contains("DarkTheme") == true;
            }
        }

        public static void SetDarkTheme()
        {
            SetTheme("Dark");
        }

        public static void SetLightTheme()
        {
            SetTheme("Light");
        }

        public static void ToggleTheme()
        {
            if (IsDarkTheme)
                SetLightTheme();
            else
                SetDarkTheme();
        }

        private static void SetTheme(string themeName)
        {
            var app = Application.Current;

            var newTheme = new ResourceDictionary
            {
                Source = new Uri(
                    $"Styles/{themeName}Theme.xaml",
                    UriKind.Relative)
            };

            var oldTheme = app.Resources.MergedDictionaries
                .FirstOrDefault(d =>
                    d.Source != null &&
                    d.Source.OriginalString.Contains("Theme.xaml"));

            if (oldTheme != null)
            {
                app.Resources.MergedDictionaries.Remove(oldTheme);
            }

            app.Resources.MergedDictionaries.Add(newTheme);
        }
    }
}