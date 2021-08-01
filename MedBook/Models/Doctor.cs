using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class Doctor : User
    {

        [MaxLength(20)]
        [Required]
        public string FName { get; set; }

        [MaxLength(20)]
        [Required]
        public string LName { get; set; }


        public List<Patient> Patients { get; set; }
    }
}
