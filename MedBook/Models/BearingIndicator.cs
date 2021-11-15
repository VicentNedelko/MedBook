using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models
{
    public class BearingIndicator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<SampleIndicator> Samples { get; set; }

    }
}
