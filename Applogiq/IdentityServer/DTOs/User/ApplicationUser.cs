using Microsoft.AspNetCore.Identity;

namespace Applogiq.IdentityServer.DTOs.User
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool Enabled { get; set; }
    }
}
