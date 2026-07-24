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
    public class MessagesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MessagesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var messages = new List<MessageDto>();
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"SELECT Id, SenderId, ReceiverId, Content, CreatedAt FROM Messages ORDER BY CreatedAt DESC";

            using var command = new MySqlCommand(sql, connection);
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

            var sql = @"INSERT INTO Messages (SenderId, ReceiverId, Content, CreatedAt)
                        VALUES (@SenderId, @ReceiverId, @Content, @CreatedAt);";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@SenderId", dto.SenderId);
            command.Parameters.AddWithValue("@ReceiverId", dto.ReceiverId);
            command.Parameters.AddWithValue("@Content", dto.Content);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            command.ExecuteNonQuery();

            return Ok("Message created.");
        }
    }
}