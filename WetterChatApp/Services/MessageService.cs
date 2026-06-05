using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Runtime.Remoting.Messaging;

namespace NimbusChat.WetterChatApp.Services
{
    public class MessageService
    {

        private readonly MessageRepository _messageRepository;

        public MessageService()
        {
            _messageRepository = new MessageRepository();
        }

        public bool SendMessage(int senderId, int receiverId, string content)
        {
            Console.WriteLine($"[MessageService.SendMessage] sender={senderId}, receiver={receiverId}, content='{content}'");

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

            var success = _messageRepository.Create(message);
            Console.WriteLine($"[MessageService.SendMessage] success={success}");
            return success;
        }

        public List<Message> GetMessagesBetween(int userId1, int userId2)
        {
            if (userId1 <= 0 || userId2 <= 0)
                return new List<Message>();

            return _messageRepository.GetMessagesBetween(userId1, userId2);
        }

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