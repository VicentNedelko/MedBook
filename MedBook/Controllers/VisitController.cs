using MedBook.Models;
using MedBook.Models.ViewModels;
using MedBook.Services.Visits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
    public class VisitController : Controller
    {
        private readonly MedBookDbContext _medBookDbContext;
        public VisitController(MedBookDbContext medBookDbContext)
        {
            _medBookDbContext = medBookDbContext;
        }



        [HttpGet]
        [Authorize(Roles = "Receptionist, Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IEnumerable<VisitVM> GetVisits([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return _medBookDbContext.Visits
                .Where(v => v.Start >= start && v.End <= end)
                .AsQueryable()
                .Select(v => new VisitVM
                {
                    Id = v.Id,
                    Text = v.Content,
                    Start = v.Start,
                    End = v.End,
                })
                .ToList();
        }

        [HttpGet]
        public IEnumerable<VisitVM> GetWeekVisits()
        {
            return _medBookDbContext.Visits
                .Where(v => v.Start >= VisitService.GetMonday() && v.End <= VisitService.GetSunday())
                .AsQueryable()
                .Select(v => new VisitVM
                {
                    Id = v.Id,
                    Text = v.Content,
                    Start = v.Start,
                    End = v.End,
                }).ToList();
        }

        public async Task<IActionResult> AddNewEventAsync()
        {
            ViewBag.Doctors = await _medBookDbContext.Doctors.AsNoTracking()
                .OrderByDescending(d => d.LName).ToArrayAsync();
            ViewBag.Patients = await _medBookDbContext.Patients.AsNoTracking()
                .OrderByDescending(p => p.LName).ToArrayAsync();
            return View();
        }
    }
}
