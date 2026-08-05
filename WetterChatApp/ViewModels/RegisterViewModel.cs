using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Services;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
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

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            _authService = new AuthService();
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

            var success = _authService.Register(Email, Password);

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
                .FirstOrDefault(w => w is Views.RegisterView);

            registerWindow?.Close();
        }
    }
}
