namespace NimbusChat.Api.Models
{
    // Request/response shape for UsersController, plus the smaller payload
    // used when only the online/away/busy status is being updated.
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string FavoriteCity { get; set; } = default!;
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; } = default!;
    }
}
