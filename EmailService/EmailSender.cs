using EmailService.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailSender : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;
        public EmailSender(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public async Task SendAsync(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.Add(new MailboxAddress("MedBook portal", _emailConfiguration.From));
            message.Subject = emailMessage.Subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content
            };
            using var smtpClient = new SmtpClient();
            try
            {
                smtpClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                smtpClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                await smtpClient.SendAsync(message);
                smtpClient.Disconnect(true);
            }
            catch (Exception e)
            {
                throw new WrongEmailException($"Wrong Email : {emailMessage.ToAddresses.First()}. Please, check and try again. Message - {e.Message}");
            }
        }
    }
}
