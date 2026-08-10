using System;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Infrastructure;
using NimbusChat.WetterChatApp.Services;

namespace NimbusChat.WetterChatApp.ViewModels
{
    // Simple API-health-check view model. Not currently wired up to any view.
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
            ApiStatus = "Not checked yet";
        }

        public async Task CheckApiAsync()
        {
            try
            {
                ApiStatus = "Checking API...";
                var result = await _apiClient.GetHealthAsync();
                ApiStatus = $"API: {result}";
            }
            catch (Exception ex)
            {
                ApiStatus = $"Error: {ex.Message}";
            }
        }
    }
}