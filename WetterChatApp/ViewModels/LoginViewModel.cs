using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Services;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly ApiClient _apiClient;

        private string _email;
        private string _password;
        private string _errorMessage;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            _authService = new AuthService();
            _apiClient = new ApiClient();
            LoginCommand = new RelayCommand(async _ => await ExecuteLoginAsync(), _ => CanExecuteLogin());
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async Task ExecuteLoginAsync()
        {
            ErrorMessage = string.Empty;

            User user;
            try
            {
                user = await _authService.LoginAsync(Email, Password);
            }
            catch (Exception)
            {
                ErrorMessage = "The server cannot be reached right now. Please try again later.";
                return;
            }

            if (user == null)
            {
                ErrorMessage = "Invalid email or password.";
                return;
            }

            try
            {
                await _apiClient.UpdateStatusAsync(user.Id, "Online");
            }
            catch (Exception)
            {
                // Login war erfolgreich; ein fehlgeschlagenes Status-Update soll das nicht blockieren.
            }

            // Dashboard mit aktuellem User öffnen
            var dashboard = new DashboardWindow(user);
            dashboard.Show();

            var loginWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is MainWindow);

            loginWindow?.Close();
        }
    }
}
