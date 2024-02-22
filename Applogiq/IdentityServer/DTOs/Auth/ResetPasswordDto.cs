using System.ComponentModel.DataAnnotations;

namespace Applogiq.IdentityServer.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }

        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}