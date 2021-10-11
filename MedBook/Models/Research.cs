using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class Research : IEquatable<Research>
    {
        public int Id { get; set; }
        public string Order { get; set; } // Laboratory Name
        public DateTime ResearchDate { get; set; }

        // Patient FK
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public List<Indicator> Indicators { get; set; }


        public bool Equals([AllowNull] Research other)
        {
            if (!(other is Research))
            {
                return false;
            }
            return Order == other.Order && ResearchDate == other.ResearchDate;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
