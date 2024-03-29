﻿using MedBook.Models.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MedBook.Models
{
    public class Indicator : IComparable<Indicator>
    {
        public int Id { get; set; }
        public int? Number { get; set; }
        public string Name { get; set; }
        public IndTYPE Type { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public double? ReferentMax { get; set; }
        public double? ReferentMin { get; set; }
        public int BearingIndicatorId { get; set; }

        public int ResearchId { get; set; }
        public Research Research { get; set; }

        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public int CompareTo([AllowNull] Indicator other)
        {
            return Name.CompareTo(other.Name);
        }

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
            var result = Name.ToUpperInvariant() == ((Indicator)obj).Name.ToUpperInvariant();
            return result;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
