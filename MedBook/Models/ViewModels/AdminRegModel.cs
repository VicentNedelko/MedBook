using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class AdminRegModel
    {
        [Required]
        [Display(Name = "Name : ")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email : ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password : ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Password confirmation : ")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
