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
        } = 0;

        public string City
        {
            get; set;
        } = string.Empty;

        public double Temperature
        {
            get; set;
        } = 0.0;

        public int Humidity
        {
            get; set;
        } = 0;

        public string Description
        {
            get; set;
        } = string.Empty;

        public DateTime CreatedAt
        {
            get; set;
        } 
    }
}