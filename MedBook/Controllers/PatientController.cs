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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using MedBook.Models.Enums;

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

                var rawText = PDFConverter.PdfGetter
                    .PdfToStringConvert(filePath);
                var text = rawText.Split(new char[] {'\n' });
                string plainText = Regex.Replace(rawText, @"\t|\n|\r", " ");
                //string clearedText = Regex.Replace(plainText, @"\s+", " ");
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                string clearedText = regex.Replace(plainText, " ");

                var researchDateStringArray = text.Where(t => t.Contains(
                    "Дата", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                var dateOfResearch = PDFConverter.PdfGetter.GetResearchDate(researchDateStringArray);

                var actualSamplesInResearch = PDFConverter.PdfGetter
                    .GetActualSampleNames(clearedText,
                    await _medBookDbContext.SampleIndicators
                    .Select(si => new SampleDTO
                    {
                        Id = si.Id,
                        Name = si.Name,
                        Unit = si.Unit,
                        ReferenceMax = si.ReferenceMax,
                        ReferenceMin = si.ReferenceMin,
                        BearingIndicatorId = si.BearingIndicatorId,
                    })
                    .AsNoTracking()
                    .ToArrayAsync());

                System.IO.File.Delete(filePath);

                ResearchVM researchVM = new ResearchVM
                {
                    Laboratory = PDFConverter.PdfGetter.GetLaboratoryName(text),
                    ResearchDate = dateOfResearch,
                    PatientId = model.patientId,
                    Items = new List<ResearchVM.Item>(),
                };

                foreach (var exactIndicator in actualSamplesInResearch)
                {
                    var bearInd = await _medBookDbContext.BearingIndicators.FindAsync(exactIndicator.BearingIndicatorId);
                    researchVM.Items.Add(new ResearchVM.Item
                    {
                        IndicatorName = bearInd.Name,
                        IndicatorValue = PDFConverter.PdfGetter.GetParameterValue(clearedText, exactIndicator, Convert.ToInt32(bearInd.Type)),
                        IndicatorUnit = bearInd.Unit,
                        IndicatorType = Convert.ToInt32(bearInd.Type),
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

            var currentPatient = await _medBookDbContext.Patients.FindAsync(patientId);
            ViewBag.PatientName = string.Concat(currentPatient.FName, " ", currentPatient.LName);

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
        public async Task<IActionResult> ShowFullReportAsync(string id)
        {
            List<IndicatorStatisticsVM> indicatorStatisticsVMs = new List<IndicatorStatisticsVM>();
            var currentPatient = await _medBookDbContext.Patients.FindAsync(id);
            var indicatorNames = await _medBookDbContext.Indicators
                .Where(ind => ind.PatientId == id)
                .Select(ind => ind.Name).Distinct()
                .ToArrayAsync();
            foreach(var name in indicatorNames)
            {
                var baseIndicator = _medBookDbContext.BearingIndicators.FirstOrDefault(sa => sa.Name == name);
                var indicatorStatistics = new IndicatorStatisticsVM
                {
                    Name = name,
                    ReferentMax = baseIndicator.ReferenceMax ?? 0,
                    ReferentMin = baseIndicator.ReferenceMin ?? 0,
                    Items = await _medBookDbContext.Indicators
                        .Where(ind => ind.Name == name)
                        .Where(ind => ind.PatientId == id)
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
                indicatorStatisticsVMs.Add(indicatorStatistics);
            };

            ViewBag.Patient = currentPatient;
            return View(indicatorStatisticsVMs);
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

        public async Task<IActionResult> ShowAllAsync()
        {
            List<PatientVM> patients = new List<PatientVM>();
            var pats = await _medBookDbContext.Patients.AsNoTracking().ToListAsync();
            foreach(var p in pats)
            {
                patients.Add(new PatientVM
                {
                    Id = p.Id,
                    FName = p.FName,
                    LName = p.LName,
                    Age = p.Age,
                });
            }
            if(pats.Count == 0)
            {
                ViewBag.Error = "Список пациентов пуст.";
            }
            return View(patients);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAsync(string id)
        {
            var patToDelete = await _medBookDbContext.Patients.FindAsync(id);
            var indsToDelete = _medBookDbContext.Indicators.Where(i => i.PatientId == id);
            var resToDelete = _medBookDbContext.Researches.Where(r => r.PatientId == id);
            if(indsToDelete.Count() != 0)
            {
                foreach (var ind in indsToDelete)
                {
                    _medBookDbContext.Indicators.Remove(ind);
                }
            }
            ViewBag.IndicatorDeleteResult = "Indicators deleted";
            if(resToDelete.Count() != 0)
            {
                foreach (var res in resToDelete)
                {
                    _medBookDbContext.Researches.Remove(res);
                }
            }
            _medBookDbContext.Patients.Remove(patToDelete);

            try
            {
                await _medBookDbContext.SaveChangesAsync();
            }
            catch(Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Error");
            }
            return RedirectToAction("ShowAll");
        }

        public IActionResult Error()
        {
            ViewBag.Error = TempData["error"];
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditAsync(string id)
        {
            var patEdit = await _medBookDbContext.Patients.FindAsync(id);
            PatientEditVM patientEditVM = new PatientEditVM
            {
                Id = patEdit.Id,
                Email = patEdit.Email,
                FName = patEdit.FName,
                LName = patEdit.LName,
                Age = patEdit.Age,
                Gender = patEdit.Gender,
                DoctorId = patEdit.DoctorId,
            };
            ViewBag.Doctors = await _medBookDbContext.Doctors.AsNoTracking().ToArrayAsync();
            return View(patientEditVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditAsync(PatientEditVM model, string docId)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await _medBookDbContext.Doctors.AsNoTracking().ToArrayAsync();
                return View(model);
            }
            var patToUpdate = await _medBookDbContext.Patients.FindAsync(model.Id);
            var newDoc = await _medBookDbContext.Doctors.FindAsync(docId);

            // update properties
            patToUpdate.FName = model.FName;
            patToUpdate.LName = model.LName;
            patToUpdate.Email = model.Email;
            patToUpdate.Age = model.Age;
            patToUpdate.Gender = model.Gender;
            patToUpdate.DoctorId = docId;
            patToUpdate.Doctor = newDoc;

            //Save
            try
            {
                await _medBookDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Error");
            }


            return RedirectToAction("ShowAll");
        }
    }
}
