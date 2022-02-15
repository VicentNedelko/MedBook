using MedBook.Models;
using MedBook.Models.ViewModels;
using MedBook.Services.Visits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            ViewBag.WeekVisits = GetWeekVisits();
            return View();
        }

        [HttpGet]
        public JsonResult GetVisits([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var response = _medBookDbContext.Visits
                .Where(v => v.Start >= start && v.End <= end)
                .Select(v => new VisitVM
                {
                    Id = v.Id,
                    Text = v.Content,
                    Start = v.Start,
                    End = v.End,
                })
                .ToArray();
            return Json(response);
        }

        public IEnumerable<VisitVM> GetWeekVisits()
        {
            return  _medBookDbContext.Visits
                .Where(v => v.Start >= VisitService.GetMonday() && v.End <= VisitService.GetSunday())
                .Select(v => new VisitVM
                {
                    Id = v.Id,
                    Text = v.Content,
                    Start = v.Start,
                    End = v.End,
                }).ToArray();
        }

        [HttpGet]
        public async Task<IActionResult> AddNewEventAsync()
        {
            ViewBag.Doctors = await _medBookDbContext.Doctors.AsNoTracking()
                .OrderByDescending(d => d.LName).ToArrayAsync();
            ViewBag.Patients = await _medBookDbContext.Patients.AsNoTracking()
                .OrderByDescending(p => p.LName).ToArrayAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEventAsync(Visit model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Visit visit = new Visit
            {
                Content = "Empty",
                Start = model.Start,
                End = model.End,
                Status = Models.Enums.VisitStatus.RESERVED,
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                Patient = _medBookDbContext.Patients.FirstOrDefault(p => p.Id == model.PatientId),
                Doctor = _medBookDbContext.Doctors.FirstOrDefault(d => d.Id == model.DoctorId),
            };
            await _medBookDbContext.Visits.AddAsync(visit);
            await _medBookDbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Visit");
        }
    }
}
