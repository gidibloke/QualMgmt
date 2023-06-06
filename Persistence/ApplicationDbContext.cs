using Domain.LookupModels;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppUser,AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        //Another EFCore convention. You can remove the properties in OnModelCreating by setting Table name to display name
        public DbSet<CareHome> CareHomes { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffQualification> StaffQualifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CareHome>(e =>
            {
                e.HasIndex(x => x.HomeName).IsUnique();
            });
            builder.Entity<Qualification>(e =>
            {
                e.HasIndex(x => x.Description).IsUnique();
            });
            builder.Entity<Staff>().HasMany(x => x.StaffQualifications).WithOne(o => o.Staff).OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
        }

    }
}
