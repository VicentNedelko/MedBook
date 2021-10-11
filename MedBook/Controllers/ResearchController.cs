using MedBook.Models;
using MedBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
    [Authorize]
    public class ResearchController : Controller
    {
        private readonly MedBookDbContext _medBookDbContext;

        public ResearchController(MedBookDbContext medBookDbContext)
        {
            _medBookDbContext = medBookDbContext;
        }


        [HttpGet]
        public IActionResult ShowResearchData()
        {
            ResearchVM researchVM = new ResearchVM();
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
                    Indicators = new List<Indicator>(),
                };
                var researchIndicatorsModel = model.Items
                    .Select(ind => new Indicator
                    {
                        Name = ind.IndicatorName,
                        Value = ind.IndicatorValue,
                        Unit = ind.IndicatorUnit,
                        Research = research,
                        PatientId = model.PatientId,
                    });
                research.Indicators = researchIndicatorsModel.ToList();

                // check if Research is also exist
                var getDoublesResearch = _medBookDbContext.Researches.FirstOrDefault(res =>
                    res.Order == research.Order &&
                    res.ResearchDate == research.ResearchDate &&
                    res.Patient.Id == research.Patient.Id);
                if(getDoublesResearch is null)
                {
                    var addResult = await _medBookDbContext.Researches.AddAsync(research);
                    if (addResult.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                    {
                        await _medBookDbContext.SaveChangesAsync();
                        return RedirectToAction("ShowDetailes", "Patient", new { id = model.PatientId });
                    }
                    ViewBag.ErrorMessage = "DB Error! Item didn't save.";
                }
                ViewBag.ErrorMessage = "Исследование с такой же датой и наименованием лаборатории уже внесено в базу.";
                return View("Error");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResearchDetailesAsync(string id)
        {
            var research = await _medBookDbContext.Researches.FindAsync(Convert.ToInt32(id));
            if(research == null)
            {
                ViewBag.Error = "Данные не найдены.";
                return View(research);
            }
            var researchVM = new ResearchVM
            {
                Laboratory = research.Order,
                ResearchDate = research.ResearchDate,

                Items = _medBookDbContext.Indicators.Where(ind => ind.ResearchId == research.Id)
                .Select(ind => new ResearchVM.Item
                {
                    IndicatorName = ind.Name,
                    IndicatorUnit = ind.Unit,
                    IndicatorValue = ind.Value,
                }).ToList(),
            };
            var patient = await _medBookDbContext.Patients.FindAsync(research.PatientId);
            ViewBag.PatientLname = patient.LName;
            ViewBag.PatientFname = patient.FName;
            ViewBag.PatientAge = patient.Age;

            return View(researchVM);
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
