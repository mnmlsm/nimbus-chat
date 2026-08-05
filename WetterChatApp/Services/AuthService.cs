using System.Threading.Tasks;
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

        public Task<User> LoginAsync(string email, string password)
        {
            return _apiClient.LoginAsync(email, password);
        }

        public Task<bool> RegisterAsync(string username, string email, string password)
        {
            return _apiClient.RegisterAsync(username, email, password);
        }
    }
}
