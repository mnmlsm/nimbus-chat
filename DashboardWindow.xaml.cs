using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using NimbusChat.ViewModels;

namespace NimbusChat
{
    public partial class DashboardWindow : Window, INotifyPropertyChanged
    {
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

        public DashboardWindow()
        {
            InitializeComponent();
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
            var profileWindow = new ProfileWindow
            {
                Owner = this
            };

            profileWindow.ShowDialog();
        }

        private void OpenMessages_Click(object sender, RoutedEventArgs e)
        {
            var messagesWindow = new MessagesWindow
            {
                Owner = this
            };

            messagesWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}