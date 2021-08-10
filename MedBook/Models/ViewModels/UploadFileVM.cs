using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class UploadFileVM
    {
        public string patientId { get; set; }

        [Display(Name = "Enter name : ")]
        public string Name { get; set; }

        [Display(Name = "Choose a file to download : ")]
        public IFormFile File { get; set; }
    }
}
