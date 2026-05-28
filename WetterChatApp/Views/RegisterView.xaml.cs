using System.Windows;
using NimbusChat.WetterChatApp.ViewModels;

namespace NimbusChat.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();
        }
    }
}