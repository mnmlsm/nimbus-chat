using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;

namespace NimbusChat
{
    public partial class DashboardWindow : Window, INotifyPropertyChanged
    {
        private readonly User _currentUser;
        private readonly UserRepository _userRepository = new UserRepository();

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

        public string Username => _currentUser.Username;

        public string UserStatus =>
            string.IsNullOrWhiteSpace(_currentUser.Status)
                ? "Offline"
                : _currentUser.Status;

        public string FavoriteCity =>
            string.IsNullOrWhiteSpace(_currentUser.FavoriteCity)
                ? "Not selected"
                : _currentUser.FavoriteCity;

        public string WelcomeText =>
            $"Welcome back, {_currentUser.Username}! 👋";

        public DashboardWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;

            // ОБЯЗАТЕЛЬНО!
            DataContext = this;

            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
            OnPropertyChanged(nameof(WelcomeText));
            OnPropertyChanged(nameof(Username));
            OnPropertyChanged(nameof(UserStatus));
            OnPropertyChanged(nameof(FavoriteCity));

            OnPropertyChanged(nameof(WeatherCity));
            OnPropertyChanged(nameof(WeatherTemperature));
            OnPropertyChanged(nameof(WeatherCondition));
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_currentUser.Id)
            {
                Owner = this
            };

            if (profileWindow.ShowDialog() == true)
            {
                var updatedUser = _userRepository.GetById(_currentUser.Id);

                if (updatedUser != null)
                {
                    _currentUser.Username = updatedUser.Username;
                    _currentUser.Status = updatedUser.Status;
                    _currentUser.FavoriteCity = updatedUser.FavoriteCity;

                    RefreshDashboard();
                }
            }
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

        private void OpenMessages_Click(object sender, RoutedEventArgs e)
        {
            var messagesWindow = new MessagesWindow(_currentUser.Id)
            {
                Owner = this
            };

            messagesWindow.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _userRepository.UpdateStatus(_currentUser.Id, "Offline");

            var login = new MainWindow();
            login.Show();

            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _userRepository.UpdateStatus(_currentUser.Id, "Offline");
            base.OnClosing(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}