using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using MySql.Data.MySqlClient; // MySQL ADO.NET Provider

namespace NimbusChat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var connectionString = _configuration.GetConnectionString("NimbusChatDatabase");

            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();
                return Ok("DB OK");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"DB ERROR: {ex.Message}");
            }
        }
    }
}