using System;
using System.Collections.Generic;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;

namespace NimbusChat.WetterChatApp.Services
{
    public class MessageService
    {
        private readonly UserRepository _userRepository;
        private readonly MessageRepository _messageRepository;

        public MessageService(UserRepository userRepository, MessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        // Alle globalen Nachrichten (aktuell: alle Messages)
        public List<(Message Message, string DisplayName)> GetAllGlobalMessages()
        {
            return _messageRepository.GetAllWithUser();
        }

        // Neue globale Nachrichten seit einer bestimmten Id
        public List<(Message Message, string DisplayName)> GetNewGlobalMessagesSince(int lastMessageId)
        {
            return _messageRepository.GetNewGlobalSince(lastMessageId);
        }

        // Global-Nachricht senden mit gültigen Foreign Keys
        public bool SendGlobalMessage(int senderId, string content)
        {
            var sender = _userRepository.GetById(senderId);
            if (sender == null)
            {
                return false;
            }

            var globalUser = GetOrCreateGlobalChatUser();
            if (globalUser == null)
            {
                return false;
            }

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = globalUser.Id,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            return _messageRepository.Create(message);
        }

        private User GetOrCreateGlobalChatUser()
        {
            var existing = _userRepository.GetByEmail("global@nimbuschat.local");
            if (existing != null)
            {
                return existing;
            }

            var globalUser = new User
            {
                Username = "GlobalChat",
                Email = "global@nimbuschat.local",
                PasswordHash = "GLOBAL_CHAT_SYSTEM_USER",
                Status = "System",
                FavoriteCity = null
            };

            var created = _userRepository.Create(globalUser);
            if (!created)
            {
                return null;
            }

            return _userRepository.GetByEmail("global@nimbuschat.local");
        }
    }
}