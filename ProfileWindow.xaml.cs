using System.Windows;
using NimbusChat.WetterChatApp.ViewModels;

namespace NimbusChat
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow(int userId)
        {
            InitializeComponent();
            DataContext = new ProfileViewModel(userId);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm && vm.SaveProfileCommand.CanExecute(null))
            {
                vm.SaveProfileCommand.Execute(null);

                if (string.IsNullOrWhiteSpace(vm.ErrorMessage))
                {
                    MessageBox.Show("Profile saved!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}