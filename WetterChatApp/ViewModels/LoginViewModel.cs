using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Models;
using System.Windows.Input;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private string _errorMessage;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login(object parameter)
        {
            // Woche 1: nur einfache Validierung, noch kein DB-Check
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter email and password";
                return;
            }

            ErrorMessage = string.Empty;

            // Woche 1: Noch kein Dashboard-Öffnen im ViewModel
            // (Fenster-Logik bleibt im MainWindow-Code-Behind)
        }
    }
}