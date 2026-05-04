using System.Windows;
using System.Windows.Controls;
using NimbusChat.WetterChatApp.ViewModels;

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
    }
}