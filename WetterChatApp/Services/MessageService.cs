using System.Collections.Generic;
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
        public List<(Message Message, string DisplayName)> GetAllGlobalMessages()
        {
            return _apiClient.GetGlobalMessagesAsync(0).GetAwaiter().GetResult();
        }

        // Neue globale Nachrichten seit einer bestimmten Id
        public List<(Message Message, string DisplayName)> GetNewGlobalMessagesSince(int lastMessageId)
        {
            return _apiClient.GetGlobalMessagesAsync(lastMessageId).GetAwaiter().GetResult();
        }

        // Global-Nachricht senden
        public bool SendGlobalMessage(int senderId, string content)
        {
            return _apiClient.SendGlobalMessageAsync(senderId, content).GetAwaiter().GetResult();
        }
    }
}
