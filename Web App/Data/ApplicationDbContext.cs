using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web_App.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Credentials> Credentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // You can globally assign schema here
            modelBuilder.HasDefaultSchema("app");

            modelBuilder.Entity<Credentials>(x =>
            {
                x.ToTable("Security", "security");
                x.HasKey(x => x.CredentialId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
