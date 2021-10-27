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
using DTO;
using static System.Net.Mime.MediaTypeNames;

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
        public async Task<IActionResult> ResearchUploadAsync(UploadFileVM model)
        {
            if (model.File != null)
            {
                var fileName = Path.GetFileName(model.File.FileName);
                string ext = Path.GetExtension(model.File.FileName);
                if (Path.GetExtension(model.File.FileName).ToUpper() != ".PDF")
                {
                    ViewBag.ErrorMessage = "File is NOT a PDF-file. Please, choose the required file.";
                    return View();
                }

                var fileDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", $"{User.Identity.Name}");
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", $"{User.Identity.Name}", fileName);

                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }
                ViewBag.InfoMessage = "File uploaded successfully.";

                var text = PDFConverter.PdfGetter
                    .PdfToStringConvert(filePath)
                    .Split(new char[] { '\n' });

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

                foreach (string str in paramsList)
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

            if (patientIds.Count == 0)
            {
                ViewBag.Message = "Список пациентов пуст. Добавьте нового пациента.";
                return View();
            }
            List<PatientVM> myPatients = new List<PatientVM>();
            foreach (var id in patientIds)
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
            if (patient != null)
            {
                var researchList = _medBookDbContext.Researches.Where(r => r.Patient.Id == patient.Id).ToArray();
                ViewBag.ResearchError = (researchList.Count() == 0) ? "Данные исследований не найдены." : "Данные исследований :";
                ViewBag.ResearchList = (researchList.Count() != 0) ? researchList : null;

                if (researchList.Count() != 0)
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
        public async Task<IActionResult> ShowStatisticsAsync([FromQuery] string patientId, [FromQuery] int indicatorId)
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
            ViewBag.PatientId = patientId;
            return View(indicatorStatistics);
        }

        [HttpGet]
        public async Task<IActionResult> ManualUploadAsync(int number, string patId)
        {
            ViewBag.PatientId = patId;
            ViewBag.ItemNumber = number;
            ViewBag.Indicators = await _medBookDbContext.SampleIndicators.ToArrayAsync();
            return View();
        }

        [HttpPost]
        public IActionResult ManualUpload(ResearchVM model)
        {
            var items = JsonSerializer.Serialize(model);
            TempData["items"] = items;
            return RedirectToAction("ShowResearchData", "Research");
        }

        [HttpGet]
        public IActionResult ManualUploadItems(string id)
        {
            ViewBag.PatientId = id;
            return View();
        }
        [HttpPost]
        public IActionResult ManualUploadItems(string itemNumber, string id)
        {
            var numberInt = Int32.Parse(itemNumber);
            return RedirectToAction("ManualUpload", new { number = numberInt, patId = id });
        }

        [HttpPost]
        public async Task<IActionResult> SavePDFAsync(IndicatorStatisticsVM model)
        {
            var patient = await _medBookDbContext.Patients.FindAsync(model.PatientId);
            IndicatorStatisticsDTO indicatorStatisticsDTO = new IndicatorStatisticsDTO
            {
                NameDTO = model.Name,
                PatientNameDTO = string.Concat(patient.FName, " ", patient.LName),
                ItemsDTO = new IndicatorStatisticsDTO.Item[model.Items.Count()],
            };
            for (int i = 0; i < model.Items.Count(); i++)
            {
                indicatorStatisticsDTO.ItemsDTO[i] = new IndicatorStatisticsDTO.Item
                {
                    ValueDTO = model.Items[i].Value,
                    ResearchDateDTO = model.Items[i].ResearchDate,
                    UnitDTO = model.Items[i].Unit,
                };

            };
            var dirPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", $"{User.Identity.Name}" ,$"{model.PatientId}");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            };
            var filePath = Path.Combine(dirPath, string.Concat(model.Name, ".pdf"));
            PDFConverter.Creator.CreateReport(indicatorStatisticsDTO, dirPath);
            return PhysicalFile(filePath, "application/pdf", Path.GetFileName(filePath));
        }

        [HttpPost]
        [Route("Patient/GetImageChart")]
        public async Task GetImageChartAsync(ImageDTO imageDTO)
        {
            var cuttedStr = imageDTO.ImageBase64.Remove(0, 22);
            var imageBytes = Convert.FromBase64String(cuttedStr);
            var dirPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", $"{User.Identity.Name}", $"{imageDTO.PatId}");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, dirPath, "imageIndicator.png");
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
        }
    }
}
