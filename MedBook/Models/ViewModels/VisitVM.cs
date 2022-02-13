using MedBook.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class VisitVM
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Color { get; set; }
        public VisitStatus Status { get; set; }

        //public List<Research> Researches { get; set; }

        //public Prescription Prescription { get; set; }

        ////Patient FK
        //public string PatientId { get; set; }
        //public Patient Patient { get; set; }

        ////Doctor FK
        //public string DoctorId { get; set; }
        //public Doctor Doctor { get; set; }
    }
}
