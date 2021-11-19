using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class SampleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? Unit { get; set; }

        public double? ReferenceMax { get; set; }
        public double? ReferenceMin { get; set; }
        public int StartIndex { get; set; }

        //Bearing FK

        public int BearingIndicatorId { get; set; }
        //public BearingIndicator BearingIndicator { get; set; }
    }
}
