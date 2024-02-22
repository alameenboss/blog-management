namespace Applogiq.IdentityServer.DTOs.User
{
    public class CreateUserDTO
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required IEnumerable<string> Roles { get; set; }
    }
}