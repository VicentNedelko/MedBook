using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class ResearchVM
    {
        public string Laboratory { get; set; }
        public DateTime ResearchDate { get; set; }
        public List<Item> Items { get; set; }

        public class Item
        {
            public string IndicatorName { get; set; }
            public double IndicatorValue { get; set; }
        }
    }
}
