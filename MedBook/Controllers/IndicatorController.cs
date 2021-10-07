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

        [HttpPost]
        public IActionResult Index(IndicatorVM model)
        {
            return View();
        }
    }
}
