using MaterialDesignThemes.Wpf;
using System.Windows;

namespace NimbusChat
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            ThemeComboBox.SelectedIndex =
                ThemeManager.IsDarkTheme ? 1 : 0;

            LanguageComboBox.SelectedIndex =
                (int)LanguageManager.CurrentLanguage;

            UpdateLanguage();

            LanguageManager.LanguageChanged += LanguageManager_LanguageChanged;
        }

        private void ThemeComboBox_SelectionChanged(
            object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            if (ThemeComboBox.SelectedIndex == 1)
            {
                ThemeManager.SetDarkTheme();
            }
            else
            {
                ThemeManager.SetLightTheme();
            }
        }

        private void LanguageComboBox_SelectionChanged(
            object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            LanguageManager.SetLanguage(
                (AppLanguage)LanguageComboBox.SelectedIndex);
        }

        private void LanguageManager_LanguageChanged(
            object sender,
            System.EventArgs e)
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            SettingsTitle.Text =
                LanguageManager.Get("Settings");

            SettingsDescription.Text =
                LanguageManager.Get("SettingsDescription");

            AppearanceTitle.Text =
                LanguageManager.Get("Appearance");

            AppearanceDescription.Text =
                LanguageManager.Get("AppearanceDescription");

            LanguageTitle.Text =
                LanguageManager.Get("Language");

            LanguageDescription.Text =
                LanguageManager.Get("LanguageDescription");

            ApplicationTitle.Text =
                LanguageManager.Get("Application");

            ApplicationDescription.Text =
                LanguageManager.Get("WeatherMessenger");

            LightThemeItem.Content =
        LanguageManager.Get("Light");

            DarkThemeItem.Content =
                LanguageManager.Get("Dark");

            EnglishLanguageItem.Content =
                LanguageManager.Get("English");

            GermanLanguageItem.Content =
                LanguageManager.Get("German");

            RussianLanguageItem.Content =
                LanguageManager.Get("Russian");
        }

        private void Close_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(
            System.EventArgs e)
        {
            LanguageManager.LanguageChanged -=
                LanguageManager_LanguageChanged;

            base.OnClosed(e);
        }
    }
}