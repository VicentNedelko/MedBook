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

namespace MedBook.Controllers
{
    [Authorize]
    public class ResearchController : Controller
    {
        private readonly MedBookDbContext _medBookDbContext;
        private readonly ResearchManager _researchManager;

        public ResearchController(MedBookDbContext medBookDbContext, ResearchManager researchManager)
        {
            _medBookDbContext = medBookDbContext;
            _researchManager = researchManager;
        }


        [HttpGet]
        public IActionResult ShowResearchData()
        {
            ResearchVM researchVM = new ResearchVM { Items = new List<ResearchVM.Item>() };
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
                    Patient = await _medBookDbContext.Patients.FindAsync(model.PatientId),
                    PatientId = model.PatientId,
                    Indicators = new List<Indicator>(),
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
                    });
                research.Indicators = researchIndicatorsModel.ToList();

                // check if Research is also exist
                var isDuplicated = _researchManager.IsResearchDuplicated(research);
                if (!isDuplicated)
                {
                    var addResult = await _medBookDbContext.Researches.AddAsync(research);
                    if (addResult.State == EntityState.Added)
                    {
                        await _medBookDbContext.SaveChangesAsync();
                        return RedirectToAction("ShowDetailes", "Patient", new { id = model.PatientId });
                    }
                    ViewBag.ErrorMessage = "DB Error! Item didn't save.";
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
                Items = _medBookDbContext.Indicators.Where(ind => ind.ResearchId == research.Id)
                .Select(ind => new ResearchVM.Item
                {
                    IndicatorType = Convert.ToInt32(ind.Type),
                    IndicatorName = ind.Name,
                    IndicatorUnit = ind.Unit,
                    IndicatorValue = ind.Value,
                }).ToList(),
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
            if(research != null)
            {
                research.ResearchDate = model.ResearchDate;
                research.Order = model.Laboratory;
                await _medBookDbContext.SaveChangesAsync();
            };
            return RedirectToAction("ShowDetailes", "Patient", new { id = model.PatientId});
        }
    }
}
