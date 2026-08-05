using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusChat.WetterChatApp.Models
{
    public class ChatMessage
    {
        public string Content { get; set; }

        public bool IsMine { get; set; }

        public string Time { get; set; }
    }
}
