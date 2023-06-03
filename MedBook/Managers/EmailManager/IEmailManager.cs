using EmailService;
using MedBook.Models;
using MedBook.Models.Enums;
using System.Threading.Tasks;

namespace MedBook.Managers.EmailManager
{
    public interface IEmailManager
    {
        public Task SendEmailConfirmationLinkAsync(string confirmationLink, string email);

        public Task SendAsync(EmailMessage message);

        public Task<EmailStatus> SendNotificationToDoctorAsync(Patient patient, string link);
    }
}
