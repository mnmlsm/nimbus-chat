namespace NimbusChat.Api.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string FavoriteCity { get; set; } = default!;
    }
}   