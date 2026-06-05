using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;

namespace NimbusChat.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        private readonly WeatherDataRepository _weatherRepository;

        private string _city;
        private string _weatherResult;
        private string _temperature;
        private string _condition;
        private bool? _dialogResult;

        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged();
            }
        }

        public string WeatherResult
        {
            get => _weatherResult;
            set
            {
                _weatherResult = value;
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

        public string Condition
        {
            get => _condition;
            set
            {
                _condition = value;
                OnPropertyChanged();
            }
        }

        public bool? DialogResultValue
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }

        public WeatherViewModel()
        {
            _weatherRepository = new WeatherDataRepository();
            SearchCommand = new RelayCommand(async _ => await Search());
        }

        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(City))
            {
                WeatherResult = "Enter a city";
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var url =
                        $"https://api.openweathermap.org/data/2.5/weather?q={City}&appid=87f7befd922e98326b86d972952a799c&units=metric";

                    var response = await client.GetStringAsync(url);

                    dynamic data = JsonConvert.DeserializeObject(response);

                    // Werte aus dem JSON holen (OpenWeather-Struktur)
                    double temp = data.main.temp;
                    int humidity = data.main.humidity;
                    string description = data.weather[0].description;

                    // UI aktualisieren
                    Temperature = $"{temp}°C";
                    Condition = description;
                    WeatherResult = $"{City}: {temp}°C, {description}";

                    // WeatherData Objekt für die DB bauen
                    var weatherData = new WeatherData
                    {
                        City = City,
                        Temperature = temp,
                        Humidity = humidity,
                        Description = description,
                        CreatedAt = DateTime.UtcNow.ToString("o") // ISO 8601
                    };

                    // In SQLite speichern
                    _weatherRepository.Create(weatherData);

                    DialogResultValue = true;
                }
            }
            catch (Exception ex)
            {
                WeatherResult = ex.Message;
            }
        }
    }
}