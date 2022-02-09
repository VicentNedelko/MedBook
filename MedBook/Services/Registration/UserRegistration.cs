using MedBook.Models;
using MedBook.Models.ViewModels;
using MedBook.Services.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Services.Registration
{
    public class UserRegistration : IUserRegistration
    {
        private readonly MedBookDbContext _medBookDbContext;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        public UserRegistration(MedBookDbContext medBookDbContext, SignInManager<User> signInManager, 
            RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _medBookDbContext = medBookDbContext;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<ServiceResult> ReceptionistRegistrationAsync(ReceptionistRegModel model)
        {
            User user = new User
            {
                UserName = model.FName + model.LName,
                Email = model.Email
            };

            var createUser = await _userManager.CreateAsync(user, model.Password);
            if (createUser.Succeeded)
            {
                if(!await _roleManager.RoleExistsAsync("Receptionist"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Receptionist"));
                }
                await _userManager.AddToRoleAsync(user, "Receptionist");
                await _medBookDbContext.Users.AddAsync(user);
                await _medBookDbContext.SaveChangesAsync();
                return ServiceResult.OK;
            }
            return ServiceResult.FAILED_DB;
        }
    }
}
