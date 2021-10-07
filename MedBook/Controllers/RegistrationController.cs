using MedBook.Models;
using MedBook.Models.Enums;
using MedBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
    [Authorize]
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

        /// <summary>
        /// Add new User with Doctor role
        /// </summary>
        /// <returns></returns>


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
                }
                else
                {
                    return Content($"{userCreate.Errors}");
                }
                _ = await _medBookDbContext.Users.AddAsync(user);
                return RedirectToAction("DoctorDbSave", new Doctor { Id = user.Id, FName = model.FName, LName = model.LName});
            }
            return View();
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        /// 

        [HttpGet]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if(user == null)
            {
                ViewBag.ErrorMessage = "Email or Password is incorrect. Try again.";
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
            if (signInResult.Succeeded)
            {
                if ((await _userManager.GetRolesAsync(user)).Contains("Doctor"))
                {
                    return RedirectToAction("ShowMyPatients", "Patient");
                }
                else if ((await _userManager.GetRolesAsync(user)).Contains("Patient"))
                {
                    var currentUserId = await _userManager.GetUserIdAsync(user);
                    return RedirectToAction("ShowDetailes", "Patient", new { id = currentUserId });
                }
                
            }
            ViewBag.ErrorMessage = $"{signInResult}";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                _signInManager.SignOutAsync();
                HttpContext.Session.Clear();
            }
            return RedirectToAction("Index", "Home");
        }

        ///<summary>
        /// Admin registration
        /// </summary>
        /// 
        [HttpGet]
        public IActionResult AdminRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminRegistrationAsync(AdminRegModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = model.Name,
                    Email = model.Email,
                };


                var userCreate = await _userManager.CreateAsync(user, model.Password);
                if (userCreate.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    };
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    return Content($"{userCreate.Errors}");
                }
                _ = await _medBookDbContext.Users.AddAsync(user);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        ///<summary>
        /// Patient registration
        /// </summary>
        /// 
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public IActionResult PatientRegistration()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> PatientRegistrationAsync(PatientRegModel model)
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
                    if (!await _roleManager.RoleExistsAsync("Patient"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Patient"));
                    };
                    await _userManager.AddToRoleAsync(user, "Patient");
                }
                else
                {
                    return Content($"{userCreate.Errors}");
                }
                _ = await _medBookDbContext.Users.AddAsync(user);

                Patient patient = new Patient
                {
                    Id = user.Id,
                    Email = user.Email,
                    FName = model.FName,
                    LName = model.LName,
                    Age = model.Age,
                    Gender = GenderStrToEnum(model.Gender),
                    Diagnosis = String.Empty,
                    Doctor = null,
                };
                return RedirectToAction("PatientDbSave", patient);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PatientDbSaveAsync(Patient patient)
        {
            patient.Doctor = await _medBookDbContext.Doctors.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _medBookDbContext.Patients.AddAsync(patient);
            await _medBookDbContext.SaveChangesAsync();
            return RedirectToAction("PatientRegistration", "Registration");
        }






        public static Gender GenderStrToEnum(string gender)
        {
            return gender switch
            {
                "male" => Gender.MALE,
                "female" => Gender.FEMALE,
                _ => Gender.UNKNOWN
            };
        }

    }
}
