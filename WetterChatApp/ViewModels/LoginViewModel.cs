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
    // Backing view model for the login screen: validates input, calls the
    // login API, and opens the dashboard on success.
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

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

            // The login endpoint already sets the status to Online server-side,
            // so user.Status comes back with the correct value here.
            var dashboard = new DashboardWindow(user);
            dashboard.Show();

            var loginWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is MainWindow);

            loginWindow?.Close();
        }
    }
}
