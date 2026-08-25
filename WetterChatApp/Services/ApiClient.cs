using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Config;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.Services
{
    // Thin HTTP wrapper around every server endpoint (auth, users, weather,
    // messages). This is the only place in the client that talks to the API.
    public class ApiClient
    {
        // A single HttpClient is shared for the app's whole lifetime instead of
        // opening a new TCP/TLS handshake on every "new ApiClient()".
        private static readonly HttpClient _httpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var config = ClientConfig.Load();

            var client = new HttpClient
            {
                BaseAddress = new Uri(config.ApiBaseUrl)
            };

            // Shared, weak API-key gate on the API side - see
            // nimbus-chat.Api\Program.cs. Set once here so it's attached to
            // every request automatically.
            if (!string.IsNullOrWhiteSpace(config.ApiKey))
                client.DefaultRequestHeaders.Add("X-Api-Key", config.ApiKey);

            return client;
        }

        public async Task<string> GetHealthAsync()
        {
            var response = await _httpClient.GetAsync("/api/health").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password }).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return null;

            return ToUser(await response.Content.ReadFromJsonAsync<UserResponse>().ConfigureAwait(false));
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new { Username = username, Email = email, Password = password }).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<User> GetUserAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/users/{id}").ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return null;

            return ToUser(await response.Content.ReadFromJsonAsync<UserResponse>().ConfigureAwait(false));
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync("/api/users").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<List<UserResponse>>().ConfigureAwait(false) ?? new List<UserResponse>();
            var result = new List<User>();

            foreach (var item in items)
                result.Add(ToUser(item));

            return result;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/users/{user.Id}", new
            {
                user.Username,
                user.Email,
                user.Status,
                user.FavoriteCity
            }).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/users/{id}/status", new { Status = status }).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        // The API fetches weather from OpenWeatherMap and logs every lookup to
        // the WeatherData table; the client only ever sees the result.
        public async Task<WeatherData> GetWeatherAsync(string city, int userId = 0)
        {
            var response = await _httpClient
                .GetAsync($"/api/weather?city={Uri.EscapeDataString(city)}&userId={userId}")
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<WeatherData>().ConfigureAwait(false);
        }

        public async Task<List<ForecastDay>> GetForecastAsync(string city)
        {
            var response = await _httpClient
                .GetAsync($"/api/weather/forecast?city={Uri.EscapeDataString(city)}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var items = await response.Content
                .ReadFromJsonAsync<List<ForecastDayResponse>>()
                .ConfigureAwait(false)
                ?? new List<ForecastDayResponse>();

            var result = new List<ForecastDay>();

            foreach (var item in items)
            {
                result.Add(new ForecastDay
                {
                    Day = LanguageManager.GetDayName(item.Date),
                    Temperature = $"{item.Temperature:0}°",
                    Icon = item.Icon,
                    IconUrl = $"https://openweathermap.org/img/wn/{item.Icon}@2x.png",
                    Condition = item.Condition
                });
            }

            return result;
        }

        public async Task<List<(Message Message, string DisplayName)>> GetGlobalMessagesAsync(int since = 0)
        {
            var response = await _httpClient.GetAsync($"/api/messages?since={since}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<List<GlobalMessageResponse>>().ConfigureAwait(false) ?? new List<GlobalMessageResponse>();
            var result = new List<(Message, string)>();

            foreach (var item in items)
            {
                result.Add((new Message
                {
                    Id = item.Id,
                    SenderId = item.SenderId,
                    Content = item.Content,
                    CreatedAt = item.CreatedAt
                }, item.SenderName));
            }

            return result;
        }

        public async Task<bool> SendGlobalMessageAsync(int senderId, string content)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/messages", new { SenderId = senderId, Content = content }).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Message>> GetMessagesBetweenAsync(int userId1, int userId2)
        {
            var response = await _httpClient.GetAsync($"/api/messages/between?user1={userId1}&user2={userId2}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<List<MessageResponse>>().ConfigureAwait(false) ?? new List<MessageResponse>();
            var result = new List<Message>();

            foreach (var item in items)
            {
                result.Add(new Message
                {
                    Id = item.Id,
                    SenderId = item.SenderId,
                    ReceiverId = item.ReceiverId,
                    Content = item.Content,
                    CreatedAt = item.CreatedAt
                });
            }

            return result;
        }

        public async Task<bool> SendPrivateMessageAsync(int senderId, int receiverId, string content)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/messages", new { SenderId = senderId, ReceiverId = receiverId, Content = content }).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        private static User ToUser(UserResponse dto)
        {
            if (dto == null)
                return null;

            return new User
            {
                Id = dto.Id,
                Username = dto.Username,
                Email = dto.Email,
                Status = dto.Status,
                FavoriteCity = dto.FavoriteCity
            };
        }

        private class UserResponse
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
            public string FavoriteCity { get; set; }
        }

        private class ForecastDayResponse
        {
            public DateTime Date { get; set; }
            public double Temperature { get; set; }
            public string Icon { get; set; }
            public string Condition { get; set; }
        }

        private class GlobalMessageResponse
        {
            public int Id { get; set; }
            public int SenderId { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
            public string SenderName { get; set; }
        }

        private class MessageResponse
        {
            public int Id { get; set; }
            public int SenderId { get; set; }
            public int ReceiverId { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
