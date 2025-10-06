using BasketApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Match> Matches => Set<Match>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>(e =>
            {
                e.Property(p => p.HomeTeam).HasMaxLength(100).IsRequired();
                e.Property(p => p.AwayTeam).HasMaxLength(100).IsRequired();
                e.Property(p => p.Status).HasMaxLength(20).HasDefaultValue("finished");
            });
        }
    }
}

