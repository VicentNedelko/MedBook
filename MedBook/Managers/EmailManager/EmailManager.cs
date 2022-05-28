using EmailService;
using EmailService.Interfaces;
using System.Threading.Tasks;

namespace MedBook.Managers.EmailManager
{
    public class EmailManager : IEmailManager
    {
        private readonly IEmailService _emailService;
        public EmailManager(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendAsync(EmailMessage message)
        {
            await _emailService.SendAsync(message);
        }

        public async Task SendEmailConfirmationLinkAsync(string confirmationLink, string email)
        {
            var message = new EmailMessage();
            message.ToAddresses.Add(new EmailAddress { Address = email });
            message.Content = $"Для подтверждения регистрации перейдите по <a href={confirmationLink}>ссылке<a>";
            message.Subject = "MedBook registration confirmation request";
            await _emailService.SendAsync(message);
        }
    }
}
