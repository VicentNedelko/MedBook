using System.Collections.Generic;

namespace MedBook.Models
{
    public class Doctor
    {
        public string Id { get; set; }

        public string FName { get; set; }

        public string LName { get; set; }


        public List<Patient> Patients { get; set; }
        public List<Visit> Visits { get; set; }
    }
}
