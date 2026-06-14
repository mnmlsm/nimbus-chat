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

        public string Status 
        { 
            get; set; 
        }       
        public string FavoriteCity 
        { 
            get; set; 
        }

        public override string ToString()
        {
            string icon = "⚫";

            if (Status == "Online")
                icon = "🟢";
            else if (Status == "Busy")
                icon = "🔴";
            else if (Status == "Away")
                icon = "🟡";
            else if (!string.IsNullOrWhiteSpace(Status))
                icon = "🔵";

            return $"{icon} {Username}";
        }
    }

}