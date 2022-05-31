using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
