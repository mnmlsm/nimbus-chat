using System;
using System.Collections.Generic;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;

namespace NimbusChat.WetterChatApp.Services
{
    public class MessageService
    {
        private readonly MessageRepository _messageRepository;

        public MessageService()
        {
            _messageRepository = new MessageRepository();
        }

        /// <summary>
        /// Sendet eine neue Nachricht von senderId an receiverId.
        /// </summary>
        public bool SendMessage(int senderId, int receiverId, string content)
        {
            if (senderId <= 0 || receiverId <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(content))
                return false;

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            return _messageRepository.Create(message);
        }

        /// <summary>
        /// Holt alle Nachrichten zwischen zwei Usern (Chat-Verlauf).
        /// </summary>
        public List<Message> GetMessagesBetween(int userId1, int userId2)
        {
            if (userId1 <= 0 || userId2 <= 0)
                return new List<Message>();

            return _messageRepository.GetMessagesBetween(userId1, userId2);
        }

        /// <summary>
        /// Holt alle neuen Nachrichten seit einer bestimmten Message-Id (für Polling).
        /// </summary>
        public List<Message> GetNewMessagesSince(int userId1, int userId2, int lastMessageId)
        {
            if (userId1 <= 0 || userId2 <= 0)
                return new List<Message>();

            if (lastMessageId < 0)
                lastMessageId = 0;

            return _messageRepository.GetNewMessagesSince(userId1, userId2, lastMessageId);
        }
    }
}