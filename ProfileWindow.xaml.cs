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

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm && vm.SaveProfileCommand.CanExecute(null))
            {
                await vm.SaveProfileAsync();

                if (string.IsNullOrWhiteSpace(vm.ErrorMessage))
                {
                    AppMessageBox.Show("Profile saved!", "Success", AppMessageBoxIcon.Success, this);
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}