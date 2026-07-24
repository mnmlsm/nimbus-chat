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

            var sql = @"SELECT Id, Username, Email, Status, FavoriteCity FROM Users";

            using var command = new MySqlCommand(sql, connection);
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
    }
}