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
        /// 1:1-Nachricht (wird für Global-Chat mit ReceiverId = 0 verwendet).
        /// </summary>
        public bool SendMessage(int senderId, int receiverId, string content)
        {
            if (senderId <= 0 || receiverId < 0)
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
        /// Globaler Chat: Nachricht an alle (ReceiverId = 0).
        /// </summary>
        public bool SendGlobalMessage(int senderId, string content)
        {
            // ReceiverId = 0 kennzeichnet globalen Chat
            return SendMessage(senderId, 0, content);
        }

        /// <summary>
        /// Globaler Chat: alle Nachrichten mit Username/Email.
        /// </summary>
        public List<(Message Message, string DisplayName)> GetAllGlobalMessages()
        {
            return _messageRepository.GetAllWithUser();
        }

        /// <summary>
        /// Globaler Chat: neue Nachrichten seit einer bestimmten Id.
        /// </summary>
        public List<(Message Message, string DisplayName)> GetNewGlobalMessagesSince(int lastMessageId)
        {
            if (lastMessageId < 0)
                lastMessageId = 0;

            return _messageRepository.GetNewGlobalSince(lastMessageId);
        }
    }
}