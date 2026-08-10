using System.Windows;
using NimbusChat.WetterChatApp.ViewModels;

namespace NimbusChat
{
    // Profile editing dialog: lets the user update their username, email,
    // and favorite city, then reports success back to the dashboard.
    public partial class ProfileWindow : Window
    {
        public ProfileWindow(int userId)
        {
            InitializeComponent();
            DataContext = new ProfileViewModel(userId);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm)
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