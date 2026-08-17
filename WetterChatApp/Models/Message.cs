using System;

namespace NimbusChat.WetterChatApp.Models
{
    // Client-side shape of a chat message, covering both global and private
    // conversations.
    public class Message
    {
        public int Id { get; set; } = 0;

        public int SenderId { get; set; } = 0;

        // Unused for global chat messages (can be 0).
        public int ReceiverId { get; set; } = 0;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}