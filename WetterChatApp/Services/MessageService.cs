using System.Collections.Generic;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.Services
{
    public class MessageService
    {
        private readonly ApiClient _apiClient;

        public MessageService()
        {
            _apiClient = new ApiClient();
        }

        // Alle globalen Nachrichten
        public Task<List<(Message Message, string DisplayName)>> GetAllGlobalMessagesAsync()
        {
            return _apiClient.GetGlobalMessagesAsync(0);
        }

        // Neue globale Nachrichten seit einer bestimmten Id
        public Task<List<(Message Message, string DisplayName)>> GetNewGlobalMessagesSinceAsync(int lastMessageId)
        {
            return _apiClient.GetGlobalMessagesAsync(lastMessageId);
        }

        // Global-Nachricht senden
        public Task<bool> SendGlobalMessageAsync(int senderId, string content)
        {
            return _apiClient.SendGlobalMessageAsync(senderId, content);
        }

        // Privater Chat: Nachrichten zwischen zwei Nutzern
        public Task<List<Message>> GetMessagesBetweenAsync(int userId1, int userId2)
        {
            return _apiClient.GetMessagesBetweenAsync(userId1, userId2);
        }

        // Privater Chat: Nachricht senden
        public Task<bool> SendPrivateMessageAsync(int senderId, int receiverId, string content)
        {
            return _apiClient.SendPrivateMessageAsync(senderId, receiverId, content);
        }
    }
}
