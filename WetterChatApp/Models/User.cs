using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NimbusChat.WetterChatApp.Models
{
    public class User
    {
        public Brush AvatarBrush
        {
            get
            {
                string[] colors =
                {
                    "#42A5F5", // Blue
                    "#66BB6A", // Green
                    "#AB47BC", // Purple
                    "#FF7043", // Orange
                    "#26C6DA", // Cyan
                    "#EC407A", // Pink
                    "#FFA726", // Amber
                    "#7E57C2"  // Deep Purple
                };

                int index = Math.Abs(Username.GetHashCode()) % colors.Length;

                return (Brush)new BrushConverter().ConvertFromString(colors[index]);
            }
        }

        public string Initial
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Username))
                    return "?";

                return Username.Substring(0, 1).ToUpper();
            }
        }

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