﻿using System;
using System.Collections.Generic;

namespace MedBook.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }

        public string? Comment { get; set; }

        public List<Research>? Researches { get; set; }

        public List<Prescription>? Prescriptions { get; set; }

        //Patient FK
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        //Doctor FK
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
