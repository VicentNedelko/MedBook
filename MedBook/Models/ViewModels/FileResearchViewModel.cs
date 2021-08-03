using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class FileResearchViewModel
    {
        public IFormFile PdfFile { get; set; }

        [DisplayName("PDF")]
        public string Pdf { get; set; }
    }
}
