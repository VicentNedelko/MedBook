using MedBook.Services.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Services.Registration
{
    public class RegistrationResult
    {
        public ServiceResult Status { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
