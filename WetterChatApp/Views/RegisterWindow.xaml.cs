using System.Windows;
using NimbusChat.WetterChatApp.ViewModels;

namespace NimbusChat
{
    // Registration screen: wires the password fields to RegisterViewModel and
    // provides the way back to the login window.
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Show();
            Close();
        }
    }
}