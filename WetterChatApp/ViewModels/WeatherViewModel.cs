using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using NimbusChat.WetterChatApp.Infrastructure;

namespace NimbusChat.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        private string _city;
        private string _cityName;
        private string _temperature;
        private string _description;
        private string _humidity;
        private string _wind;
        private string _feelsLike;

        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged();
            }
        }

        public string CityName
        {
            get => _cityName;
            set
            {
                _cityName = value;
                OnPropertyChanged();
            }
        }

        public string Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Humidity
        {
            get => _humidity;
            set
            {
                _humidity = value;
                OnPropertyChanged();
            }
        }

        public string Wind
        {
            get => _wind;
            set
            {
                _wind = value;
                OnPropertyChanged();
            }
        }

        public string FeelsLike
        {
            get => _feelsLike;
            set
            {
                _feelsLike = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }

        public WeatherViewModel()
        {
            SearchCommand = new RelayCommand(async _ => await Search());
        }

        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(City))
                return;

            try
            {
                using (var client = new HttpClient())
                {
                    var url =
                        $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(City)}&appid=9a88681c950ff48215f6b23a7b43f21a&units=metric";

                    var response = await client.GetStringAsync(url);

                    dynamic weather = JsonConvert.DeserializeObject(response);

                    CityName = weather.name;
                    Temperature = $"{weather.main.temp:0}°C";
                    Description = weather.weather[0].description.ToString();
                    Humidity = $"{weather.main.humidity}%";
                    Wind = $"{weather.wind.speed:0.#} m/s";
                    FeelsLike = $"{weather.main.feels_like:0}°C";
                }
            }
            catch (Exception ex)
            {
                Description = ex.Message;
            }
        }
    }
}
