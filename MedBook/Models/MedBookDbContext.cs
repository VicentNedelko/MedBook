using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedBook.Models
{
    public class MedBookDbContext : IdentityDbContext<User>
    {
        public MedBookDbContext(DbContextOptions<MedBookDbContext> options) : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Research> Researches { get; set; }

        public DbSet<Prescription> Prescriptions { get; set; }

        public DbSet<Cure> Cures { get; set; }

        public DbSet<Visit> Visits { get; set; }

        public DbSet<Indicator> Indicators { get; set; }

        public DbSet<SampleIndicator> SampleIndicators { get; set; }

        public DbSet<BearingIndicator> BearingIndicators { get; set; }
    }
}
