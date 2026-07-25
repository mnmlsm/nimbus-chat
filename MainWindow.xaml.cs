using NimbusChat.Views;
using NimbusChat.WetterChatApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NimbusChat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void OpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var register = new RegisterView();
            register.Show();
            Close();
        }

        private bool _showPassword = false;

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordTextBox.Text;
            }
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _showPassword = !_showPassword;

            if (_showPassword)
            {
                PasswordTextBox.Text = PasswordBox.Password;

                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;

                EyeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOffOutline;
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;

                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;

                EyeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOutline;
            }
        }
    }
}