using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class DoctorRegModel
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is mandatory")]
        public string FName { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is mandatory")]
        public string LName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is mandatory")]
        public string Email { get; set; }
    }
}
