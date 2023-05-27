using System.Collections.Generic;
using System;

namespace DTO
{
    public class ResearchDTO
    {
        public string Laboratory { get; set; }

        public DateTime ResearchDate { get; set; }

        public string? Comment { get; set; }
        public List<Item> Items { get; set; }

        public int? Id { get; set; }

        public string Num { get; set; }

        public bool NotificateDoctor { get; set; }

        public class Item
        {
            public int IndicatorType { get; set; }
            public string IndicatorName { get; set; }
            public double IndicatorValue { get; set; }
            public string IndicatorUnit { get; set; }
            public int BearingIndicatorId { get; set; }
        }
    }
}
