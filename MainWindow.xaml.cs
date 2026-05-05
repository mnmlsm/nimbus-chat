using NimbusChat.WetterChatApp.ViewModels;
using NimbusChat.WetterChatApp.Views;
using System.Windows;
using System.Windows.Controls;

namespace NimbusChat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Wird vom PasswordBox.PasswordChanged im XAML aufgerufen
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerView = new RegisterView();
            registerView.Show();
        }
    }
}