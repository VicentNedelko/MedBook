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
    }
}
