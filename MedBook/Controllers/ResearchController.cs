using MedBook.Models;
using MedBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MedBook.Models.Enums;
using MedBook.Managers.ResearchesManager;
using AutoMapper;
using MedBook.Managers.EmailManager;

namespace MedBook.Controllers
{
    [Authorize]
    public class ResearchController : Controller
    {
        private readonly MedBookDbContext _medBookDbContext;
        private readonly ResearchManager _researchManager;
        private readonly IMapper _mapper;
        private readonly IEmailManager _emailManager;

        public ResearchController(MedBookDbContext medBookDbContext, ResearchManager researchManager,
            IMapper mapper, IEmailManager emailManager)
        {
            _medBookDbContext = medBookDbContext;
            _researchManager = researchManager;
            _mapper = mapper;
            _emailManager = emailManager;
        }


        [HttpGet]
        public IActionResult ShowResearchData()
        {
            ResearchVM researchVM = new() { Items = new List<ResearchVM.Item>() };
            if (TempData["items"] is string s)
            {
                researchVM = JsonSerializer.Deserialize<ResearchVM>(s);
            }

            return View(researchVM);
        }


        [HttpPost]
        public async Task<IActionResult> ShowResearchDataAsync(ResearchVM model)
        {
            if (ModelState.IsValid)
            {
                var research = new Research
                {
                    Order = model.Laboratory,
                    ResearchDate = model.ResearchDate,
                    Comment = model.Comment,
                    Patient = await _medBookDbContext.Patients.FindAsync(model.PatientId),
                    PatientId = model.PatientId,
                    Indicators = new List<Indicator>(),
                    Num = model.Num,
                };
                var researchIndicatorsModel = model.Items
                    .Select(ind => new Indicator
                    {
                        Type = EnumConverter.IntToEnum(ind.IndicatorType),
                        Name = ind.IndicatorName,
                        Value = ind.IndicatorValue,
                        Unit = ind.IndicatorUnit,
                        Research = research,
                        PatientId = model.PatientId,
                        BearingIndicatorId = ind.BearingIndicatorId,
                    })
                    .OrderBy(ind => ind.Name)
                    .ToList();
                research.Indicators = researchIndicatorsModel;

                // check if Research is also exist
                var isDuplicated = _researchManager.IsResearchDuplicated(research);
                if (!isDuplicated)
                {
                    var addResult = await _medBookDbContext.Researches.AddAsync(research);
                    if (addResult.State == EntityState.Added)
                    {
                        await _medBookDbContext.SaveChangesAsync();
                        if (model.NotificateDoctor)
                        {
                            var link = Url.Action("Index", "Home", null, HttpContext.Request.Scheme);
                            await _emailManager.SendNotificationToDoctorAsync(research.Patient, link);
                        }
                        return RedirectToAction("ShowDetailes", "Patient", new { id = model.PatientId });
                    }
                    ViewBag.ErrorMessage = "DB Error! Item hasn't been saved.";
                }
                ViewBag.ErrorMessage = $"Исследование с такой же датой, наименованием лаборатории и списком показателей уже внесено в базу.";
                return View("ErrorResearchDuplicate", research);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResearchDetailesAsync(string id)
        {
            var research = await _medBookDbContext.Researches.FindAsync(Convert.ToInt32(id));
            if (research == null)
            {
                ViewBag.Error = "Данные не найдены.";
                return View(research);
            }
            var researchVM = new ResearchVM
            {
                Laboratory = research.Order,
                ResearchDate = research.ResearchDate,
                PatientId = research.PatientId,
                Num = research.Num,
                Items = _medBookDbContext.Indicators.Where(ind => ind.ResearchId == research.Id)
                .Select(ind => new ResearchVM.Item
                {
                    OriginId = ind.Id,
                    IndicatorType = Convert.ToInt32(ind.Type),
                    IndicatorName = ind.Name,
                    IndicatorUnit = ind.Unit,
                    IndicatorValue = ind.Value,
                }).OrderBy(x => x.IndicatorName).ToList(),
            };
            var patient = await _medBookDbContext.Patients.FindAsync(research.PatientId);
            ViewBag.PatientLname = patient.LName;
            ViewBag.PatientFname = patient.FName;
            ViewBag.PatientAge = patient.Age;
            ViewBag.ResearchID = id;
            return View(researchVM);
        }


        [HttpPost]
        public async Task<IActionResult> FindIndicatorAsync(string inputIndicator)
        {
            var indicator = await _medBookDbContext.SampleIndicators.Where(ind => ind.Name.ToUpper().StartsWith(inputIndicator.ToUpper()))
                .Select(ind => new IndicatorVM
                {
                    Id = ind.Id,
                    Name = ind.Name,
                    Unit = ind.Unit,
                    ReferentMax = ind.ReferenceMax,
                    ReferentMin = ind.ReferenceMin,
                })
                .ToListAsync();
            return PartialView("_FindIndicator", indicator);
        }

        [HttpGet]
        public async Task<IActionResult> ShowIndicatorParamsAsync(int id)
        {
            var indicator = await _medBookDbContext.SampleIndicators.FindAsync(id);
            var indicatorVM = new IndicatorVM
            {
                Id = indicator.Id,
                Name = indicator.Name,
                Unit = indicator.Unit,
                ReferentMax = indicator.ReferenceMax,
                ReferentMin = indicator.ReferenceMin,
            };
            return PartialView("_ShowIndicatorParams", indicatorVM);
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var researchToEdit = _medBookDbContext.Researches.AsQueryable()
                .FirstOrDefault(r => r.Id == Convert.ToInt32(id));
            if (researchToEdit != null)
            {
                ResearchVM researchVM = new ResearchVM
                {
                    Laboratory = researchToEdit.Order,
                    ResearchDate = researchToEdit.ResearchDate,
                    PatientId = researchToEdit.PatientId,
                    Id = Convert.ToInt32(id),
                    Items = _medBookDbContext.Indicators.Where(ind => ind.ResearchId == researchToEdit.Id)
                    .Select(ind => new ResearchVM.Item
                    {
                        IndicatorType = Convert.ToInt32(ind.Type),
                        IndicatorName = ind.Name,
                        IndicatorUnit = ind.Unit,
                        IndicatorValue = ind.Value,
                    }).ToList(),
                };
                return View(researchVM);
            }
            return Content("Research did not found.");
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(ResearchVM model)
        {
            var research = await _medBookDbContext.Researches.FindAsync(model.Id);
            if (research != null)
            {
                research.ResearchDate = model.ResearchDate;
                research.Order = model.Laboratory;
                await _medBookDbContext.SaveChangesAsync();
            };
            return RedirectToAction("ShowDetailes", "Patient", new { id = model.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteIndicatorAsync(int researchId, string indicatorName, double indicatorValue)
        {
            var indicator = _medBookDbContext.Indicators
                .Where(ind => ind.Name == indicatorName)
                .Where(ind => ind.Value == indicatorValue)
                .Where(ind => ind.ResearchId == researchId)
                .First();
            _medBookDbContext.Indicators.Remove(indicator);
            await _medBookDbContext.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = researchId.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteResearchAsync(int researchId, string patientId)
        {
            var researchToDelete = await _medBookDbContext.Researches.FindAsync(researchId);
            if (researchToDelete != null)
            {
                var indicatorsToDelete = await _medBookDbContext.Indicators
                    .Where(ind => ind.ResearchId == researchToDelete.Id)
                    .ToListAsync();
                if (indicatorsToDelete!= null && indicatorsToDelete.Count != 0)
                {
                    _medBookDbContext.Indicators.RemoveRange(indicatorsToDelete);
                }
                _medBookDbContext.Researches.Remove(researchToDelete);
                await _medBookDbContext.SaveChangesAsync(true);
            }
            return RedirectToAction("ShowDetailes", "Patient", new { id = patientId});
        }

        [HttpPost]
        public IActionResult FindIndicatorToEditAsync(string indicatorName)
        {
            var indicatorsList = _medBookDbContext.SampleIndicators
                .Where(ind => ind.Name.ToUpper().Contains(indicatorName.ToUpper()))
                .Select(_mapper.Map<SampleIndicator, IndicatorVM>).ToList();
          
            return PartialView(indicatorsList);
        }

        [HttpPost]
        public async Task<IActionResult> AddIndicatorToEditResearchAsync(string addIndicatorName, string addIndicatorValue, string researchId, string bearingId, string patientId)
        {
            var indValue = double.Parse(addIndicatorValue.Replace('.', ','));
            var bearindInd = await _medBookDbContext.BearingIndicators.FindAsync(int.Parse(bearingId));
            var indicatorToAdd = new Indicator
            {
                Name = addIndicatorName,
                Type = bearindInd.Type,
                Value = indValue,
                Unit = bearindInd.Unit,
                ReferentMax = bearindInd.ReferenceMax,
                ReferentMin = bearindInd.ReferenceMin,
                BearingIndicatorId = bearindInd.Id,
                ResearchId = int.Parse(researchId),
                PatientId = patientId,
            };
            await _medBookDbContext.Indicators.AddAsync(indicatorToAdd);
            await _medBookDbContext.SaveChangesAsync();

            var research = await _medBookDbContext.Researches.FindAsync(int.Parse(researchId));

            ResearchVM researchVM = new ResearchVM
            {
                Laboratory = research.Order,
                ResearchDate = research.ResearchDate,
                PatientId = research.PatientId,
                Id = Convert.ToInt32(research.Id),
                Items = _medBookDbContext.Indicators.Where(ind => ind.ResearchId == research.Id)
                .Select(ind => new ResearchVM.Item
                {
                    IndicatorType = Convert.ToInt32(ind.Type),
                    IndicatorName = ind.Name,
                    IndicatorUnit = ind.Unit,
                    IndicatorValue = ind.Value,
                }).ToList(),
            };

            return PartialView("_IndicatorList", researchVM);
        }
    }
}
