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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}