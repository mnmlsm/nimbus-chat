using System.Text.Json;
using NimbusChat.Api.Weather;
using Xunit;

namespace NimbusChat.Api.Tests
{
    public class OpenWeatherMapperTests
    {
        private const string CurrentWeatherJson = """
        {
          "name": "Hamburg",
          "main": { "temp": 18.4, "feels_like": 17.9, "humidity": 72 },
          "wind": { "speed": 4.6 },
          "weather": [ { "description": "leichter Regen", "icon": "10d" } ]
        }
        """;

        private static JsonElement Parse(string json) => JsonDocument.Parse(json).RootElement.Clone();

        private static string ForecastJson(params (string DtTxt, double Temp, string Icon, string Condition)[] entries)
        {
            var items = entries.Select(e => $$"""
                {
                  "dt_txt": "{{e.DtTxt}}",
                  "main": { "temp": {{e.Temp.ToString(System.Globalization.CultureInfo.InvariantCulture)}} },
                  "weather": [ { "icon": "{{e.Icon}}", "description": "{{e.Condition}}" } ]
                }
                """);

            return $$"""{ "list": [ {{string.Join(",", items)}} ] }""";
        }

        // --- MapCurrent ---

        [Fact]
        public void MapCurrent_MapsEveryField()
        {
            var createdAt = new DateTime(2026, 8, 20, 10, 30, 0, DateTimeKind.Utc);

            var dto = OpenWeatherMapper.MapCurrent(Parse(CurrentWeatherJson), "Hamburg", 7, createdAt);

            Assert.Equal("Hamburg", dto.City);
            Assert.Equal(18.4, dto.Temperature);
            Assert.Equal(17.9, dto.FeelsLike);
            Assert.Equal(72, dto.Humidity);
            Assert.Equal(4.6, dto.WindSpeed);
            Assert.Equal("leichter Regen", dto.Description);
            Assert.Equal("10d", dto.Icon);
            Assert.Equal(createdAt, dto.CreatedAt);
            Assert.Equal(7, dto.UserId);
        }

        // Anonymous lookups must not be attributed to a user, otherwise the
        // WeatherData foreign key points at a user that never searched.
        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void MapCurrent_TreatsNonPositiveUserIdAsAnonymous(int userId)
        {
            var dto = OpenWeatherMapper.MapCurrent(Parse(CurrentWeatherJson), "Hamburg", userId, DateTime.UtcNow);

            Assert.Null(dto.UserId);
        }

        [Fact]
        public void MapCurrent_FallsBackToRequestedCity_WhenProviderOmitsTheName()
        {
            var json = CurrentWeatherJson.Replace("\"Hamburg\"", "null");

            var dto = OpenWeatherMapper.MapCurrent(Parse(json), "Bremen", null, DateTime.UtcNow);

            Assert.Equal("Bremen", dto.City);
        }

        // --- MapForecast ---

        [Fact]
        public void MapForecast_SkipsTheCurrentDay()
        {
            var json = ForecastJson(
                ("2026-08-20 15:00:00", 20.0, "01d", "klar"),
                ("2026-08-21 12:00:00", 22.0, "02d", "leicht bewoelkt"));

            var days = OpenWeatherMapper.MapForecast(Parse(json), new DateTime(2026, 8, 20));

            Assert.Single(days);
            Assert.Equal(new DateTime(2026, 8, 21), days[0].Date);
        }

        [Fact]
        public void MapForecast_PicksTheEntryClosestToNoon()
        {
            var json = ForecastJson(
                ("2026-08-21 03:00:00", 11.0, "01n", "nachts"),
                ("2026-08-21 12:00:00", 23.0, "01d", "mittags"),
                ("2026-08-21 21:00:00", 15.0, "01n", "abends"));

            var days = OpenWeatherMapper.MapForecast(Parse(json), new DateTime(2026, 8, 20));

            Assert.Single(days);
            Assert.Equal(23.0, days[0].Temperature);
            Assert.Equal("mittags", days[0].Condition);
            Assert.Equal("01d", days[0].Icon);
        }

        [Fact]
        public void MapForecast_PicksNearestEntry_WhenNoonIsMissing()
        {
            var json = ForecastJson(
                ("2026-08-21 09:00:00", 17.0, "02d", "vormittags"),
                ("2026-08-21 18:00:00", 19.0, "03d", "abends"));

            var days = OpenWeatherMapper.MapForecast(Parse(json), new DateTime(2026, 8, 20));

            Assert.Equal("vormittags", days[0].Condition);
        }

        [Fact]
        public void MapForecast_ReturnsAtMostFiveDaysInAscendingOrder()
        {
            var entries = Enumerable.Range(1, 7)
                .Select(offset => ($"2026-08-{20 + offset:00} 12:00:00", 20.0 + offset, "01d", $"tag-{offset}"))
                .ToArray();

            var days = OpenWeatherMapper.MapForecast(Parse(ForecastJson(entries)), new DateTime(2026, 8, 20));

            Assert.Equal(5, days.Count);
            Assert.Equal(new DateTime(2026, 8, 21), days[0].Date);
            Assert.Equal(new DateTime(2026, 8, 25), days[4].Date);
            Assert.Equal(days.OrderBy(d => d.Date).Select(d => d.Date), days.Select(d => d.Date));
        }

        [Fact]
        public void MapForecast_ReturnsEmptyList_WhenOnlyTodayIsAvailable()
        {
            var json = ForecastJson(("2026-08-20 12:00:00", 20.0, "01d", "heute"));

            Assert.Empty(OpenWeatherMapper.MapForecast(Parse(json), new DateTime(2026, 8, 20)));
        }

        // The controller passes DateTime.Now (with a time component), so the
        // "skip today" rule has to compare dates, not timestamps.
        [Fact]
        public void MapForecast_IgnoresTheTimeComponentOfToday()
        {
            var json = ForecastJson(("2026-08-21 12:00:00", 22.0, "01d", "morgen"));

            var days = OpenWeatherMapper.MapForecast(Parse(json), new DateTime(2026, 8, 20, 23, 59, 0));

            Assert.Single(days);
        }
    }
}
