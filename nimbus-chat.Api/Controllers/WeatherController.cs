using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NimbusChat.Api.Models;

namespace NimbusChat.Api.Controllers
{
    // Weather endpoints: proxies OpenWeatherMap so the API key never reaches
    // the client, and persists every current-weather lookup to WeatherData.
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        // Fetch AND store the current weather. Every lookup inserts a row into
        // WeatherData, so the full history ends up in the DB.
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string city, [FromQuery] int userId = 0)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City is required.");

            JsonDocument? document;
            try
            {
                document = await FetchOpenWeatherAsync("weather", city);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, $"Weather provider error: {ex.Message}");
            }

            if (document == null)
                return NotFound($"Unknown city '{city}'.");

            using (document)
            {
                var root = document.RootElement;
                var main = root.GetProperty("main");
                var weather = root.GetProperty("weather")[0];

                var dto = new WeatherDto
                {
                    UserId = userId > 0 ? userId : null,
                    City = root.GetProperty("name").GetString() ?? city,
                    Temperature = main.GetProperty("temp").GetDouble(),
                    FeelsLike = main.GetProperty("feels_like").GetDouble(),
                    Humidity = main.GetProperty("humidity").GetInt32(),
                    WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                    Description = weather.GetProperty("description").GetString() ?? string.Empty,
                    Icon = weather.GetProperty("icon").GetString() ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                dto.Id = Insert(dto);

                return Ok(dto);
            }
        }

        // 5-day forecast. Deliberately not stored, just passed through, so the
        // OpenWeatherMap key stays server-side.
        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City is required.");

            JsonDocument? document;
            try
            {
                document = await FetchOpenWeatherAsync("forecast", city);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, $"Weather provider error: {ex.Message}");
            }

            if (document == null)
                return NotFound($"Unknown city '{city}'.");

            using (document)
            {
                var today = DateTime.Now.Date;
                var entries = new List<(DateTime Time, double Temp, string Icon, string Condition)>();

                foreach (var item in document.RootElement.GetProperty("list").EnumerateArray())
                {
                    var time = DateTime.Parse(item.GetProperty("dt_txt").GetString()!);

                    // Only future days, not today.
                    if (time.Date <= today)
                        continue;

                    var weather = item.GetProperty("weather")[0];

                    entries.Add((
                        time,
                        item.GetProperty("main").GetProperty("temp").GetDouble(),
                        weather.GetProperty("icon").GetString() ?? string.Empty,
                        weather.GetProperty("description").GetString() ?? string.Empty));
                }

                var days = entries
                    .GroupBy(e => e.Time.Date)
                    .OrderBy(g => g.Key)
                    .Take(5);

                var result = new List<ForecastDayDto>();

                foreach (var day in days)
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

                return Ok(result);
            }
        }

        // Stored history, optionally filtered by city.
        [HttpGet("history")]
        public IActionResult GetHistory([FromQuery] string? city = null, [FromQuery] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 20;

            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
SELECT Id, UserId, City, Temperature, FeelsLike, Humidity, WindSpeed, Description, Icon, CreatedAt
FROM WeatherData
WHERE (@City IS NULL OR City = @City)
ORDER BY Id DESC
LIMIT @Limit";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@City", string.IsNullOrWhiteSpace(city) ? DBNull.Value : city);
            command.Parameters.AddWithValue("@Limit", limit);

            var result = new List<WeatherDto>();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new WeatherDto
                {
                    Id = reader.GetInt32("Id"),
                    UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? null : reader.GetInt32("UserId"),
                    City = reader.GetString("City"),
                    Temperature = reader.GetDouble("Temperature"),
                    FeelsLike = reader.IsDBNull(reader.GetOrdinal("FeelsLike")) ? 0 : reader.GetDouble("FeelsLike"),
                    Humidity = reader.GetInt32("Humidity"),
                    WindSpeed = reader.IsDBNull(reader.GetOrdinal("WindSpeed")) ? 0 : reader.GetDouble("WindSpeed"),
                    Description = reader.GetString("Description"),
                    Icon = reader.IsDBNull(reader.GetOrdinal("Icon")) ? string.Empty : reader.GetString("Icon"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }

            return Ok(result);
        }

        private int Insert(WeatherDto dto)
        {
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string sql = @"
INSERT INTO WeatherData (UserId, City, Temperature, FeelsLike, Humidity, WindSpeed, Description, Icon, CreatedAt)
VALUES (@UserId, @City, @Temperature, @FeelsLike, @Humidity, @WindSpeed, @Description, @Icon, @CreatedAt);
SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", (object?)dto.UserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@City", dto.City);
            command.Parameters.AddWithValue("@Temperature", dto.Temperature);
            command.Parameters.AddWithValue("@FeelsLike", dto.FeelsLike);
            command.Parameters.AddWithValue("@Humidity", dto.Humidity);
            command.Parameters.AddWithValue("@WindSpeed", dto.WindSpeed);
            command.Parameters.AddWithValue("@Description", dto.Description);
            command.Parameters.AddWithValue("@Icon", dto.Icon);
            command.Parameters.AddWithValue("@CreatedAt", dto.CreatedAt);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        // Returns null when OpenWeatherMap doesn't recognize the city (404).
        private async Task<JsonDocument?> FetchOpenWeatherAsync(string endpoint, string city)
        {
            var apiKey = _configuration["OpenWeather:ApiKey"];
            var baseUrl = _configuration["OpenWeather:BaseUrl"];

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("OpenWeather configuration is missing.");

            var url = $"{baseUrl}{endpoint}?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json);
        }
    }
}
