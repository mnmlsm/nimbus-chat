using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;
using NimbusChat.WetterChatApp.Services; // für AuthService
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;
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

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            _userRepository = new UserRepository();
            _authService = new AuthService(); // nutzt SHA-256-Hashing
            RegisterCommand = new RelayCommand(_ => ExecuteRegister(), _ => CanRegister());
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteRegister()
        {
            ErrorMessage = "";

            var existingUser = _userRepository.GetByEmail(Email);

            if (existingUser != null)
            {
                ErrorMessage = "User already exists";
                return;
            }

            // Passwort hashen statt Klartext speichern
            var hashedPassword = _authService.HashPassword(Password);

            var user = new User
            {
                Username = Email,
                Email = Email,
                PasswordHash = hashedPassword
            };

            var success = _userRepository.Create(user);

            if (!success)
            {
                ErrorMessage = "Registration failed.";
                return;
            }

            MessageBox.Show("Registration successful!");

            // zurück zum Login/MainWindow
            var login = new MainWindow();
            login.Show();

            var registerWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is Views.RegisterView);

            registerWindow?.Close();
        }
    }
}