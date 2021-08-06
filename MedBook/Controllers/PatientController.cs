using MedBook.Models;
using MedBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
    public class PatientController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MedBookDbContext _medBookDbContext;

        public PatientController(IWebHostEnvironment webHostEnvironment, MedBookDbContext medBookDbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _medBookDbContext = medBookDbContext;
        }


        [HttpGet]
        public IActionResult ResearchUpload()
        {
            return View(new UploadFileVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResearchUploadAsync(UploadFileVM model)
        {
            if(model.File != null)
            {
                var fileName = Path.GetFileName(model.File.FileName);
                string ext = Path.GetExtension(model.File.FileName);
                if(Path.GetExtension(model.File.FileName).ToUpper() != ".PDF")
                {
                    ViewBag.ErrorMessage = "File is NOT a PDF-file. Please, choose the required file.";
                    return View();
                }
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }
                ViewBag.InfoMessage = "File uploaded successfully.";

                var text = PDFConverter.PdfGetter
                    .PdfToStringConvert(filePath)
                    .Split(new char[] {'\n'});

                var researchDateString = text.Where(t => t.Contains(
                    "Дата", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                var dateOfResearch = PDFConverter.PdfGetter.GetResearchDate(researchDateString);

                var paramsStrings = PDFConverter.PdfGetter
                    .GetDesiredParameters(text,
                    _medBookDbContext.SampleIndicators.AsNoTracking()
                    .ToArray().Select(sample => sample.Name)
                    .ToArray());

                var paramsList = paramsStrings
                    .Where(str => !String.IsNullOrEmpty(str));

                System.IO.File.Delete(filePath);
                return View();
            }
            ViewBag.ErrorMessage = "File is empty.";
            return View();
        }

        //public IActionResult ResearchEdit(string str)
        //{

        //}
    }
}
