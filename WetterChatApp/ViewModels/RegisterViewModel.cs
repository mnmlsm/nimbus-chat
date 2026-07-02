using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;

        private string _email;
        private string _username;
        private string _confirmPassword;
        private string _password;
        private string _errorMessage;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
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
            RegisterCommand = new RelayCommand(_ => ExecuteRegister(), _ => CanRegister());
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
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

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            var user = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password
            };

            var success = _userRepository.Create(user);

            if (!success)
            {
                ErrorMessage = "Registration failed.";
                return;
            }

            MessageBox.Show("Registration successful!");

            var login = new MainWindow();
            login.Show();

            var registerWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is Views.RegisterView);

            registerWindow?.Close();
        }
    }
}