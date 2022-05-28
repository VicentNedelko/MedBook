using EmailService;
using System.Threading.Tasks;

namespace MedBook.Managers.EmailManager
{
    public interface IEmailManager
    {
        public Task SendEmailConfirmationLinkAsync(string confirmationLink, string email);

        public Task SendAsync(EmailMessage message);
    }
}
