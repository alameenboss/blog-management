namespace Applogiq.IdentityServer.DTOs.User
{
    public class UserResponseDTO : ApplicationUser
    {
        public required IEnumerable<string> Roles { get; set; }
    }
}