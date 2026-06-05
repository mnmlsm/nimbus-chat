using System;

namespace NimbusChat.WetterChatApp.Models
{
    public class Message
    {
        public int Id { get; set; } = 0;

        public int SenderId { get; set; } = 0;

        public int ReceiverId { get; set; } = 0;

        public string Content { get; set; } = string.Empty;

        // Wir speichern DateTime in C#, in SQLite liegt es als TEXT "yyyy-MM-dd HH:mm:ss"
        public DateTime CreatedAt { get; set; }
    }
}