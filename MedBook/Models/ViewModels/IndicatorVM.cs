using MedBook.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MedBook.Models.ViewModels
{
    public class IndicatorVM : IComparable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Укажите MAX значение")]
        [Range(-1, double.MaxValue, ErrorMessage = "Неверный формат данных")]
        public double? ReferentMax { get; set; }

        [Required(ErrorMessage = "Укажите MIN значение")]
        [Range(-1, double.MaxValue, ErrorMessage = "Неверный формат данных")]
        public double? ReferentMin { get; set; }

        public IndTYPE Type { get; set; }

        public int BearingIndicatorId { get; set; }

        public int CompareTo(object obj)
        {
            IndicatorVM anotherIndicator = obj as IndicatorVM;
            if(anotherIndicator != null)
            {
                return Name.CompareTo(anotherIndicator.Name);
            }
            else
            {
                throw new ArgumentException("Object is not an Indicator");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is IndicatorVM))
            {
                return false;
            }
            return this.Name.Equals(((IndicatorVM)obj).Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
