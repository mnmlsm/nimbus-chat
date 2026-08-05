using Newtonsoft.Json;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NimbusChat
{
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

        public DashboardWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;

            DataContext = this;

            RefreshDashboard();

            Loaded += DashboardWindow_Loaded;
        }

        private async void DashboardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFavoriteCityWeatherAsync();
        }

        private async Task LoadFavoriteCityWeatherAsync()
        {
            if (string.IsNullOrWhiteSpace(_currentUser.FavoriteCity))
            {
                WeatherCity = "Select your city";
                WeatherTemperature = "-";
                WeatherCondition = "No weather data";
                WeatherHumidity = "-";
                WeatherWind = "-";

                UpdateWeatherVisual();
                return;
            }

            await LoadWeatherForCityAsync(_currentUser.FavoriteCity);
        }

        private async Task LoadWeatherForCityAsync(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return;

            try
            {
                using (var client = new HttpClient())
                {
                    string city = Uri.EscapeDataString(cityName);

                    string url =
    $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(cityName)}&appid=9a88681c950ff48215f6b23a7b43f21a&units=metric";

                    string response = await client.GetStringAsync(url);

                    dynamic data = JsonConvert.DeserializeObject(response);


                    double temperature = data.main.temp;
                    int humidity = data.main.humidity;
                    double windSpeed = data.wind.speed;
                    string condition = data.weather[0].description;

                    WeatherCity = cityName;
                    WeatherTemperature = $"{temperature:0.#}°C";
                    WeatherCondition = condition;
                    WeatherHumidity = $"{humidity}%";

                    double windKmH = windSpeed * 3.6;
                    WeatherWind = $"{windKmH:0.#} km/h";

                    UpdateWeatherVisual();
                    await LoadForecastAsync(cityName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

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
            // Fire-and-forget: das Fenster schließt sofort, ohne auf die
            // Netzwerkantwort zu warten (die App läuft ohnehin noch kurz weiter).
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

            using (var client = new HttpClient())
            {
                string url =
        $"https://api.openweathermap.org/data/2.5/forecast?q={Uri.EscapeDataString(city)}&appid=9a88681c950ff48215f6b23a7b43f21a&units=metric";

                string json = await client.GetStringAsync(url);

                dynamic data = JsonConvert.DeserializeObject(json);

                var today = DateTime.Now.Date;
                var addedDates = new HashSet<DateTime>();

                foreach (var item in data.list)
                {
                    DateTime date =
                        DateTime.Parse((string)item.dt_txt);

                    // Nur zukünftige Tage anzeigen, nicht den heutigen.
                    if (date.Date <= today)
                        continue;

                    if (addedDates.Contains(date.Date))
                        continue;

                    addedDates.Add(date.Date);

                    string iconCode = item.weather[0].icon.ToString();

                    Forecast.Add(new ForecastDay
                    {
                        Day = date.ToString("ddd"),
                        Temperature = $"{(double)item.main.temp:0}°",
                        IconUrl = $"https://openweathermap.org/img/wn/{iconCode}@2x.png"
                    });

                    if (Forecast.Count == 5)
                        break;
                }
            }
        }
    }
}