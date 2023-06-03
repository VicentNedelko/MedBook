using EmailService;
using EmailService.Interfaces;
using MedBook.Models;
using MedBook.Models.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Managers.EmailManager
{
    public class EmailManager : IEmailManager
    {
        private readonly IEmailService _emailService;
        private readonly MedBookDbContext _medBookDbContext;
        public EmailManager(IEmailService emailService, MedBookDbContext medBookDbContext)
        {
            _emailService = emailService;
            _medBookDbContext = medBookDbContext;
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
            message.Subject = "MedBook: запрос подтверждения регистрации";
            await _emailService.SendAsync(message);
        }

        public async Task<EmailStatus> SendNotificationToDoctorAsync(Patient patient, string link)
        {
            var assosiatedDoctor = _medBookDbContext.Doctors.FirstOrDefault(x => x.Patients.Select(x => x.Id).Contains(patient.Id));
            if (assosiatedDoctor != null)
            {
                var message = new EmailMessage();
                message.ToAddresses.Add(new EmailAddress { Address = assosiatedDoctor.Email });
                message.Content = $"Пациент {patient.FName} {patient.LName} загрузил новые данные исследований\n" +
                    $"<a href={link}>Войти на MedBook</a>";
                message.Subject = $"Уведомление портала MedBook для пациента {patient.FName} {patient.LName}";
                try
                {
                    await _emailService.SendAsync(message);
                    return EmailStatus.SEND;
                }
                catch (WrongEmailException)
                {
                    return EmailStatus.MAILERROR;
                }
            }
            return EmailStatus.OTHERERROR;
        }
    }
}
