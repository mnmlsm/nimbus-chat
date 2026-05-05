using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusChat.WetterChatApp.Models
{
    public class User
    {
        public int Id
        {
            get; set;
        }
            = 0;

        public string Username
        {
            get; set;
        }
            = string.Empty;

        public string Email 
        { 
            get; set; 
        }
            = string.Empty;

        public string PasswordHash
        {
            get; set;
        }
            = string.Empty;

    }
}