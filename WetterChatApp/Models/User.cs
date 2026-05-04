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

        public string Username
        {
            get; set;
        }

        public string PasswordHash
        {
            get; set;
        }

    }
}