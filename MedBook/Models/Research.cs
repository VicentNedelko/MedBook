using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MedBook.Models
{
    public class Research : IEquatable<Research>
    {
        public int Id { get; set; }
        public string Order { get; set; } // Laboratory Name
        public string Num { get; set; } // Order PID
        public DateTime ResearchDate { get; set; }

        // Patient FK
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        //Visit FK
        public int VisitId { get; set; }
        public Visit Visit { get; set; }

        public List<Indicator> Indicators { get; set; }


        public bool Equals([AllowNull] Research other)
        {
            if (!(other is Research))
            {
                return false;
            }
            var result = ResearchDate == other.ResearchDate
                && PatientId == other.PatientId
                && Indicators.Count == other.Indicators.Count
                && IndicatorsEquality(Indicators, other.Indicators);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool IndicatorsEquality(List<Indicator> indicators1, List<Indicator> indicators2)
        {
            for(var i = 0; i < indicators1.Count; i++)
            {
                if (!(indicators1[i].Equals(indicators2[i])))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
