using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class IndicatorStatisticsVM
    {
        public string Name { get; set; }
        public Item[] Items { get; set; }

        public class Item
        {
            public double Value { get; set; }
            public DateTime ResearchDate { get; set; }
        }
    }
}
