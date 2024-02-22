namespace Applogiq.EmailService.Config
{
    public class EmailConfiguration
    {
        public required EmailAddress From { get; set; }
        public required string SmtpServer { get; set; }
        public int Port { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }

    public class EmailAddress
    {
        public EmailAddress(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}