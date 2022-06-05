using MedBook.Models.Enums;
using System;

namespace MedBook.Models.ViewModels
{
    public class IndicatorStatisticsVM
    {
        public string Name { get; set; }
        public double? ReferentMax { get; set; }
        public double? ReferentMin { get; set; }
        public string PatientId { get; set; }
        public IndTYPE Type { get; set; }
        public Item[] Items { get; set; }

        public class Item
        {
            public double Value { get; set; }
            public DateTime ResearchDate { get; set; }
            public string Unit { get; set; }
        }
    }
}
