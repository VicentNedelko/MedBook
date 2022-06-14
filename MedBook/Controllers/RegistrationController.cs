using EmailService;
using MedBook.Managers.EmailManager;
using MedBook.Models;
using MedBook.Models.Enums;
using MedBook.Models.ViewModels;
using MedBook.Models.ViewModels.EmailServiceVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailManager _emailManager;

        public RegistrationController(
            MedBookDbContext medBookDbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment,
            IEmailManager emailManager)
        {
            _medBookDbContext = medBookDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _emailManager = emailManager;
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


        [HttpPost]
        public async Task<IActionResult> DoctorRegistrationAsync(DoctorRegModel model)
        {
            if (ModelState.IsValid)
            {
                var doctor = new Doctor
                {
                    FName = model.FName,
                    LName = model.LName,
                    Email = model.Email,
                    IsBlock = false,
                    UserName = model.FName + model.LName,
                };
                await _userManager.CreateAsync(doctor, model.Password);

                if (!await _roleManager.RoleExistsAsync("Doctor"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                };
                var emailStatus = await SendRegistrationConfirmationAsync(doctor);
                if (emailStatus == EmailStatus.SEND)
                {
                    await _userManager.AddToRoleAsync(doctor, "Doctor");
                }
                else
                {
                    ViewBag.ErrorMessage = "Неверный Email. Проверьте правильность написания.";
                    return View("Error");
                }
                await _medBookDbContext.SaveChangesAsync();
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
            if (user == null || user.IsBlock)
            {
                ViewBag.ErrorMessage = "Email или пароль неверны. Попробуйте ещё раз.";
                return View();
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                var isTokenValid = await _userManager.VerifyUserTokenAsync(
                    user,
                    _userManager.Options.Tokens.EmailConfirmationTokenProvider,
                    UserManager<User>.ConfirmEmailTokenPurpose,
                    user.EmailConfirmationToken);
                if (isTokenValid)
                {
                    ViewBag.ErrorMessage = "Авторизация не подтверждена через Email. Проверьте Email.";
                    return View("Error");
                }
                else
                {
                    var userToDelete = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.DeleteAsync(userToDelete);
                    ViewBag.ErrorMessage = "Авторизация не была завершена. Пройдите процедуру снова.";
                    return View("Error");
                };
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
                else if ((await _userManager.GetRolesAsync(user)).Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            var userDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", $"{User.Identity.Name}");
            if (Directory.Exists(userDir))
            {
                var dir = new DirectoryInfo(userDir);
                dir.Delete(true);
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
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", $"{User.Identity.Name}");
            if (Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                dir.Delete(true);
            }
            return RedirectToAction("Index", "Home");
        }

        ///<summary>
        /// Admin registration
        /// </summary>
        /// 
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminRegistration()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminRegistrationAsync(AdminRegModel model)
        {
            if (ModelState.IsValid)
            {
                BaseAdmin admin = new BaseAdmin
                {
                    UserName = model.Name,
                    Email = model.Email,
                    IsBlock = false,
                };


                var adminCreate = await _userManager.CreateAsync(admin, model.Password);
                if (adminCreate.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    };
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    return Content($"{adminCreate.Errors}");
                }
                _ = await _medBookDbContext.Users.AddAsync(admin);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        ///<summary>
        /// Patient registration
        /// </summary>
        /// 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult PatientRegistration()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SuccessRegistration()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PatientRegistrationAsync(PatientRegModel model)
        {
            if (ModelState.IsValid)
            {
                Patient patient = new Patient
                {
                    FName = model.FName,
                    LName = model.LName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = GenderStrToEnum(model.Gender),
                    Diagnosis = String.Empty,
                };

                var userCreate = await _userManager.CreateAsync(patient, model.Password);
                if (userCreate.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Patient"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Patient"));
                    };
                }
                else
                {
                    string err = string.Empty;
                    foreach (var e in userCreate.Errors)
                    {
                        err = string.Concat(e.Description, "; ", err);
                    }
                    TempData["error"] = err;
                    return RedirectToAction("Error", "Home");
                }

                // send Email confirmation link
                var emailStatus = await SendRegistrationConfirmationAsync(patient);
                if (emailStatus == EmailStatus.SEND)
                {
                    await _userManager.AddToRoleAsync(patient, "Patient");
                }
                else
                {
                    var userToDelete = await _userManager.FindByEmailAsync(patient.Email);
                    await _userManager.DeleteAsync(userToDelete);
                    ViewBag.ErrorMessage = new List<string> { "Неверный Email. Проверьте правильность написания и повторите попытку регистрации." };
                    return View("Error");
                }

                // current userId (aka DoctorId)
                string docId;
                if (User.Identity.IsAuthenticated && !User.IsInRole("Admin"))
                {
                    docId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                else
                {
                    docId = _medBookDbContext.Doctors.FirstOrDefault(d => d.FName == "Default" && d.LName == "Doctor").Id;
                }
                patient.DoctorId = docId;
                var currentDate = DateTime.Today;
                patient.Age = (int)currentDate.Subtract(patient.DateOfBirth).TotalDays / 365;
                return RedirectToAction("PatientDbSave", patient);
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, string Email)
        {
            var isUserExist = await _userManager.FindByEmailAsync(Email);
            if (isUserExist == null)
            {
                ViewBag.ErrorMessage = $"User with email = {Email} didn't find in DB.";
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(isUserExist, token);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = result.Errors;
                return View("Error");
            }
            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PatientDbSaveAsync(Patient patient)
        {
            await _medBookDbContext.Patients.AddAsync(patient);
            try
            {
                await _medBookDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                ViewBag.ErrorMessage = e.Message;
                return RedirectToAction("Error");
            }

            return RedirectToAction("SuccessRegistration");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetEmailPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && user.EmailConfirmed)
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Registration", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    var confirmationLink = $"Для сброса пароля перейдите по ссылке : <a href = {callbackUrl}>link<a>";
                    var emailMessage = new EmailMessage
                    {
                        ToAddresses = new List<EmailAddress> { new EmailAddress() { Address = user.Email } },
                        Content = confirmationLink,
                        Subject = "Reset password confirmation",
                    };
                    await _emailManager.SendAsync(emailMessage);

                    return View("ForgetPasswordConfirmation");
                }
            }
            ViewBag.ErrorMessage = $"Такой Email - {model.Email} - нам неизвестен. Его нет в базе. Возможно, Вы не регистрировались.";

            return View("Error");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (resetResult.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }

            foreach (var error in resetResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }


        private static Gender GenderStrToEnum(string gender)
        {
            return gender switch
            {
                "male" => Gender.MALE,
                "female" => Gender.FEMALE,
                _ => Gender.UNKNOWN
            };
        }

        private async Task<EmailStatus> SendRegistrationConfirmationAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail),
                                    "Registration", new { token, user.Email },
                                    protocol: HttpContext.Request.Scheme);
            try
            {
                await _emailManager.SendEmailConfirmationLinkAsync(confirmationLink, user.Email);
                var currentUser = await _userManager.FindByEmailAsync(user.Email);
                currentUser.EmailConfirmationToken = token;
                await _userManager.UpdateAsync(currentUser);
                return EmailStatus.SEND;
            }
            catch (WrongEmailException)
            {
                await DeleteUserAsync(user);
                return EmailStatus.ERROR;
            }
        }

        private async Task DeleteUserAsync(User user)
        {
            var userToDelete = await _userManager.FindByIdAsync(user.Id);
            await _userManager.DeleteAsync(userToDelete);
        }
    }
}
