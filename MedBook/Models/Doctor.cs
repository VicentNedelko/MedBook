using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedBook.Models
{
    [Table("Doctors")]
    public class Doctor : User
    {
        public string FName { get; set; }

        public string LName { get; set; }


        public List<Patient> Patients { get; set; }
        public List<Visit> Visits { get; set; }
    }
}
