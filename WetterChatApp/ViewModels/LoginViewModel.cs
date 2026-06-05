using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Services;

namespace NimbusChat.WetterChatApp.ViewModels
{
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

            try
            {
                var user = await _authService.LoginAsync(Email, Password);

                if (user == null)
                {
                    ErrorMessage = "Ungültige E-Mail oder Passwort.";
                    return;
                }

                var dashboard = new DashboardWindow();
                dashboard.Show();

                var loginWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w is MainWindow);

                loginWindow?.Close();
            }
            catch
            {
                ErrorMessage = "Login fehlgeschlagen.";
            }
        }
    }
}