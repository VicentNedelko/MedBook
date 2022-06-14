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
    public class DoctorController : Controller
    {
        private readonly MedBookDbContext _medBookDbContext;
        private readonly UserManager<User> _userManager;

        public DoctorController(MedBookDbContext medBookDbContext)
        {
            _medBookDbContext = medBookDbContext;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ShowAllAsync()
        {
            List<DoctorVM> doctorVMs = new List<DoctorVM>();
            var docs = await _medBookDbContext.Doctors.AsNoTracking().ToArrayAsync();
            foreach(var d in docs)
            {
                doctorVMs.Add(new DoctorVM { FName = d.FName, LName = d.LName, Id = d.Id });
            }
            if(docs.Length == 0)
            {
                ViewBag.Error = "Список докторов пуст.";
            }
            return View(doctorVMs);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAsync(string id)
        {
            var doc = await _medBookDbContext.Doctors.FindAsync(id);
            DoctorVM doctorVM = new DoctorVM
            {
                Id = doc.Id,
                FName = doc.FName,
                LName = doc.LName,
                IsBlock = doc.IsBlock,
            };
            return View(doctorVM);
        }

        public async Task<IActionResult> EditAsync(DoctorVM model)
        {
            var newDoc = await _medBookDbContext.Doctors.FindAsync(model.Id);
            newDoc.FName = model.FName;
            newDoc.LName = model.LName;
            newDoc.IsBlock = model.IsBlock;
            await _medBookDbContext.SaveChangesAsync();
            return RedirectToAction("ShowAll");
        }

        public IActionResult Remove(string id)
        {
            return View();
        }
    }
}
