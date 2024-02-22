using System.ComponentModel.DataAnnotations;

namespace Applogiq.IdentityServer.DTOs.Auth
{
    public class TwoFactorDto
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Provider { get; set; }

        [Required]
        public required string Token { get; set; }
    }
}