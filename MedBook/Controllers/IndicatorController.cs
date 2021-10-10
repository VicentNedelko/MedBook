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
    [Authorize(Roles = "Doctor")]
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
            var indicatorList = await _medBookDbContext.SampleIndicators.AsNoTracking().ToArrayAsync();
            Array.Sort(indicatorList);
            ViewBag.IndicatorList = indicatorList;
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
                return RedirectToAction("Index", "Indicator");
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
            };
            var addResult = await _medBookDbContext.SampleIndicators.AddAsync(sample);
            if(addResult.State == EntityState.Added)
            {
                _ = await _medBookDbContext.SaveChangesAsync();
            }
            else
            {
                ViewBag.ErrorMessage = $"Indicator hasn't been added - {addResult.State}";
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
    }
}
