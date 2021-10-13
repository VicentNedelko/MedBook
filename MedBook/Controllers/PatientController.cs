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
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MedBook.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MedBookDbContext _medBookDbContext;
        private readonly UserManager<User> _userManager;

        public PatientController(IWebHostEnvironment webHostEnvironment, MedBookDbContext medBookDbContext, UserManager<User> userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _medBookDbContext = medBookDbContext;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult ResearchUpload(string id)
        {
            ViewBag.PatientId = id;
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

                var researchDateStringArray = text.Where(t => t.Contains(
                    "Дата", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                var dateOfResearch = PDFConverter.PdfGetter.GetResearchDate(researchDateStringArray);

                var paramsStrings = PDFConverter.PdfGetter
                    .GetDesiredParameters(text,
                    _medBookDbContext.SampleIndicators.AsNoTracking()
                    .ToArray().Select(sample => sample.Name)
                    .ToArray());

                var sampleIndicators = await _medBookDbContext.SampleIndicators.AsNoTracking().ToArrayAsync();

                var paramsList = paramsStrings
                    .Where(str => !String.IsNullOrEmpty(str));

                System.IO.File.Delete(filePath);

                ResearchVM researchVM = new ResearchVM
                {
                    Laboratory = PDFConverter.PdfGetter.GetLaboratoryName(text),
                    ResearchDate = dateOfResearch,
                    PatientId = model.patientId,
                    Items = new List<ResearchVM.Item>(),
                };

                

                (string name, double value) indicatorTuple;

                foreach(string str in paramsList)
                {
                    indicatorTuple = PDFConverter.PdfGetter.GetParameterValue(str, await _medBookDbContext.SampleIndicators
                        .AsNoTracking().Select(sample => sample.Name).ToArrayAsync());
                    researchVM.Items.Add(new ResearchVM.Item
                    {
                        IndicatorName = indicatorTuple.name,
                        IndicatorValue = indicatorTuple.value,
                        IndicatorUnit = sampleIndicators.Where(si => si.Name == indicatorTuple.name)
                        .FirstOrDefault().Unit,
                    });
                }
                var items = JsonSerializer.Serialize(researchVM);
                TempData["items"] = items;
                return RedirectToAction("ShowResearchData", "Research");
            }
            ViewBag.ErrorMessage = "File is empty.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ShowMyPatientsAsync()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patientIds = _medBookDbContext.Patients
                .Where(pat => pat.Doctor.Id == doctorId)
                .Select(pat => pat.Id).ToList();

            if(patientIds.Count == 0)
            {
                ViewBag.Message = "Список пациентов пуст. Добавьте нового пациента.";
                return View();
            }
            List<PatientVM> myPatients = new List<PatientVM>();
            foreach(var id in patientIds)
            {
                var patient = await _medBookDbContext.Patients.FindAsync(id);
                myPatients.Add(new PatientVM
                {
                    Id = patient.Id,
                    FName = patient.FName,
                    LName = patient.LName,
                    Age = patient.Age,
                });
            }
            return View(myPatients);
        }

        [HttpGet]
        public async Task<IActionResult> ShowDetailesAsync(string id)
        {
            var patient = await _medBookDbContext.Patients.FindAsync(id);
            if(patient != null)
            {
                var researchList = _medBookDbContext.Researches.Where(r => r.Patient.Id == patient.Id).ToArray();
                ViewBag.ResearchError = (researchList.Count() == 0) ? "Данные исследований не найдены." : "Данные исследований :";
                ViewBag.ResearchList = (researchList.Count() != 0) ? researchList : null;

                if(researchList.Count() != 0)
                {
                    var researches = _medBookDbContext.Researches
                                    .Where(res => res.PatientId == id)
                                    .Select(res => res.Indicators)
                                    .AsNoTracking()
                                    .ToList();

                    var indicList = researches.Aggregate((prev, next) => prev.Union(next).ToList())
                                    .Select(ind => new IndicatorVM { Id = ind.Id, Name = ind.Name }).ToArray();
                    Array.Sort(indicList);
                    ViewBag.IndicatorList = indicList;
                    return View(patient);
                }
            }
            ViewBag.Error = "Данные не найдены.";
            return View(patient);
        }

        [HttpGet]
        [Route("/Patient/ShowStatistics")]
        public async Task<IActionResult> ShowStatisticsAsync([FromQuery]string patientId, [FromQuery]int indicatorId)
        {
            var controlIndicatorName = _medBookDbContext.Indicators
                .Where(ind => ind.Id == indicatorId).FirstOrDefault().Name;

            //var result = _medBookDbContext.Indicators.ToLookup(ind => ind.Name == controlIndicatorName);
            var indicatorStatistics = new IndicatorStatisticsVM
            {
                Name = controlIndicatorName,
                Items = await _medBookDbContext.Indicators
                        .Where(ind => ind.Name == controlIndicatorName)
                        .Where(ind => ind.PatientId == patientId)
                        .Select(ind => new IndicatorStatisticsVM.Item
                        {
                            ResearchDate = ind.Research.ResearchDate, 
                            Value = ind.Value,
                            Unit = ind.Unit
                        })
                        .OrderBy(i => i.ResearchDate)
                        .AsNoTracking()
                        .ToArrayAsync(),
            };
            var patientResearches = await _medBookDbContext.Researches
                .Where(res => res.PatientId == patientId)
                .AsNoTracking()
                .ToArrayAsync();

            return View(indicatorStatistics);
        }

        [HttpGet]
        public async Task<IActionResult> ManualUploadAsync(string id)
        {
            ViewBag.Patient = await _medBookDbContext.Patients.FindAsync(id);
            ViewBag.Indicators = await _medBookDbContext.SampleIndicators.ToArrayAsync();
            var researchVM = new ResearchVM
            {
                Laboratory = "Not defined",
                ResearchDate = DateTime.Now,
                PatientId = id,
                Items = new List<ResearchVM.Item>(),
            };
            return View(researchVM);
        }

    }
}
