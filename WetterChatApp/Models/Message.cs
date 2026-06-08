using System;

namespace NimbusChat.WetterChatApp.Models
{
    public class Message
    {
        public int Id { get; set; } = 0;

        public int SenderId { get; set; } = 0;

        // Für globalen Chat wird ReceiverId aktuell nicht verwendet (kann 0 sein)
        public int ReceiverId { get; set; } = 0;

        public string Content { get; set; } = string.Empty;

        // In SQLite liegt CreatedAt als TEXT "yyyy-MM-dd HH:mm:ss"
        public DateTime CreatedAt { get; set; }
    }
}