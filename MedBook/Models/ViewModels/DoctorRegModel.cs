using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class DoctorRegModel
    {
        [Display(Name = "Имя ")]
        [MaxLength(20)]
        public string FName { get; set; }
        [Display(Name = "Фамилия ")]
        [MaxLength(20)]
        public string LName { get; set; }

        [Display(Name = "Email ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Пароль ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Подтверждение пароля ")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
