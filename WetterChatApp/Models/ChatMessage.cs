using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusChat.WetterChatApp.Models
{
    // UI-facing shape of a single chat bubble, built from a Message plus
    // whatever extra display info the chat list needs.
    public class ChatMessage
    {
        public string Content { get; set; }

        public bool IsMine { get; set; }

        public string Time { get; set; }

        // Only set in Global Chat (multiple senders), empty for private 1:1 chats.
        public string SenderName { get; set; }
    }
}
