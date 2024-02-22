using Applogiq.EmailService.Model;

namespace Applogiq.EmailService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}