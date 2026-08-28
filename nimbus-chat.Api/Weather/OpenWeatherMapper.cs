using System.Text.Json;
using NimbusChat.Api.Models;

namespace NimbusChat.Api.Weather
{
    // Translates raw OpenWeatherMap JSON into the DTOs the client consumes.
    public static class OpenWeatherMapper
    {
        public static WeatherDto MapCurrent(JsonElement root, string requestedCity, int? userId, DateTime createdAtUtc)
        {
            var main = root.GetProperty("main");
            var weather = root.GetProperty("weather")[0];

            return new WeatherDto
            {
                UserId = userId > 0 ? userId : null,
                City = root.GetProperty("name").GetString() ?? requestedCity,
                Temperature = main.GetProperty("temp").GetDouble(),
                FeelsLike = main.GetProperty("feels_like").GetDouble(),
                Humidity = main.GetProperty("humidity").GetInt32(),
                WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                Description = weather.GetProperty("description").GetString() ?? string.Empty,
                Icon = weather.GetProperty("icon").GetString() ?? string.Empty,
                CreatedAt = createdAtUtc
            };
        }

        // Condenses the 3-hour forecast list into one entry per day. `today` is
        // passed in so the "skip the current day" rule stays testable.
        public static List<ForecastDayDto> MapForecast(JsonElement root, DateTime today)
        {
            var entries = new List<(DateTime Time, double Temp, string Icon, string Condition)>();

            foreach (var item in root.GetProperty("list").EnumerateArray())
            {
                var time = DateTime.Parse(item.GetProperty("dt_txt").GetString()!);

                if (time.Date <= today.Date)
                    continue;

                var weather = item.GetProperty("weather")[0];

                entries.Add((
                    time,
                    item.GetProperty("main").GetProperty("temp").GetDouble(),
                    weather.GetProperty("icon").GetString() ?? string.Empty,
                    weather.GetProperty("description").GetString() ?? string.Empty));
            }

            var result = new List<ForecastDayDto>();

            foreach (var day in entries.GroupBy(e => e.Time.Date).OrderBy(g => g.Key).Take(5))
            {
                // Pick the entry closest to noon, so the icon/temperature
                // represent the day realistically (instead of, say, a 3am reading).
                var noon = day.Key.AddHours(12);
                var representative = day.OrderBy(e => Math.Abs((e.Time - noon).TotalMinutes)).First();

                result.Add(new ForecastDayDto
                {
                    Date = day.Key,
                    Temperature = representative.Temp,
                    Icon = representative.Icon,
                    Condition = representative.Condition
                });
            }

            return result;
        }
    }
}
