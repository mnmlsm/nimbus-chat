using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Config;
using NimbusChat.WetterChatApp.Models;

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

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password }).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<User> GetUserAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/users/{id}").ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return null;

            return ToUser(await response.Content.ReadFromJsonAsync<UserResponse>().ConfigureAwait(false));
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

        private class GlobalMessageResponse
        {
            public int Id { get; set; }
            public int SenderId { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
            public string SenderName { get; set; }
        }
    }
}
