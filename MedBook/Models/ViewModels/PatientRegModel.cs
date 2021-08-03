using MedBook.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class PatientRegModel
    {
        [Required]
        [Display(Name = "First name : ")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "Last name : ")]
        public string LName { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Age is out of valid range")]
        [Display(Name = "Age : ")]
        public int Age { get; set; }

        [Required]
        [Display(Name = "Email : ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password : ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public string Gender { get; set; }
        
    }
}
