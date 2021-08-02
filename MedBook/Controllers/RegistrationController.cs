using MedBook.Models;
using MedBook.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly MedBookDbContext _medBookDbContext;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        
        public RegistrationController(MedBookDbContext medBookDbContext, UserManager<User> userManager,
            SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _medBookDbContext = medBookDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult DoctorRegistration()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DoctorDbSaveAsync(Doctor doctor)
        {

            await _medBookDbContext.Doctors.AddAsync(doctor);
            await _medBookDbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DoctorRegistrationAsync(DoctorRegModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = model.FName + model.LName,
                    Email = model.Email,
                };


                var userCreate = await _userManager.CreateAsync(user, model.Password);
                if (userCreate.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Doctor"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                    };
                    await _userManager.AddToRoleAsync(user, "Doctor");
                };
                _ = await _medBookDbContext.Users.AddAsync(user);
                return RedirectToAction("DoctorDbSave", new Doctor { Id = user.Id, FName = model.FName, LName = model.LName});
            }
            return View();
        }
    }
}
