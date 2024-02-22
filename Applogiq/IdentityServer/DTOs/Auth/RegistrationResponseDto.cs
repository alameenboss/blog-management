namespace Applogiq.IdentityServer.DTOs.Auth
{
    public class RegistrationResponseDto
    {
        public bool IsSuccessfulRegistration { get; set; }
        public required IEnumerable<string> Errors { get; set; }
    }
}