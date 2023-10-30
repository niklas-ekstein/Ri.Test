using Ri.Interview.Models;
using Microsoft.EntityFrameworkCore;

namespace Ri.Interview.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasKey(p => p.Id); 
            
            modelBuilder.Entity<Project>().OwnsOne(p => p.Settings);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.LogTo(Console.WriteLine);
            
            if (!optionsBuilder.IsConfigured)
            {
                // Should not be hard coded.. change
                optionsBuilder.UseSqlServer("Server=localhost; Database=db3; User Id=sa; Password=your_password123;Trusted_Connection=False;Encrypt=False;",
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            }
        }
    }
}