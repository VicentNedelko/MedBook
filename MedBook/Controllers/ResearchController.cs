using MedBook.Models;
using MedBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
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
        public IActionResult ShowResearchData(ResearchVM model)
        {
            if (ModelState.IsValid)
            {
                var research = new Research
                {
                    Order = model.Laboratory,
                    ResearchDate = model.ResearchDate,
                    Patient = null, // add Patient
                    Indicators = new List<Indicator>(),
                };
                var researchIndicators = model.Items
                    .Select(ind => new
                    {
                        Name = ind.IndicatorName,
                        Value = ind.IndicatorValue,
                        Unit = ind.IndicatorUnit,
                    })
                    .Cast<Indicator>();
            }

            return View();
        }
    }
}
