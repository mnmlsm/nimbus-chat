using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace NimbusChat
{
    // Code-behind for the main dashboard: loads the user's weather/forecast,
    // keeps a periodic server-connection check running, and opens the
    // chat/weather/profile sub-windows.
    public partial class DashboardWindow : Window, INotifyPropertyChanged
    {
        private readonly User _currentUser;
        private readonly ApiClient _apiClient = new ApiClient();

        public ObservableCollection<ForecastDay> Forecast
        {
            get;
        }
         = new ObservableCollection<ForecastDay>();

        private string _weatherCity = "Select your city.";
        public string WeatherCity
        {
            get => _weatherCity;
            set
            {
                _weatherCity = value;
                OnPropertyChanged();
            }
        }

        private string _weatherTemperature = "-";
        public string WeatherTemperature
        {
            get => _weatherTemperature;
            set
            {
                _weatherTemperature = value;
                OnPropertyChanged();
            }
        }

        private string _weatherCondition = "-";
        public string WeatherCondition
        {
            get => _weatherCondition;
            set
            {
                _weatherCondition = value;
                OnPropertyChanged();
            }
        }

        private string _weatherHumidity = "-";
        public string WeatherHumidity
        {
            get => _weatherHumidity;
            set
            {
                _weatherHumidity = value;
                OnPropertyChanged();
            }
        }

        private string _weatherWind = "-";
        public string WeatherWind
        {
            get => _weatherWind;
            set
            {
                _weatherWind = value;
                OnPropertyChanged();
            }
        }



        
        public string Username => _currentUser.Username;

        public string Email => _currentUser.Email;

        public string UserStatus =>
            string.IsNullOrWhiteSpace(_currentUser.Status)
                ? "Offline"
                : _currentUser.Status;



        public string FavoriteCity =>
            string.IsNullOrWhiteSpace(_currentUser.FavoriteCity)
                ? "Not selected"
                : _currentUser.FavoriteCity;

        public string MessagesInfo => "Open your chats";

        public string WeatherInfo =>
            string.IsNullOrWhiteSpace(_currentUser.FavoriteCity)
                ? "No city selected"
                : _currentUser.FavoriteCity;

        public string ProfileInfo => UserStatus;

        public string WelcomeText
        {
            get
            {
                int hour = DateTime.Now.Hour;

                if (hour >= 5 && hour < 12)
                    return $"Good Morning, {_currentUser.Username}!";

                if (hour >= 12 && hour < 17)
                    return $"Good Afternoon, {_currentUser.Username}!";

                if (hour >= 17 && hour < 22)
                    return $"Good Evening, {_currentUser.Username}!";

                return $"Good Night, {_currentUser.Username}!";
            }
        }

        public string GreetingIcon
        {
            get
            {
                int hour = DateTime.Now.Hour;

                if (hour >= 5 && hour < 12)
                    return "WeatherSunsetUp";

                if (hour >= 12 && hour < 17)
                    return "WhiteBalanceSunny";

                if (hour >= 17 && hour < 22)
                    return "WeatherSunsetDown";

                return "WeatherNight";
            }
        }

        public string UserInitial
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_currentUser.Username))
                    return "?";

                return _currentUser.Username.Substring(0, 1).ToUpper();
            }
        }

        public System.Windows.Media.Brush AvatarBrush
        {
            get
            {
                var colors = new[]
                {
            "#4F8EF7",
            "#8B5CF6",
            "#14B8A6",
            "#F97316",
            "#EF4444",
            "#10B981"
        };

                int index = Math.Abs(_currentUser.Username.GetHashCode()) % colors.Length;

                return (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter()
                    .ConvertFrom(colors[index]);
            }
        }

        public Brush UserStatusBrush
        {
            get
            {
                switch (UserStatus)
                {
                    case "Online":
                        return Brushes.LimeGreen;

                    case "Busy":
                        return Brushes.IndianRed;

                    case "Away":
                        return Brushes.Gold;

                    default:
                        return Brushes.Gray;
                }
            }
        }

        private readonly DispatcherTimer _connectionCheckTimer;

        public DashboardWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;

            DataContext = this;

            RefreshDashboard();

            Loaded += DashboardWindow_Loaded;

            _connectionCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };
            _connectionCheckTimer.Tick += async (s, e) => await CheckConnectionAsync();
            _connectionCheckTimer.Start();
        }

        private async void DashboardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFavoriteCityWeatherAsync();
            await CheckConnectionAsync();
        }

        private async Task CheckConnectionAsync()
        {
            try
            {
                await _apiClient.GetHealthAsync();
                ConnectionDot.Fill = Brushes.LimeGreen;
                ConnectionText.Text = "WEATHER · CONNECTED";
            }
            catch
            {
                ConnectionDot.Fill = Brushes.Red;
                ConnectionText.Text = "WEATHER · NOT CONNECTED";
            }
        }

        // Fallback for as long as the user hasn't saved a city in their profile
        // yet, so weather and forecast on the dashboard never stay empty.
        private const string DefaultCity = "Berlin";

        private async Task LoadFavoriteCityWeatherAsync()
        {
            var city = string.IsNullOrWhiteSpace(_currentUser.FavoriteCity)
                ? DefaultCity
                : _currentUser.FavoriteCity;

            await LoadWeatherForCityAsync(city);
        }

        private async Task LoadWeatherForCityAsync(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return;

            try
            {
                // The API calls OpenWeatherMap and writes the lookup to the
                // database; only the finished result arrives here.
                var data = await _apiClient.GetWeatherAsync(cityName, _currentUser.Id);

                WeatherCity = cityName;
                WeatherTemperature = $"{data.Temperature:0.#}°C";
                WeatherCondition = data.Description;
                WeatherHumidity = $"{data.Humidity}%";

                double windKmH = data.WindSpeed * 3.6;
                WeatherWind = $"{windKmH:0.#} km/h";

                UpdateWeatherVisual();
                await LoadForecastAsync(cityName);
            }
            catch (Exception)
            {
                AppMessageBox.Show("Weather data could not be loaded.", "Weather Error", AppMessageBoxIcon.Error, this);

                WeatherTemperature = "-";
                WeatherCondition = "Weather unavailable";
                WeatherHumidity = "-";
                WeatherWind = "-";

                UpdateWeatherVisual();
            }
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
            OnPropertyChanged(nameof(WeatherHumidity));
            OnPropertyChanged(nameof(WeatherWind));
            OnPropertyChanged(nameof(UserInitial));
            OnPropertyChanged(nameof(AvatarBrush));
            OnPropertyChanged(nameof(UserStatusBrush));
            OnPropertyChanged(nameof(MessagesInfo));
            OnPropertyChanged(nameof(WeatherInfo));
            OnPropertyChanged(nameof(ProfileInfo));
        }

        private async void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_currentUser.Id)
            {
                Owner = this
            };

            if (profileWindow.ShowDialog() == true)
            {
                var updatedUser = await _apiClient.GetUserAsync(_currentUser.Id);

                if (updatedUser != null)
                {
                    _currentUser.Username = updatedUser.Username;
                    _currentUser.Email = updatedUser.Email;
                    _currentUser.Status = updatedUser.Status;
                    _currentUser.FavoriteCity = updatedUser.FavoriteCity;

                    RefreshDashboard();

                    
                    await LoadFavoriteCityWeatherAsync();
                }
            }
        }

        private async void OpenWeather_Click(object sender, RoutedEventArgs e)
        {
            var window = new WeatherWindow
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                var vm = window.ViewModel;

                await LoadWeatherForCityAsync(vm.City);
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

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            await _apiClient.UpdateStatusAsync(_currentUser.Id, "Offline");

            var login = new MainWindow();
            login.Show();

            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _connectionCheckTimer?.Stop();

            // Fire-and-forget: the window closes immediately without waiting for
            // the network response (the app keeps running for a moment anyway).
            _ = _apiClient.UpdateStatusAsync(_currentUser.Id, "Offline");
            base.OnClosing(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateWeatherVisual()
        {
            string c = WeatherCondition.ToLower();

            SunnyVisual.Visibility = Visibility.Collapsed;
            CloudyVisual.Visibility = Visibility.Collapsed;
            RainVisual.Visibility = Visibility.Collapsed;
            SnowVisual.Visibility = Visibility.Collapsed;

            if (c.Contains("clear"))
            {
                SunnyVisual.Visibility = Visibility.Visible;
            }
            else if (c.Contains("rain") ||
                     c.Contains("drizzle") ||
                     c.Contains("thunder"))
            {
                RainVisual.Visibility = Visibility.Visible;
            }
            else if (c.Contains("snow"))
            {
                SnowVisual.Visibility = Visibility.Visible;
            }
            else if (c.Contains("cloud"))
            {
                CloudyVisual.Visibility = Visibility.Visible;
            }
            else if (c.Contains("mist") ||
                     c.Contains("fog") ||
                     c.Contains("haze"))
            {
                CloudyVisual.Visibility = Visibility.Visible;
            }
            else
            {
                CloudyVisual.Visibility = Visibility.Visible;
            }
        }

        private async Task LoadForecastAsync(string city)
        {
            Forecast.Clear();

            // Grouping down to one entry per day happens server-side.
            foreach (var day in await _apiClient.GetForecastAsync(city))
                Forecast.Add(day);
        }
    }
}