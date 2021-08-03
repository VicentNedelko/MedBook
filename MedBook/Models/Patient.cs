using MedBook.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class Patient
    {
        public string Id { get; set; }

        public string Email { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }

        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string? Diagnosis { get; set; }

        // Doctor FK
        public Doctor Doctor { get; set; }

        public List<Research> Researches { get; set; }
    }
}
