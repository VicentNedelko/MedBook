using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class Research
    {
        public int Id { get; set; }
        public string Order { get; set; } // Laboratory Name
        public DateTime ResearchDate { get; set; }

        // Patient FK
        public Patient Patient { get; set; }

        public List<Indicator> Indicators { get; set; }
    }
}
