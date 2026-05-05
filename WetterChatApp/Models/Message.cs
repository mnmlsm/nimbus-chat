using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusChat.WetterChatApp.Models
{
    public class Message
    {
        public int Id
        {
            get; set;
        } = 0;

        public int SenderId
        {
            get; set;
        }  = 0;

        public int ReceiverId
        {
            get; set;
        } = 0;   

        public string Content
        {
            get; set;
        } = string.Empty;


        public DateTime CreatedAt
        {
            get; set;
        } 
    }
}