using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class Indicator
    {
        public int Id { get; set; }
        public int? Number { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public double? ReferentMax { get; set; }
        public double? ReferentMin { get; set; }

        public int ResearchId { get; set; }
        public Research Research { get; set; }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            if(!(obj is Indicator))
            {
                return false;
            }
            return Name == ((Indicator)obj).Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
