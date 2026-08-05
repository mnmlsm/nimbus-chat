using System.Threading.Tasks;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Services;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;

        private int _userId;
        private string _username;
        private string _email;
        private string _status;
        private string _favoriteCity;
        private string _errorMessage;
        private string _successMessage;

        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string FavoriteCity
        {
            get => _favoriteCity;
            set => SetProperty(ref _favoriteCity, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public ICommand SaveProfileCommand { get; }

        public ProfileViewModel(int userId)
        {
            _apiClient = new ApiClient();
            UserId = userId;

            SaveProfileCommand = new RelayCommand(async _ => await SaveProfileAsync(), _ => CanSaveProfile());

            _ = LoadProfileAsync();
        }

        private async Task LoadProfileAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            var user = await _apiClient.GetUserAsync(UserId);
            if (user == null)
            {
                ErrorMessage = "User profile could not be loaded.";
                return;
            }

            Username = user.Username;
            Email = user.Email;
            Status = user.Status;
            FavoriteCity = user.FavoriteCity;
        }

        private bool CanSaveProfile()
        {
            if (string.IsNullOrWhiteSpace(Username))
                return false;

            if (string.IsNullOrWhiteSpace(Email))
                return false;

            if (!Email.Contains("@") || !Email.Contains("."))
                return false;

            return true;
        }

        public async Task SaveProfileAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (!CanSaveProfile())
            {
                ErrorMessage = "Please enter a valid username and email.";
                return;
            }

            var user = await _apiClient.GetUserAsync(UserId);
            if (user == null)
            {
                ErrorMessage = "User not found.";
                return;
            }

            user.Username = Username;
            user.Email = Email;
            user.Status = Status;
            user.FavoriteCity = FavoriteCity;

            var updated = await _apiClient.UpdateUserAsync(user);

            if (!updated)
            {
                ErrorMessage = "Saving profile failed. Email or username might already be in use.";
                return;
            }

            SuccessMessage = "Profile updated successfully.";
        }
    }
}
