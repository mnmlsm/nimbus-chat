using Newtonsoft.Json;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Infrastructure;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NimbusChat.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
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
            SearchCommand = new RelayCommand(_ => Search());
        }

        private async void Search()
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
                    var url = $"https://api.openweathermap.org/data/2.5/weather?q={City}&appid=ТВОЙ_API_KEY&units=metric";

                    var response = await client.GetStringAsync(url);

                    dynamic data = JsonConvert.DeserializeObject(response);

                    var temp = data.main.temp;
                    var description = data.weather[0].description;

                    WeatherResult = $"{City}: {temp}°C, {description}";
                }
            }
            catch (Exception ex)
            {
                WeatherResult = ex.Message;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}