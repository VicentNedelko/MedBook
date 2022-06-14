using MedBook.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedBook.Models
{
    [Table("Patients")]
    public class Patient : User
    {
        //public string Id { get; set; }

        //public string Email { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string? Diagnosis { get; set; }

        // Doctor FK
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public List<Research> Researches { get; set; }

        public List<Visit> Visits { get; set; }

        public static string GenderToStrConverter(Gender gender)
        {
            return gender switch
            {
                Gender.FEMALE => "ЖЕН",
                Gender.MALE => "МУЖ",
                _ => "UNKNOWN",
            };
        }
    }
}
