namespace Applogiq.IdentityServer.DTOs.Auth
{
    public class ExternalAuthDto
    {
        public required string Provider { get; set; }
        public required string IdToken { get; set; }
    }
}