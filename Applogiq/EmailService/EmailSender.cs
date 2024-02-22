using Applogiq.EmailService.Config;
using Applogiq.EmailService.Model;
using MailKit.Net.Smtp;
using MimeKit;

namespace Applogiq.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendEmailAsync(Message message)
        {
            MimeMessage mailMessage = ConstructEmailMessage(message);

            await SendAsync(mailMessage);
        }

        private MimeMessage ConstructEmailMessage(Message message)
        {
            MimeMessage emailMessage = new();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From.Name, _emailConfig.From.Email));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            BodyBuilder bodyBuilder = new() { HtmlBody = message.Content };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (IFormFile attachment in message.Attachments)
                {
                    using (MemoryStream ms = new())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }

                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }


        private async Task SendAsync(MimeMessage mailMessage)
        {
            using SmtpClient client = new();
            try
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, MailKit.Security.SecureSocketOptions.None);
                //client.AuthenticationMechanisms.Remove("XOAUTH2");
                //await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                await client.SendAsync(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception, or both.
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}