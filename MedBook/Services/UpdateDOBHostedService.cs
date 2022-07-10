using MedBook.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedBook.Services
{
    public class UpdateAgeHostedService : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public UpdateAgeHostedService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using IServiceScope scope = _provider.CreateScope();
                var scopedProvider = scope.ServiceProvider;
                var context = scope.ServiceProvider
                    .GetRequiredService<MedBookDbContext>();
                UpdateAges(context);
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private void UpdateAges(MedBookDbContext medBookDbContext)
        {
            var patients = medBookDbContext.Patients.ToList();
            var today = DateTime.Now;
            foreach (var p in patients)
            {
                if (p.DateOfBirth.Month == today.Month && p.DateOfBirth.Day == today.Day)
                {
                    p.Age++;
                }
            }
        }
    }
}
