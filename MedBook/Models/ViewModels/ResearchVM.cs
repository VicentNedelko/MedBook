using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class ResearchVM
    {
        [Display(Name = "Лаборатория :")]
        [Required(ErrorMessage = "Укажите наименование лаборатории")]
        public string Laboratory { get; set; }

        [Display(Name = "Дата анализа :")]
        [Required(ErrorMessage = "Укажите корректную дату анализа")]
        public DateTime ResearchDate { get; set; }
        public List<Item> Items { get; set; }

        [Display(Name = "Пациент :")]
        public string PatientId { get; set; }
        public int? Id { get; set; }

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
