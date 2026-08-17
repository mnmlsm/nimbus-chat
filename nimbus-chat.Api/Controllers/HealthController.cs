using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using MySql.Data.MySqlClient; // MySQL ADO.NET Provider

namespace NimbusChat.Api.Controllers
{
    // Liveness/health endpoint: confirms the API can actually open a
    // connection to the database, not just that the process is running.
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