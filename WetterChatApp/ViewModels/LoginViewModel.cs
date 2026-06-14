using System.Linq;
using System.Windows;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Repositories;
using NimbusChat.WetterChatApp.Services;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly UserRepository _userRepository;

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
            // DB vorbereiten
            DatabaseInitializer.Initialize();

            _authService = new AuthService();
            _userRepository = new UserRepository();
            LoginCommand = new RelayCommand(_ => ExecuteLogin(), _ => CanExecuteLogin());
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin()
        {
            ErrorMessage = string.Empty;

            // Einfach synchroner Call auf AuthService
            var user = _authService.Login(Email, Password);

            if (user == null)
            {
                ErrorMessage = "Invalid email or password.";
                return;
            }

            _userRepository.UpdateStatus(user.Id, "Online");

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