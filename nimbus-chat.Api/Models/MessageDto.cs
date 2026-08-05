using System;

namespace NimbusChat.Api.Models
{
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