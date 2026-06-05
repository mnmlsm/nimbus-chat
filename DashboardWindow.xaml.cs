using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using NimbusChat.ViewModels;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat
{
    public partial class DashboardWindow : Window, INotifyPropertyChanged
    {
        private readonly User _currentUser;

        private string _weatherCity = "Leipzig";
        public string WeatherCity
        {
            get => _weatherCity;
            set
            {
                _weatherCity = value;
                OnPropertyChanged();
            }
        }

        private string _weatherTemperature = "14°C";
        public string WeatherTemperature
        {
            get => _weatherTemperature;
            set
            {
                _weatherTemperature = value;
                OnPropertyChanged();
            }
        }

        private string _weatherCondition = "Cloudy";
        public string WeatherCondition
        {
            get => _weatherCondition;
            set
            {
                _weatherCondition = value;
                OnPropertyChanged();
            }
        }

        public DashboardWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DataContext = this;
        }

        private void OpenWeather_Click(object sender, RoutedEventArgs e)
        {
            var window = new WeatherWindow
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                var vm = window.ViewModel;

                WeatherCity = vm.City;
                WeatherTemperature = vm.Temperature;
                WeatherCondition = vm.Condition;
            }
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_currentUser.Id)
            {
                Owner = this
            };

            profileWindow.ShowDialog();
        }

        private void OpenMessages_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Hier echten Chat-Partner bestimmen.
            // Zum Testen nehmen wir z.B. User mit Id 2:
            var otherUserId = 2;

            var messagesWindow = new MessagesWindow(_currentUser.Id, otherUserId)
            {
                Owner = this
            };

            messagesWindow.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Show();
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}