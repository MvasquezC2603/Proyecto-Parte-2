using Microsoft.EntityFrameworkCore;
using BasketApi.Models;

namespace BasketApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<MatchRoster> MatchRosters => Set<MatchRoster>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>().HasIndex(u => u.Username).IsUnique();

            // Team ↔ Player (sin cascada)
            mb.Entity<Player>()
              .HasOne(p => p.Team)
              .WithMany(t => t.Players)
              .HasForeignKey(p => p.TeamId)
              .OnDelete(DeleteBehavior.NoAction);

            // Match ↔ Team (sin cascada)
            mb.Entity<Match>()
              .HasOne(m => m.HomeTeam)
              .WithMany(t => t.HomeMatches)
              .HasForeignKey(m => m.HomeTeamId)
              .OnDelete(DeleteBehavior.NoAction);

            mb.Entity<Match>()
              .HasOne(m => m.AwayTeam)
              .WithMany(t => t.AwayMatches)
              .HasForeignKey(m => m.AwayTeamId)
              .OnDelete(DeleteBehavior.NoAction);

            // MatchRoster (sin cascada en todas las FKs)
            mb.Entity<MatchRoster>()
              .HasOne(r => r.Match)
              .WithMany(m => m.Rosters)
              .HasForeignKey(r => r.MatchId)
              .OnDelete(DeleteBehavior.NoAction);

            mb.Entity<MatchRoster>()
              .HasOne(r => r.Team)
              .WithMany() // navegación inversa opcional
              .HasForeignKey(r => r.TeamId)
              .OnDelete(DeleteBehavior.NoAction);

            mb.Entity<MatchRoster>()
              .HasOne(r => r.Player)
              .WithMany() // navegación inversa opcional
              .HasForeignKey(r => r.PlayerId)
              .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(mb);
        }
    }
}
