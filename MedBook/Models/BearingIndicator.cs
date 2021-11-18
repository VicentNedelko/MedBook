using MedBook.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class BearingIndicator : IComparable<BearingIndicator>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IndTYPE Type { get; set; } // absolute, relative etc.
        public string? Description { get; set; }
        public double? ReferenceMax { get; set; }
        public double? ReferenceMin { get; set; }
        public string? Unit { get; set; }

        public List<SampleIndicator> Samples { get; set; }

        public int CompareTo([AllowNull] BearingIndicator indicator)
        {
            return this.Name.CompareTo(indicator.Name);
        }

    }
}
