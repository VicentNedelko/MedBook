using MedBook.Models;
using MedBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
    [Authorize(Roles = "Doctor, Admin")]
    public class IndicatorController : Controller
    {
        private readonly MedBookDbContext _medBookDbContext;

        public IndicatorController(MedBookDbContext medBookDbContext)
        {
            _medBookDbContext = medBookDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var sampleIndList = await _medBookDbContext.SampleIndicators.AsNoTracking().ToArrayAsync();
            var bearingIndList = await _medBookDbContext.BearingIndicators.AsNoTracking().ToArrayAsync();
            Array.Sort(sampleIndList);
            Array.Sort(bearingIndList);
            ViewBag.SampleIndicatorList = sampleIndList;
            ViewBag.BearingIndicatorList = bearingIndList;
            return View();
        }

        //[HttpPost]
        //public IActionResult Index(IndicatorVM model)
        //{
        //    return View();
        //}

        // Add new Sample Indicator

        [HttpPost]
        public async Task<IActionResult> AddIndicatorAsync(IndicatorVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = $"Model State - {ModelState.ValidationState}";
                return View("Error", "Indicator");
            }
            var existingIndicatorsList = await _medBookDbContext.SampleIndicators.ToArrayAsync();
            foreach(var ind in existingIndicatorsList)
            {
                if(model.Name.ToUpper() == ind.Name.ToUpper())
                {
                    ViewBag.ErrorMessage = $"Индикатор уже существует. В базу внесен такой --> {ind.Name} {ind.Unit}, Referent (max) = {ind.ReferenceMax} ({ind.Unit}) - Reference (min) = {ind.ReferenceMin} ({ind.Unit})";
                    return View("Error");
                }
            }
            var sample = new SampleIndicator
            {
                Name = model.Name,
                Unit = model.Unit,
                ReferenceMax = model.ReferentMax,
                ReferenceMin = model.ReferentMin,
                BearingIndicatorId = model.BearingIndicatorId,
                BearingIndicator = await _medBookDbContext.BearingIndicators.FindAsync(model.BearingIndicatorId),
            };
            var addResult = await _medBookDbContext.SampleIndicators.AddAsync(sample);
            if(addResult.State == EntityState.Added)
            {
                _ = await _medBookDbContext.SaveChangesAsync();
            }
            else
            {
                ViewBag.ErrorMessage = $"Indicator hasn't been added - {addResult.State}";
                return View("Error");
            }
            return RedirectToAction("Index", "Indicator");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var result = Int32.TryParse(id, out int sampleId);
            if (!result)
            {
                ViewBag.ErrorMessage = $"TryParse Error! ID - {id}";
                return View("Error");
            }
            var editSample = _medBookDbContext.SampleIndicators.Find(sampleId);
            if(editSample is null)
            {
                ViewBag.ErrorMessage = $"Can't find Sample with ID = {sampleId}";
                return View("Error");
            }
            var editSampleVM = new IndicatorVM
            {
                Id = editSample.Id,
                Name = editSample.Name,
                Unit = editSample.Unit,
                ReferentMax = editSample.ReferenceMax,
                ReferentMin = editSample.ReferenceMin,
            };
            return View(editSampleVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(IndicatorVM model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var editedSample = await _medBookDbContext.SampleIndicators.FindAsync(model.Id);
            if(editedSample is null)
            {
                ViewBag.ErrorMessage = $"Индикатор не найден ID - {model.Id}";
                return View("Error");
            }
            editedSample.Name = model.Name;
            editedSample.Unit = model.Unit;
            editedSample.ReferenceMax = model.ReferentMax;
            editedSample.ReferenceMin = model.ReferentMin;
            await _medBookDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> RemoveAsync(string id)
        {
            bool result = Int32.TryParse(id, out int sampleId);
            if (!result)
            {
                ViewBag.ErrorMessage = $"DB Error! Невозможно найти индикатор ID = {id}";
                return View("Error");
            }
            var sampleToRemove = await _medBookDbContext.SampleIndicators.FindAsync(sampleId);
            if(!(sampleToRemove is null))
            {
                IndicatorVM indicatorVM = new IndicatorVM
                {
                    Id = sampleToRemove.Id,
                    Name = sampleToRemove.Name,
                    Unit = sampleToRemove.Unit,
                    ReferentMax = sampleToRemove.ReferenceMax,
                    ReferentMin = sampleToRemove.ReferenceMin,
                };
                return View(indicatorVM);
            }
            ViewBag.ErrorMessage = $"DB Error! Индикатор ID = {id} NULL";
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAsync(IndicatorVM model)
        {
            var indicatorToRemove = await _medBookDbContext.SampleIndicators.FindAsync(model.Id);
            if(!(indicatorToRemove is null))
            {
                _medBookDbContext.SampleIndicators.Remove(indicatorToRemove);
                await _medBookDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = $"Indicator didn't find ID - {model.Id}";
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> FindIndicatorAsync(string inputIndicator)
        {
            var indicator = await _medBookDbContext.SampleIndicators.Where(ind => ind.Name.ToUpper().StartsWith(inputIndicator.ToUpper()))
                .Select(ind => new IndicatorVM {
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
        public async Task<IActionResult> AddNewBearingAsync()
        {
            ViewBag.BearingList = await _medBookDbContext.BearingIndicators.AsNoTracking().OrderByDescending(bi => bi.Name).ToArrayAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewBearingAsync(BearingIndVM model)
        {
            if (ModelState.IsValid)
            {
                BearingIndicator bearingIndicator = new BearingIndicator
                {
                    Name = model.Name,
                    Type = model.Type,
                    Description = model.Description ?? "Описание отсутствует",
                    ReferenceMax = model.ReferenceMax ?? -1,
                    ReferenceMin = model.ReferenceMin ?? -1,
                    Unit = model.Unit ?? "ед. изм.",
                };

                await _medBookDbContext.BearingIndicators.AddAsync(bearingIndicator);
                await _medBookDbContext.SaveChangesAsync();
            }
            
            ViewBag.BearingList = await _medBookDbContext.BearingIndicators.AsNoTracking().OrderBy(bi => bi.Name).ToArrayAsync();
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> EditBearingAsync(int id)
        {
            var bearing = await _medBookDbContext.BearingIndicators.FindAsync(id);
            BearingIndVM bearingIndVM = new BearingIndVM
            {
                Id = bearing.Id,
                Name = bearing.Name,
                Type = bearing.Type,
                Description = bearing.Description,
                ReferenceMin = bearing.ReferenceMin,
                ReferenceMax = bearing.ReferenceMax,
                Unit = bearing.Unit,
            };
            return View(bearingIndVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditBearingAsync(BearingIndVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "BearingIndicator Model is NOT valid.";
                return View("Error");
            }
            var bearInd = await _medBookDbContext.BearingIndicators.FindAsync(model.Id);

            if(bearInd == null)
            {
                ViewBag.ErrorMessage = $"Bearing Indicator with ID = {model.Id} NOT found.";
                return View("Error");
            }
            bearInd.Name = model.Name;
            bearInd.Type = model.Type;
            bearInd.Description = model.Description;
            bearInd.ReferenceMin = model.ReferenceMin;
            bearInd.ReferenceMax = model.ReferenceMax;
            bearInd.Unit = model.Unit;

            // Change all dependent Indicator Names

            var dependentIndicators = await _medBookDbContext.Indicators
                .Where(di => di.BearingIndicatorId == bearInd.Id)
                .ToArrayAsync();
            if(dependentIndicators.Length != 0)
            {
                foreach(var di in dependentIndicators)
                {
                    di.Name = bearInd.Name;
                }
            }

            try
            {
                await _medBookDbContext.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                ViewBag.ErrorMessage = $"Error DB - {e.Message}";
                return View("Error");
            }
            return RedirectToAction("AddNewBearing");
        }
    }
}
