namespace NimbusChat.Api.Models
{
    // Request payload for both login and register - Username is only
    // required (and used) for registration.
    public class LoginDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? Username { get; set; }
    }
}
