using MedBook.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class Patient : User
    {
        [MaxLength(20)]
        [Required]
        public string FName { get; set; }
        [MaxLength(20)]
        [Required]
        public string LName { get; set; }

        [Range(0, 100, ErrorMessage = "Age value is out of range.")]
        [Required]
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string? Diagnosis { get; set; }

        // Doctor FK
        public Doctor Doctor { get; set; }

        public List<Research> Researches { get; set; }
    }
}
