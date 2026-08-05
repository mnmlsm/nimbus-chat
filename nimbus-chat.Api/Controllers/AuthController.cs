using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NimbusChat.Api.Models;

namespace NimbusChat.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password are required.");

            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string sql = @"SELECT Id, Username, Email, PasswordHash, Status, FavoriteCity FROM Users WHERE Email = @Email LIMIT 1;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", dto.Email);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return Unauthorized("Invalid email or password.");

            var storedHash = reader.GetString("PasswordHash");
            if (!string.Equals(storedHash, HashPassword(dto.Password), StringComparison.Ordinal))
                return Unauthorized("Invalid email or password.");

            return Ok(new UserDto
            {
                Id = reader.GetInt32("Id"),
                Username = reader.GetString("Username"),
                Email = reader.GetString("Email"),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString("Status"),
                FavoriteCity = reader.IsDBNull(reader.GetOrdinal("FavoriteCity")) ? string.Empty : reader.GetString("FavoriteCity")
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password are required.");

            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string sql = @"INSERT INTO Users (Username, Email, PasswordHash, Status) VALUES (@Username, @Email, @PasswordHash, 'Online');";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Username", string.IsNullOrWhiteSpace(dto.Username) ? dto.Email : dto.Username);
            command.Parameters.AddWithValue("@Email", dto.Email);
            command.Parameters.AddWithValue("@PasswordHash", HashPassword(dto.Password));

            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                return Conflict("User already exists.");
            }

            return Ok();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);

            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
