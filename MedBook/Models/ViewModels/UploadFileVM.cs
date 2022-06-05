using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MedBook.Models.ViewModels
{
    public class UploadFileVM
    {
        public string patientId { get; set; }

        [Display(Name = "Метка файла (не обязательно) : ")]
        public string Name { get; set; }

        [Display(Name = "Выберите файл (.PDF) : ")]
        public IFormFile File { get; set; }
    }
}
