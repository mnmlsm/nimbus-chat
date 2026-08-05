using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        private string _username;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _errorMessage;

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

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            _authService = new AuthService();
            RegisterCommand = new RelayCommand(async _ => await ExecuteRegisterAsync(), _ => CanRegister());
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private async Task ExecuteRegisterAsync()
        {
            ErrorMessage = "";

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            var success = await _authService.RegisterAsync(Username, Email, Password);

            if (!success)
            {
                ErrorMessage = "User already exists or registration failed.";
                return;
            }

            MessageBox.Show("Registration successful!");

            // zurück zum Login/MainWindow
            var login = new MainWindow();
            login.Show();

            var registerWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is RegisterWindow);

            registerWindow?.Close();
        }
    }
}
