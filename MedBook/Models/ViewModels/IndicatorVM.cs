﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class IndicatorVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Укажите MAX значение")]
        [Range(0, double.MaxValue, ErrorMessage = "Неверный формат данных")]
        public double? ReferentMax { get; set; }

        [Required(ErrorMessage = "Укажите MIN значение")]
        [Range(0, double.MaxValue, ErrorMessage = "Неверный формат данных")]
        public double? ReferentMin { get; set; }
    }
}
