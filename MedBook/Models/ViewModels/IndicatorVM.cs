using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class IndicatorVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double? ReferentMax { get; set; }
        public double? ReferentMin { get; set; }
    }
}
