using AdminGateway.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminGateway.Infrastructure
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options) { }

        public DbSet<User>? Users { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<RefreshToken>()
                .HasKey(r => r.Id);
        }
    }
}
