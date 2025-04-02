using Microsoft.EntityFrameworkCore;
using PainterPalApi.Models;

namespace PainterPalApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ProductPreferences> ProductPreferences { get; set; }
        public DbSet<Colour> Colours { get; set; }
        public DbSet<JobEmployee> JobEmployees { get; set; }

        public DbSet<Material> Materials { get; set; }
        public DbSet<JobMaterial> JobMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobEmployee>()
                .HasOne(je => je.Job).WithMany(j => j.EmployeeAssignments).HasForeignKey(je => je.JobId);

            modelBuilder.Entity<JobEmployee>().HasOne(je => je.Employee).WithMany(e => e.JobAssignments).HasForeignKey(je => je.EmployeeId);

            modelBuilder.Entity<Job>().HasOne(j => j.Customer).WithMany(c => c.Jobs).HasForeignKey(j => j.CustomerId);

        } 
    }
}
