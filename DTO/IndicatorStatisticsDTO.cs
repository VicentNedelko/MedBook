using System;

namespace DTO
{
    public class IndicatorStatisticsDTO
    {
        public string PatientNameDTO { get; set; }
        public string NameDTO { get; set; }
        public Item[] ItemsDTO { get; set; }

        public class Item
        {
            public double ValueDTO { get; set; }
            public DateTime ResearchDateDTO { get; set; }
            public string UnitDTO { get; set; }
        }
    }
}
