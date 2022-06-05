using EmailService;
using EmailService.Interfaces;
using MedBook.EmailTokenServiceProvider;
using MedBook.Managers.EmailManager;
using MedBook.Managers.ResearchesManager;
using MedBook.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;

namespace MedBook
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IConfiguration BaseDbConfiguration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                            .AddJsonFile("adminConfiguration.json");
            BaseDbConfiguration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MedBookDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("MedBookConnection")));
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.SignIn.RequireConfirmedEmail = true;
                opts.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            })
                .AddEntityFrameworkStores<MedBookDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");

            services.Configure<DataProtectionTokenProviderOptions>(opts =>
                                    opts.TokenLifespan = TimeSpan.FromHours(5));
            services.Configure<EmailConfirmationTokenProviderOptions>(opts =>
                                    opts.TokenLifespan = TimeSpan.FromDays(100));

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Registration/Login";
                options.AccessDeniedPath = "/Registration/Login";
            });

            services.AddScoped<ResearchManager>();
            var emailConfiguration = Configuration
                            .GetSection("EmailConfiguration")
                            .Get<EmailConfiguration>();
            var baseAdmin = BaseDbConfiguration.Get<BaseAdmin>();
            services.AddSingleton<IEmailConfiguration>(emailConfiguration);
            services.AddSingleton<IBaseAdmin>(baseAdmin);
            services.AddScoped<IEmailService, EmailSender>();
            services.AddScoped<IEmailManager, EmailManager>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var cultureInfo = new CultureInfo("ru-RU");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
