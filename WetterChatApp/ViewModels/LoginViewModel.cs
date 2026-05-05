using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Repositories;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
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

            _userRepository = new UserRepository();

            LoginCommand = new RelayCommand(_ => ExecuteLogin(), _ => CanExecuteLogin());
        }

        private bool CanExecuteLogin()
        {
            // für Woche 2: sinnvolle minimale Validierung
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin()
        {
            ErrorMessage = string.Empty;

            var user = _userRepository.GetByEmail(Email);

            if (user == null)
            {
                ErrorMessage = "Benutzer nicht gefunden.";
                return;
            }

            // Einfacher Vergleich – später Hashen
            if (user.PasswordHash != Password)
            {
                ErrorMessage = "Ungültiges Passwort.";
                return;
            }

            // Login erfolgreich -> Dashboard
            var dashboard = new DashboardWindow();
            dashboard.Show();

            var loginWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is MainWindow);

            loginWindow?.Close();
        }
    }
}