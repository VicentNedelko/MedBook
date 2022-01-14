﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<Cure> Cures { get; set; }

        // Visit FK
        public int VisitId { get; set; }
        public Visit Visit { get; set; }
    }
}
