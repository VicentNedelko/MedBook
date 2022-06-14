using MedBook.BaseSettings;
using MedBook.Managers.EmailManager;
using MedBook.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace MedBook
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var baseAdmin = services.GetRequiredService<BaseAdmin>();
                var baseDoctor = services.GetRequiredService<BaseDoctor>();
                var emailManager = services.GetRequiredService<IEmailManager>();
                await BaseInitializer
                    .InitializeAsync(userManager, roleManager, baseAdmin, baseDoctor, emailManager);
            }
            catch (Exception e) { };

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:5100");
                });
    }
}
