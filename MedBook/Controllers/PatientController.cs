using AutoMapper;
using DTO;
using MedBook.Models;
using MedBook.Models.Enums;
using MedBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MedBookDbContext _medBookDbContext;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public PatientController(
            IWebHostEnvironment webHostEnvironment,
            MedBookDbContext medBookDbContext,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _webHostEnvironment = webHostEnvironment;
            _medBookDbContext = medBookDbContext;
            _userManager = userManager;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult ResearchUpload(string id)
        {
            ViewBag.PatientId = id;
            return View(new UploadFileVM());
        }

        [HttpPost]
        public async Task<IActionResult> FindPatientIndicatorsAsync(string indicatorName, string patientId)
        {
            var patientIndicators = await _medBookDbContext.Indicators
                .Where(ind => ind.PatientId == patientId)
                .Where(ind => ind.Name.ToUpper().Contains(indicatorName.ToUpper()))
                .Select(ind => new IndicatorVM {Id = ind.Id, Name = ind.Name})
                .ToListAsync();
            var indicatorsVM = patientIndicators.GroupBy(ind => ind.Name)
                .Select(ind => ind.First()).ToList();
            ViewBag.PatientId = patientId;
            return PartialView(indicatorsVM);
        }

        [HttpPost]
        public async Task<PartialViewResult> FindIndicatorAsync(string indicatorName)
        {
            var sampleIndicators = await _medBookDbContext.SampleIndicators
                .Where(x => x.Name.ToUpper().Contains(indicatorName.ToUpper()))
                .Select(x => new IndicatorVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Unit = x.Unit,
                    ReferentMax = x.ReferenceMax,
                    ReferentMin = x.ReferenceMin,
                    Type = x.Type,
                    BearingIndicatorId = x.BearingIndicatorId,
                })
                .AsNoTracking().ToListAsync();
            return PartialView(sampleIndicators);
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
                ViewBag.InfoMessage = $"Файл загружен : {fileName}";

                var rawText = PDFConverter.PdfGetter
                    .PdfToStringConvert(filePath);
                var text = rawText.Split(new char[] { '\n' });
                string plainText = Regex.Replace(rawText, @"\t|\n|\r", " ");
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                string cleared = regex.Replace(plainText, " ");
                string clearedText = cleared.Replace("*", string.Empty);


                var researchDateStringArray = text.Where(t => t.Contains(
                    "Дата", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                DateTime dateOfResearch = DateTime.Now;
                if (researchDateStringArray.Length != 0)
                {
                    dateOfResearch = PDFConverter.PdfGetter.GetResearchDate(researchDateStringArray);
                }

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

                //
                string pidString = String.Empty;
                string researchPID = String.Empty;
                if (researchVM.Laboratory == PDFConverter.Constants.LaboratoryName.INVITRO)
                {
                    pidString = text.Where(t => t.Contains(PDFConverter.Constants.ResearchPID.RPID_INVITRO))
                                        .FirstOrDefault();
                    researchPID = PDFConverter.PdfGetter.GetResearchPIDInvitro(pidString);
                }
                else if (researchVM.Laboratory == PDFConverter.Constants.LaboratoryName.SYNEVO)
                {
                    researchPID = PDFConverter.PdfGetter.GetResearchPIDSynevo(text);
                }
                researchVM.Num = String.IsNullOrEmpty(researchPID) ? "Не определено" : researchPID;

                foreach (var exactIndicator in actualSamplesInResearch)
                {
                    var bearInd = await _medBookDbContext.BearingIndicators.FindAsync(exactIndicator.BearingIndicatorId);
                    researchVM.Items.Add(new ResearchVM.Item
                    {
                        IndicatorName = bearInd.Name,
                        IndicatorValue = PDFConverter.PdfGetter.GetParameterValue(clearedText, exactIndicator, Convert.ToInt32(bearInd.Type)),
                        IndicatorUnit = bearInd.Unit,
                        IndicatorType = Convert.ToInt32(bearInd.Type),
                        BearingIndicatorId = bearInd.Id,
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
                _medBookDbContext.RemoveRange(
                    _medBookDbContext.Researches
                    .Where(x => x.Indicators == null || x.Indicators.Count == 0)
                    .Where(res => res.PatientId == patient.Id));
                await _medBookDbContext.SaveChangesAsync();
                var researchList = _medBookDbContext.Researches.Where(r => r.Patient.Id == patient.Id).AsQueryable().AsNoTracking()
                    .OrderBy(r => r.ResearchDate).ToArray();

                ViewBag.ResearchError = (researchList.Count() == 0) ? "Данные исследований не найдены." : "Данные исследований";
                ViewBag.ResearchList = (researchList.Count() != 0) ? researchList : null;

                if (researchList.Count() != 0)
                {
                    var researches = _medBookDbContext.Researches
                                    .Where(res => res.PatientId == id)
                                    .Select(res => res.Indicators)
                                    .AsNoTracking()
                                    .ToList();

                    var indicList = researches.Aggregate((prev, next) => prev.Union(next).ToList())
                                    .Select(ind => new IndicatorVM { Id = ind.Id, Name = ind.Name }).ToList();
                    var indicatorListVM = indicList.GroupBy(x => x.Name)
                        .Select(group => group.First())
                        .OrderBy(ind => ind.Name).ToList();
                    ViewBag.IndicatorList = indicatorListVM;
                    return View(patient);
                }
            }
            ViewBag.Error = "Данные не найдены.";
            return View(patient);
        }

        [HttpGet]
        public async Task<IActionResult> ShowIndicatorsAsync(string id)
        {
            var indicatorList = await _medBookDbContext.Indicators
                .Where(ind => ind.PatientId == id)
                .Select(ind => new IndicatorVM { Id = ind.Id, Name = ind.Name})
                .ToListAsync();
            var indicatorListVM = indicatorList.GroupBy(x => x.Name)
                .Select(group => group.First())
                .OrderBy(ind => ind.Name).ToList();
            ViewBag.PatientId = id;
            return View(indicatorListVM);
        }

        [HttpGet]
        [Route("/Patient/ShowStatistics")]
        public async Task<IActionResult> ShowStatisticsAsync([FromQuery] string patientId, [FromQuery] int indicatorId)
        {
            var controlIndicatorName = _medBookDbContext.Indicators
                .Where(ind => ind.Id == indicatorId).FirstOrDefault().Name;

            var controlIndicatorType = _medBookDbContext.Indicators
                .Where(ind => ind.Id == indicatorId).FirstOrDefault().Type;

            if (controlIndicatorType == IndTYPE.YESNO)
            {
                return RedirectToAction("ShowStatisticsDiscrete", new { patientId, indicatorId });
            }

            var currentPatient = await _medBookDbContext.Patients.FindAsync(patientId);
            ViewBag.PatientName = string.Concat(currentPatient.FName, " ", currentPatient.LName);

            var indicatorStatistics = new IndicatorStatisticsVM
            {
                Name = controlIndicatorName,
                Type = controlIndicatorType,
                Items = await _medBookDbContext.Indicators
                        .Where(ind => ind.Name == controlIndicatorName)
                        .Where(ind => ind.PatientId == patientId)
                        .Select(ind => new IndicatorStatisticsVM.Item
                        {
                            ResearchDate = ind.Research.ResearchDate,
                            ResearchId = ind.ResearchId,
                            Value = ind.Value,
                            Unit = ind.Unit
                        })
                        .OrderBy(i => i.ResearchDate)
                        .AsNoTracking()
                        .ToArrayAsync(),
            };

            ViewBag.PatientId = patientId;
            return View(indicatorStatistics);
        }

        [HttpGet]
        public async Task<IActionResult> ShowStatisticsDiscreteAsync(string patientId, int indicatorId)
        {
            var controlIndicatorName = _medBookDbContext.Indicators
                .Where(ind => ind.Id == indicatorId).FirstOrDefault().Name;
            var currentPatient = await _medBookDbContext.Patients.FindAsync(patientId);
            ViewBag.PatientName = string.Concat(currentPatient.FName, " ", currentPatient.LName);
            var indicatorStatistics = new IndicatorStatisticsVM
            {
                Name = controlIndicatorName,
                Type = _medBookDbContext.Indicators.Where(ind => ind.Id == indicatorId).FirstOrDefault().Type,
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
            foreach (var name in indicatorNames)
            {
                var baseIndicator = _medBookDbContext.BearingIndicators.FirstOrDefault(sa => sa.Name == name);
                if (baseIndicator != null)
                {
                    var indicatorStatistics = new IndicatorStatisticsVM
                    {
                        Name = name,
                        Type = baseIndicator.Type,
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
                }
            };

            ViewBag.Patient = currentPatient;
            return View(indicatorStatisticsVMs);
        }

        [HttpGet]
        public async Task<IActionResult> AddNewResearchAsync(string id)
        {
            var manualResearch = new Research
            {
                Order = "Лаборатория",
                PatientId = id,
            };
            await _medBookDbContext.Researches.AddAsync(manualResearch);
            await _medBookDbContext.SaveChangesAsync();
            var researchVM = _mapper.Map<ResearchVM>(manualResearch);

            return View(researchVM);
        }

        [HttpPost]
        public async Task<IActionResult> ManualUploadAsync(ResearchVM model)
        {
            var research = await _medBookDbContext.Researches
                .FindAsync(model.Id);
            research.ResearchDate = model.ResearchDate;
            research.Comment = model.Comment ?? string.Empty;
            await _medBookDbContext.SaveChangesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManualUploadAsync(string id)
        {
            _medBookDbContext.Researches
                .RemoveRange(_medBookDbContext.Researches.Where(r => r.Indicators.Count == 0));
            await _medBookDbContext.SaveChangesAsync();
            var manualResearch = new Research
            {
                Order = "Лаборатория",
                Num = Guid.NewGuid().ToString(),
                ResearchDate = DateTime.Now.Date,
                PatientId = id,
                Indicators = new List<Indicator>(),
            };
            await _medBookDbContext.Researches.AddAsync(manualResearch);
            await _medBookDbContext.SaveChangesAsync();
            var research = _medBookDbContext.Researches
                .FirstOrDefault(x => x.Num == manualResearch.Num);
            var researchVM = _mapper.Map<ResearchVM>(research);
            ViewBag.Indicators = researchVM;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddIndicatorToManualResearchAsync(int ResearchId, string manualIndicatorName, string manualIndicatorUnit, string manualIndicatorValue, int indicatorId)
        {
            var research = await _medBookDbContext.Researches.FindAsync(ResearchId);
            if (research == null)
            {
                ViewBag.ErrorMessage = "Исследование не найдено";
                return View("Error");
            }
            var indicator = await _medBookDbContext.SampleIndicators.FindAsync(indicatorId);
            if (indicator == null)
            {
                ViewBag.ErrorMessage = $"Индикатор с ID = {indicatorId} не найден.";
            }
            var bearingInd = await _medBookDbContext.BearingIndicators.FindAsync(indicator.BearingIndicatorId);
            var indicatorToAdd = new Indicator
            {
                Name = bearingInd.Name,
                Type = bearingInd.Type,
                Value = Convert.ToDouble(manualIndicatorValue),
                Unit = bearingInd.Unit,
                ReferentMax = bearingInd.ReferenceMax,
                ReferentMin = bearingInd.ReferenceMin,
                BearingIndicatorId = bearingInd.Id,
                PatientId = research.PatientId,
                ResearchId = research.Id,
            };
            _medBookDbContext.Indicators.Add(indicatorToAdd);
            await _medBookDbContext.SaveChangesAsync();
            var researchUpdate = await _medBookDbContext.Researches.FindAsync(research.Id);
            var researchVM = _mapper.Map<ResearchVM>(researchUpdate);
            researchVM.Items = new List<ResearchVM.Item>();
            var indicatorList = await _medBookDbContext.Indicators
                .Where(ind => ind.ResearchId == research.Id).ToListAsync();
            foreach (var ind in indicatorList)
            {
                researchVM.Items.Add(new ResearchVM.Item
                {
                    IndicatorName = ind.Name,
                    IndicatorType = ind.Type == IndTYPE.VALUE ? 0 : 1,
                    IndicatorUnit = ind.Unit,
                    IndicatorValue = ind.Value,
                    BearingIndicatorId = ind.BearingIndicatorId,
                });
            }
            ViewBag.ResearchId = ResearchId;
            return PartialView(researchVM);
        }

        [HttpPost]
        public async Task<IActionResult> SavePDFAsync(IndicatorStatisticsVM model)
        {
            var patient = await _medBookDbContext.Patients.FindAsync(model.PatientId);
            IndicatorStatisticsDTO indicatorStatisticsDTO = new IndicatorStatisticsDTO
            {
                NameDTO = model.Name,
                TypeDTO = Convert.ToInt32(model.Type),
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
            var dirPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", $"{User.Identity.Name}", $"{model.PatientId}");
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
            foreach (var p in pats)
            {
                patients.Add(new PatientVM
                {
                    Id = p.Id,
                    FName = p.FName,
                    LName = p.LName,
                    Age = p.Age,
                });
            }
            if (pats.Count == 0)
            {
                ViewBag.Error = "Список пациентов пуст.";
            }
            return View(patients);
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
                IsBlock = patEdit.IsBlock,
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
            patToUpdate.IsBlock = model.IsBlock;

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
