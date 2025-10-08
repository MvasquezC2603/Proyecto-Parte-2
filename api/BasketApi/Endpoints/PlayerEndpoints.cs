using Microsoft.EntityFrameworkCore;
using BasketApi.Data;
using BasketApi.Models;

namespace BasketApi.Endpoints
{
    public static class PlayerEndpoints
    {
        public static void MapPlayerEndpoints(this WebApplication app)
        {
            // Listado con filtros opcionales: q y teamId
            app.MapGet("/api/players", async (AppDbContext db, string? q, int? teamId) =>
                await db.Players
                    .Include(p => p.Team)
                    .Where(p =>
                        (q == null || p.FullName.Contains(q) || p.Position.Contains(q)) &&
                        (teamId == null || p.TeamId == teamId)
                    )
                    .OrderBy(p => p.FullName)
                    .ToListAsync()
            ).RequireAuthorization();

            app.MapPost("/api/players", async (Player p, AppDbContext db) =>
            {
                db.Players.Add(p);
                await db.SaveChangesAsync();
                return Results.Created($"/api/players/{p.Id}", p);
            }).RequireAuthorization();

            app.MapPut("/api/players/{id:int}", async (int id, Player updated, AppDbContext db) =>
            {
                var p = await db.Players.FindAsync(id);
                if (p is null) return Results.NotFound();

                p.FullName = updated.FullName;
                p.Number = updated.Number;
                p.Position = updated.Position;
                p.Height = updated.Height;
                p.Age = updated.Age;
                p.Nationality = updated.Nationality;
                p.TeamId = updated.TeamId;

                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization();

            app.MapDelete("/api/players/{id:int}", async (int id, AppDbContext db) =>
            {
                var p = await db.Players.FindAsync(id);
                if (p is null) return Results.NotFound();

                db.Players.Remove(p);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
