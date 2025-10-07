using BasketApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BasketApi.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Match> Matches { get; set; } = default!;
        public DbSet<Team> Teams { get; set; } = default!;
        public DbSet<Player> Players { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // <- necesario para Identity

            modelBuilder.Entity<Match>(e =>
            {
                e.Property(p => p.HomeTeam).HasMaxLength(100).IsRequired();
                e.Property(p => p.AwayTeam).HasMaxLength(100).IsRequired();
                e.Property(p => p.Status).HasMaxLength(20).HasDefaultValue("finished");
            });
        }
    }
}
