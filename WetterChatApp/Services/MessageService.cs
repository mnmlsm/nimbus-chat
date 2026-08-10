using System.Collections.Generic;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.Services
{
    // Small facade over ApiClient for global and private chat messages, used
    // by MessagesWindow.
    public class MessageService
    {
        private readonly ApiClient _apiClient;

        public MessageService()
        {
            _apiClient = new ApiClient();
        }

        public Task<List<(Message Message, string DisplayName)>> GetAllGlobalMessagesAsync()
        {
            return _apiClient.GetGlobalMessagesAsync(0);
        }

        public Task<List<(Message Message, string DisplayName)>> GetNewGlobalMessagesSinceAsync(int lastMessageId)
        {
            return _apiClient.GetGlobalMessagesAsync(lastMessageId);
        }

        public Task<bool> SendGlobalMessageAsync(int senderId, string content)
        {
            return _apiClient.SendGlobalMessageAsync(senderId, content);
        }

        public Task<List<Message>> GetMessagesBetweenAsync(int userId1, int userId2)
        {
            return _apiClient.GetMessagesBetweenAsync(userId1, userId2);
        }

        public Task<bool> SendPrivateMessageAsync(int senderId, int receiverId, string content)
        {
            return _apiClient.SendPrivateMessageAsync(senderId, receiverId, content);
        }
    }
}
