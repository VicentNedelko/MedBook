using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class ReceptionistRegModel
    {
        [Required]
        [Display(Name = "Имя : ")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "Фамилия : ")]
        public string LName { get; set; }

        [Required]
        [Display(Name = "Email : ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Пароль : ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Подтверждение : ")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
