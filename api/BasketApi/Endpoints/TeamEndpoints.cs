using Microsoft.EntityFrameworkCore;
using BasketApi.Data;
using BasketApi.Models;

namespace BasketApi.Endpoints
{
    public static class TeamEndpoints
    {
        public static void MapTeamEndpoints(this WebApplication app)
        {
            // Obtener lista de equipos (con búsqueda)
            app.MapGet("/api/teams", async (AppDbContext db, string? q) =>
                await db.Teams
                    .Where(t => q == null || t.Name.Contains(q) || t.City.Contains(q))
                    .OrderBy(t => t.Name)
                    .ToListAsync()
            )
            .RequireAuthorization(); // Solo usuarios autenticados

            // Crear nuevo equipo
            app.MapPost("/api/teams", async (Team team, AppDbContext db) =>
            {
                db.Teams.Add(team);
                await db.SaveChangesAsync();
                return Results.Created($"/api/teams/{team.Id}", team);
            })
            .RequireAuthorization();

            // Editar equipo existente
            app.MapPut("/api/teams/{id:int}", async (int id, Team updated, AppDbContext db) =>
            {
                var team = await db.Teams.FindAsync(id);
                if (team == null)
                    return Results.NotFound();

                team.Name = updated.Name;
                team.City = updated.City;
                team.LogoUrl = updated.LogoUrl;
                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .RequireAuthorization();

            // Eliminar equipo
            app.MapDelete("/api/teams/{id:int}", async (int id, AppDbContext db) =>
            {
                var team = await db.Teams.FindAsync(id);
                if (team == null)
                    return Results.NotFound();

                db.Teams.Remove(team);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .RequireAuthorization();
        }
    }
}
