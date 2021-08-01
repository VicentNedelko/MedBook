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
        private readonly RoleManager<User> _roleManager;
        private readonly UserManager<User> _userManager;
        
        public RegistrationController(MedBookDbContext medBookDbContext, UserManager<User> userManager,
            SignInManager<User> signInManager, RoleManager<User> roleManager)
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

        [HttpPost]
        public IActionResult DoctorRegistration(DoctorRegModel model)
        {
            return View();
        }
    }
}
