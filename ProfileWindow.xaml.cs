using NimbusChat.WetterChatApp.ViewModels;
using System;
using System.Windows;

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

            LanguageManager.LanguageChanged +=
                LanguageManager_LanguageChanged;

            UpdateLanguage();
        }

        private void LanguageManager_LanguageChanged(
            object sender,
            EventArgs e)
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            Title =
                LanguageManager.Get("EditProfile");

            EditProfileTitle.Text =
                LanguageManager.Get("EditProfile");

            UpdateAccountDetailsText.Text =
                LanguageManager.Get("UpdateAccountDetails");

            UsernameLabel.Text =
                LanguageManager.Get("Username");

            EmailLabel.Text =
                LanguageManager.Get("Email");

            StatusLabel.Text =
                LanguageManager.Get("Status");

            FavoriteCityLabel.Text =
                LanguageManager.Get("FavoriteCityLabel");

            SaveChangesButton.Content =
                LanguageManager.Get("SaveChanges");

            OnlineStatusItem.Content =
                LanguageManager.Get("Online");

            AwayStatusItem.Content =
                LanguageManager.Get("Away");

            BusyStatusItem.Content =
                LanguageManager.Get("Busy");

            OfflineStatusItem.Content =
                LanguageManager.Get("Offline");
        }

        private async void Save_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm)
            {
                await vm.SaveProfileAsync();

                if (string.IsNullOrWhiteSpace(vm.ErrorMessage))
                {
                    AppMessageBox.Show(
                        LanguageManager.Get("ProfileSaved"),
                        LanguageManager.Get("Success"),
                        AppMessageBoxIcon.Success,
                        this);

                    DialogResult = true;
                    Close();
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            LanguageManager.LanguageChanged -=
                LanguageManager_LanguageChanged;

            base.OnClosed(e);
        }
    }
}