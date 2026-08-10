using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NimbusChat.Api.Models;

namespace NimbusChat.Api.Controllers
{
    // Login and registration endpoints: hashes/verifies passwords and issues
    // no session token - the client just holds onto the returned user object.
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

            int id;
            string username, email, favoriteCity;

            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                    return Unauthorized("Invalid email or password.");

                var storedHash = reader.GetString("PasswordHash");
                if (!string.Equals(storedHash, HashPassword(dto.Password), StringComparison.Ordinal))
                    return Unauthorized("Invalid email or password.");

                id = reader.GetInt32("Id");
                username = reader.GetString("Username");
                email = reader.GetString("Email");
                favoriteCity = reader.IsDBNull(reader.GetOrdinal("FavoriteCity")) ? string.Empty : reader.GetString("FavoriteCity");
            }

            // Every successful login resets the status to Online, regardless of
            // its previous value (e.g. Busy/Away/Offline left over from the last session).
            const string updateSql = "UPDATE Users SET Status = 'Online' WHERE Id = @Id;";
            using var updateCommand = new MySqlCommand(updateSql, connection);
            updateCommand.Parameters.AddWithValue("@Id", id);
            updateCommand.ExecuteNonQuery();

            return Ok(new UserDto
            {
                Id = id,
                Username = username,
                Email = email,
                Status = "Online",
                FavoriteCity = favoriteCity
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
