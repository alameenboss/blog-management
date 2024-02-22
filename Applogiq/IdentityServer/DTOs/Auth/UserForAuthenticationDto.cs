using System.ComponentModel.DataAnnotations;

namespace Applogiq.IdentityServer.DTOs.Auth
{
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }

        public required string ClientURI { get; set; }
    }
}