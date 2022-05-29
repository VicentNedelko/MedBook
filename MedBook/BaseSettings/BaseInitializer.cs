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
            IEmailManager emailManager)
        {
            if (await roleManager.FindByNameAsync("Admin") is null)
            {
                await roleManager.CreateAsync(new IdentityRole(baseAdmin.Role));
            }
            if (await userManager.FindByEmailAsync(baseAdmin.Email) is null)
            {
                var admin = new User { Email = baseAdmin.Email, UserName = baseAdmin.Name };
                var result = await userManager.CreateAsync(admin, baseAdmin.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(admin);
                    var replacePluses = token.Replace("+", "%2B");
                    var outputToken = replacePluses.Replace("/", "%2F");
                    var confirmationLink = string.Concat(UriConstants.host,
                        UriConstants.controller,
                        UriConstants.action,
                        "token=", outputToken,
                        "&Email=", admin.Email);
                    await emailManager.SendEmailConfirmationLinkAsync(confirmationLink, admin.Email);
                    await userManager.AddToRoleAsync(admin, baseAdmin.Role);
                };
            }
        }
    }
}
