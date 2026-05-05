using System.Windows;
using NimbusChat.WetterChatApp.ViewModels;
using NimbusChat.Views;

namespace NimbusChat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void OpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var register = new RegisterView();
            register.Show();
            this.Close();
        }
    }


}