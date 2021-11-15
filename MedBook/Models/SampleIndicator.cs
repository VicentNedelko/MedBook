using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class SampleIndicator : IComparable<SampleIndicator>
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string? Unit { get; set; }

        public double? ReferenceMax { get; set; }
        public double? ReferenceMin { get; set; }

        //Bearing FK

        public int BearingIndicatorId { get; set; }
        public BearingIndicator BearingIndicator { get; set; }

        public int CompareTo([AllowNull] SampleIndicator indicator)
        {
            return this.Name.CompareTo(indicator.Name);
        }
    }
}
