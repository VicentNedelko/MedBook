using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class DoctorRegModel
    {
        [Display(Name = "First Name : ")]
        [Required(ErrorMessage = "First Name is mandatory")]
        [MaxLength(20)]
        public string FName { get; set; }
        [Display(Name = "Last Name : ")]
        [Required(ErrorMessage = "Last Name is mandatory")]
        [MaxLength(20)]
        public string LName { get; set; }

        [Display(Name = "Email : ")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is mandatory")]
        public string Email { get; set; }

        [Display(Name = "Password : ")]
        [Required(ErrorMessage = "Password is mandatory")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password : ")]
        [Required(ErrorMessage = "Enter Password confirmation")]
        [Compare("Password", ErrorMessage = "Pass do not fit.")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
