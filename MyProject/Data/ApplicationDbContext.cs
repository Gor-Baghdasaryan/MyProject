using Microsoft.EntityFrameworkCore;
using MyProject.DataModel;
using MyProject.Models;

namespace MyProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                        .HasOne(p => p.User)
                        .WithMany(l => l.Products)
                        .HasForeignKey(p => p.UserId);

        }
    }
}
