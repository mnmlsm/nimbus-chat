using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;
namespace NimbusChat.WetterChatApp.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.RegisterViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }
}