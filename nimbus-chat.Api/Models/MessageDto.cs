using System;

namespace NimbusChat.Api.Models
{
    // Request/response shapes for MessagesController: a stored message, the
    // payload for creating one, and the version enriched with a sender name
    // for the global chat feed.
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMessageDto
    {
        public int SenderId { get; set; }
        public string Content { get; set; } = default!;

        // Optional: set for private 1:1 messages; empty/0 means the global chat.
        public int? ReceiverId { get; set; }
    }

    public class GlobalMessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string SenderName { get; set; } = default!;
    }
}