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
        private string _weatherResult;

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

        public ICommand SearchCommand { get; }

        public WeatherViewModel()
        {
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

                    var temp = data.main.temp;
                    var description = data.weather[0].description;

                    WeatherResult =
                        $"{City}: {temp}°C, {description}";
                }
            }
            catch (Exception ex)
            {
                WeatherResult = ex.Message;
            }
        }
    }
}