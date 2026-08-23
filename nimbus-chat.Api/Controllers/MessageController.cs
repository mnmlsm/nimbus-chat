using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NimbusChat.Api.Messaging;
using NimbusChat.Api.Models;

namespace NimbusChat.Api.Controllers
{
    // Chat endpoints for both the global chat (backed by a hidden system
    // user) and private 1:1 conversations between two users.
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private const string GlobalChatEmail = "global@nimbuschat.local";

        private readonly IConfiguration _configuration;

        public MessagesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Global chat history; ?since=<lastId> returns only newer messages (polling).
        [HttpGet]
        public IActionResult Get([FromQuery] int since = 0)
        {
            var messages = new List<GlobalMessageDto>();
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var globalChatUserId = GetOrCreateGlobalChatUserId(connection);

            var sql = @"
SELECT m.Id, m.SenderId, m.Content, m.CreatedAt, u.Username
FROM Messages m
JOIN Users u ON u.Id = m.SenderId
WHERE m.Id > @Since AND m.ReceiverId = @GlobalChatUserId
ORDER BY m.Id ASC";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Since", since);
            command.Parameters.AddWithValue("@GlobalChatUserId", globalChatUserId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                messages.Add(new GlobalMessageDto
                {
                    Id = reader.GetInt32("Id"),
                    SenderId = reader.GetInt32("SenderId"),
                    Content = reader.GetString("Content"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    SenderName = reader.GetString("Username")
                });
            }

            return Ok(messages);
        }

        // Private chat history between two users.
        [HttpGet("between")]
        public IActionResult GetBetween([FromQuery] int user1, [FromQuery] int user2)
        {
            var messages = new List<MessageDto>();
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
SELECT Id, SenderId, ReceiverId, Content, CreatedAt
FROM Messages
WHERE (SenderId = @User1 AND ReceiverId = @User2)
   OR (SenderId = @User2 AND ReceiverId = @User1)
ORDER BY Id ASC";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@User1", user1);
            command.Parameters.AddWithValue("@User2", user2);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                messages.Add(new MessageDto
                {
                    Id = reader.GetInt32("Id"),
                    SenderId = reader.GetInt32("SenderId"),
                    ReceiverId = reader.GetInt32("ReceiverId"),
                    Content = reader.GetString("Content"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }

            return Ok(messages);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateMessageDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
            {
                return BadRequest("Content is required.");
            }

            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var receiverId = MessageRouting.IsGlobal(dto.ReceiverId)
                ? GetOrCreateGlobalChatUserId(connection)
                : dto.ReceiverId!.Value;

            var sql = @"INSERT INTO Messages (SenderId, ReceiverId, Content, CreatedAt)
                        VALUES (@SenderId, @ReceiverId, @Content, @CreatedAt);";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@SenderId", dto.SenderId);
            command.Parameters.AddWithValue("@ReceiverId", receiverId);
            command.Parameters.AddWithValue("@Content", dto.Content);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            command.ExecuteNonQuery();

            return Ok("Message created.");
        }

        private static int GetOrCreateGlobalChatUserId(MySqlConnection connection)
        {
            const string selectSql = "SELECT Id FROM Users WHERE Email = @Email LIMIT 1;";
            using (var select = new MySqlCommand(selectSql, connection))
            {
                select.Parameters.AddWithValue("@Email", GlobalChatEmail);
                var existing = select.ExecuteScalar();
                if (existing != null)
                    return Convert.ToInt32(existing);
            }

            const string insertSql = @"
INSERT INTO Users (Username, Email, PasswordHash, Status)
VALUES ('GlobalChat', @Email, 'GLOBAL_CHAT_SYSTEM_USER', 'System');
SELECT LAST_INSERT_ID();";

            using var insert = new MySqlCommand(insertSql, connection);
            insert.Parameters.AddWithValue("@Email", GlobalChatEmail);
            return Convert.ToInt32(insert.ExecuteScalar());
        }
    }
}
