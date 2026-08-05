using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NimbusChat.Api.Models;

namespace NimbusChat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Muss mit MessagesController.GlobalChatEmail übereinstimmen: der System-User
        // fürs globale Chat-Postfach soll nicht als normaler Kontakt auftauchen,
        // die Oberfläche zeigt "Global Chat" bereits als eigenen, gepinnten Eintrag.
        private const string GlobalChatEmail = "global@nimbuschat.local";

        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = new List<UserDto>();
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"SELECT Id, Username, Email, Status, FavoriteCity FROM Users WHERE Email <> @GlobalChatEmail";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GlobalChatEmail", GlobalChatEmail);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new UserDto
                {
                    Id = reader.GetInt32("Id"),
                    Username = reader.GetString("Username"),
                    Email = reader.GetString("Email"),
                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString("Status"),
                    FavoriteCity = reader.IsDBNull(reader.GetOrdinal("FavoriteCity")) ? string.Empty : reader.GetString("FavoriteCity")
                });
            }

            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"SELECT Id, Username, Email, Status, FavoriteCity FROM Users WHERE Id = @Id LIMIT 1";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return NotFound();

            return Ok(new UserDto
            {
                Id = reader.GetInt32("Id"),
                Username = reader.GetString("Username"),
                Email = reader.GetString("Email"),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString("Status"),
                FavoriteCity = reader.IsDBNull(reader.GetOrdinal("FavoriteCity")) ? string.Empty : reader.GetString("FavoriteCity")
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UserDto dto)
        {
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"UPDATE Users SET Username = @Username, Email = @Email, Status = @Status, FavoriteCity = @FavoriteCity WHERE Id = @Id";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Username", dto.Username);
            command.Parameters.AddWithValue("@Email", dto.Email);
            command.Parameters.AddWithValue("@Status", (object)dto.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@FavoriteCity", (object)dto.FavoriteCity ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", id);

            try
            {
                var rows = command.ExecuteNonQuery();
                return rows > 0 ? Ok() : NotFound();
            }
            catch (MySqlException)
            {
                return Conflict("Username or email already in use.");
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"UPDATE Users SET Status = @Status WHERE Id = @Id";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Status", (object)dto.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();

            return Ok();
        }
    }
}