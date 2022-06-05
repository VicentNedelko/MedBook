using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace MedBook.Models.ViewModels
{
    public class FileResearchViewModel
    {
        public IFormFile PdfFile { get; set; }

        [DisplayName("PDF")]
        public string Pdf { get; set; }
    }
}
