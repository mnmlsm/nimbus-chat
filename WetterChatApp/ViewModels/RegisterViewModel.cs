using System.Linq;
using System.Windows;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Repositories;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
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

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            _userRepository = new UserRepository();
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

            _userRepository.AddUser(Email, Password);

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