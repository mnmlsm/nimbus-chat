using System;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Services;

namespace NimbusChat.WetterChatApp.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;

        private string _apiStatus;
        public string ApiStatus
        {
            get => _apiStatus;
            set
            {
                _apiStatus = value;
                OnPropertyChanged();
            }
        }

        public MessageViewModel()
        {
            _apiClient = new ApiClient();
            ApiStatus = "Noch nicht geprüft";
        }

        public async Task CheckApiAsync()
        {
            try
            {
                ApiStatus = "Prüfe API...";
                var result = await _apiClient.GetHealthAsync();
                ApiStatus = $"API: {result}";
            }
            catch (Exception ex)
            {
                ApiStatus = $"Fehler: {ex.Message}";
            }
        }
    }
}