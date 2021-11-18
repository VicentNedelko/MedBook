using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class BearingIndVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя базового показателя")]
        public string Name { get; set; }
        public string Type { get; set; } // absolute, relative etc.
        public string? Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:$###,###}")]
        public double? ReferenceMax { get; set; }

        [DisplayFormat(DataFormatString = "{0:$###,###}")]
        public double? ReferenceMin { get; set; }
        public string? Unit { get; set; }
    }
}
