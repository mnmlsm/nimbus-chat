using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Config;

namespace NimbusChat.WetterChatApp.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            var config = ClientConfig.Load();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(config.ApiBaseUrl)
            };
        }

        public async Task<string> GetHealthAsync()
        {
            var response = await _httpClient.GetAsync("/api/health");
            response.EnsureSuccessStatusCode();

            var text = await response.Content.ReadAsStringAsync();
            return text;
        }

        // Hier kannst du später weitere Methoden ergänzen:
        // z.B. GetMessagesAsync(), CreateMessageAsync(), GetUsersAsync(), ...
    }
}