﻿using MedBook.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MedBook.Models.ViewModels
{
    public class BearingIndVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя базового показателя")]
        public string Name { get; set; }
        public IndTYPE Type { get; set; } // absolute, relative etc.
        public string? Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:$#####,####}")]
        [Range(-1, double.MaxValue, ErrorMessage = "Неверное значение")]
        public double? ReferenceMax { get; set; }

        [DisplayFormat(DataFormatString = "{0:$#####,####}")]
        [Range(-1, double.MaxValue, ErrorMessage = "Неверное значение")]
        public double? ReferenceMin { get; set; }
        public string? Unit { get; set; }
    }
}
