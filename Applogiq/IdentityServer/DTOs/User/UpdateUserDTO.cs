namespace Applogiq.IdentityServer.DTOs.User
{
    public class UpdateUserDTO
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required IEnumerable<string> Roles { get; set; }
    }
}