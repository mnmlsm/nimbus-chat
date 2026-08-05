using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.Services
{
    public class AuthService
    {
        private readonly ApiClient _apiClient;

        public AuthService()
        {
            _apiClient = new ApiClient();
        }

        public User Login(string email, string password)
        {
            return _apiClient.LoginAsync(email, password).GetAwaiter().GetResult();
        }

        public bool Register(string email, string password)
        {
            return _apiClient.RegisterAsync(email, password).GetAwaiter().GetResult();
        }
    }
}
