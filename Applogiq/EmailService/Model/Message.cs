using Applogiq.EmailService.Config;
using MimeKit;

namespace Applogiq.EmailService.Model
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public IFormFileCollection Attachments { get; set; }

        public Message(IEnumerable<EmailAddress> to, string subject, string content, IFormFileCollection attachments)
        {
            To = new List<MailboxAddress>();
            To = to.Select(x => new MailboxAddress(x.Name, x.Email)).ToList();
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }

        public Message(EmailAddress to, string subject, string content, IFormFileCollection attachments)
        {
            To = new List<MailboxAddress>() { new MailboxAddress(to.Name, to.Email) };
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }
    }
}