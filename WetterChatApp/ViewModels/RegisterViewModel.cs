using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;

        private string _username;
        private string _email;
        private string _password;
        private string _errorMessage;
        private string _successMessage;

        public RegisterViewModel()
        {
            DatabaseInitializer.Initialize();
            _userRepository = new UserRepository();

            RegisterCommand = new RelayCommand(_ => ExecuteRegister(), _ => CanExecuteRegister());
        }

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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public ICommand RegisterCommand { get; }

        private bool CanExecuteRegister()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteRegister()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            var user = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password // später Hashen
            };

            var created = _userRepository.Create(user);

            if (created)
            {
                SuccessMessage = "Registrierung erfolgreich. Du kannst dich jetzt einloggen.";
            }
            else
            {
                ErrorMessage = "Registrierung fehlgeschlagen. Username/E-Mail eventuell bereits vorhanden.";
            }
        }
    }
}