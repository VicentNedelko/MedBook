using MedBook.Managers.EmailManager;
using MedBook.Models;
using MedBook.Models.Constants;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace MedBook.BaseSettings
{
    public class BaseInitializer
    {
        public static async Task InitializeAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IBaseAdmin baseAdmin,
            BaseDoctor baseDoctor,
            IEmailManager emailManager)
        {
            if (await roleManager.FindByNameAsync("Admin") is null)
            {
                await roleManager.CreateAsync(new IdentityRole(baseAdmin.Role));
            }

            if (await roleManager.FindByNameAsync(baseDoctor.Role) is null)
            {
                await roleManager.CreateAsync(new IdentityRole(baseDoctor.Role));
            }

            if (await userManager.FindByEmailAsync(baseAdmin.Email) is null)
            {
                var admin = new BaseAdmin { Email = baseAdmin.Email, UserName = baseAdmin.Name };
                var result = await userManager.CreateAsync(admin, baseAdmin.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(admin);
                    admin.EmailConfirmationToken = token;
                    await userManager.UpdateAsync(admin);
                    var replacePluses = token.Replace("+", "%2B");
                    var outputToken = replacePluses.Replace("/", "%2F");
                    var confirmationLink = string.Concat(
                        UriConstants.host,
                        UriConstants.controller,
                        UriConstants.action,
                        "token=", outputToken,
                        "&Email=", admin.Email);
                    await emailManager.SendEmailConfirmationLinkAsync(confirmationLink, admin.Email);
                    await userManager.AddToRoleAsync(admin, baseAdmin.Role);
                };
            }

            if (await userManager.FindByEmailAsync(baseDoctor.Email) is null)
            {
                var doctor = new BaseDoctor { Email = baseDoctor.Email, UserName = baseDoctor.UserName };
                var result = await userManager.CreateAsync(doctor, baseDoctor.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(doctor);
                    doctor.EmailConfirmationToken = token;
                    await userManager.UpdateAsync(doctor);
                    var replacePluses = token.Replace("+", "%2B");
                    var outputToken = replacePluses.Replace("/", "%2F");
                    var confirmationLink = string.Concat(
                        UriConstants.host,
                        UriConstants.controller,
                        UriConstants.action,
                        "token=", outputToken,
                        "&Email=", doctor.Email);
                    await emailManager.SendEmailConfirmationLinkAsync(confirmationLink, doctor.Email);
                    await userManager.AddToRoleAsync(doctor, baseDoctor.Role);
                };
            }
        }
    }
}
