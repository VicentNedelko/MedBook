using System;
using System.ComponentModel.DataAnnotations;

namespace MedBook.Models.ViewModels
{
    public class PatientRegModel
    {
        [Display(Name = "Имя ")]
        public string FName { get; set; }

        [Display(Name = "Фамилия ")]
        public string LName { get; set; }

        [Range(0, 100, ErrorMessage = "Age is out of valid range")]
        [Display(Name = "Возраст ")]
        public int Age { get; set; } // remove

        [DataType(DataType.DateTime)]
        [Display(Name = "Дата рождения")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Email ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Пароль ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Подтверждение пароля ")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Пол ")]
        public string Gender { get; set; }
        
    }
}
