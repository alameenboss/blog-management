using System.ComponentModel.DataAnnotations;

namespace Applogiq.IdentityServer.DTOs.Auth
{
    public class UserForRegistrationDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }

        public required string ClientURI { get; set; }
    }
}