using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Infrastructure;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Infrastructure;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
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
            // Command ist immer verfügbar (Woche 1)
            LoginCommand = new RelayCommand(_ => ExecuteLogin(), _ => CanExecuteLogin());
        }

        private bool CanExecuteLogin()
        {
            // Wenn du absolut jede Eingabe zulassen willst:
            return true;

            // Wenn du nur nicht-leere Felder zulassen willst:
            // return !string.IsNullOrWhiteSpace(Email) &&
            //        !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin()
        {
            // Woche 1: jedes Login als erfolgreich behandeln,
            // solange E-Mail und Passwort nicht leer sind
            if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = string.Empty;

                // Dashboard öffnen (dein vorhandenes DashboardWindow)
                var dashboard = new DashboardWindow();
                dashboard.Show();

                // Login-Fenster (MainWindow) schließen
                var loginWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w is MainWindow);

                loginWindow?.Close();
            }
            else
            {
                ErrorMessage = "Bitte E-Mail und Passwort eingeben.";
            }
        }
    }
}