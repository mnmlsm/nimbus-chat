using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusChat.WetterChatApp.Models
{
    public class WeatherData
    {
        public int Id
        {
            get; set;
        }

        public string City
        {
            get; set;
        }

        public double Temperature
        {
            get; set;
        }

        public int Humidity
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        }
    }
}