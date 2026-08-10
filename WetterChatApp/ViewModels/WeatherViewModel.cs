using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Services;

namespace NimbusChat.ViewModels
{
    // Backing view model for the standalone weather search dialog.
    public class WeatherViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient = new ApiClient();

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
                // Goes through the API so the lookup also gets recorded in the database.
                var weather = await _apiClient.GetWeatherAsync(City);

                CityName = weather.City;
                Temperature = $"{weather.Temperature:0}°C";
                Description = weather.Description;
                Humidity = $"{weather.Humidity}%";
                Wind = $"{weather.WindSpeed:0.#} m/s";
                FeelsLike = $"{weather.FeelsLike:0}°C";
            }
            catch (Exception ex)
            {
                Description = ex.Message;
            }
        }
    }
}
