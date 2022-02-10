using MedBook.Models.ViewModels;
using MedBook.Services.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Services.Registration
{
    public interface IUserRegistration
    {
        public Task<RegistrationResult> ReceptionistRegistrationAsync(ReceptionistRegModel model);
    }
}
