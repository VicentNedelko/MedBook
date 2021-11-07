using MedBook.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class PatientEditVM
    {
        public string Id  { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string FName { get; set; }

        [Required]
        public string LName { get; set; }

        [Required]
        [Range(0,100)]
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public string DoctorId { get; set; }
    }
}
