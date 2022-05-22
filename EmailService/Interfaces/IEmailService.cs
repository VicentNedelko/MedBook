using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage emailMessage);
    }
}
