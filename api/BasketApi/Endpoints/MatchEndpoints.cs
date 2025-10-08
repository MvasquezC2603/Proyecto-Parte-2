using Microsoft.EntityFrameworkCore;
using BasketApi.Data;
using BasketApi.Models;

namespace BasketApi.Endpoints
{
    public static class MatchEndpoints
    {
        public static void MapMatchEndpoints(this WebApplication app)
        {
            // Crear partido
            app.MapPost("/api/matches", async (CreateMatchDto dto, AppDbContext db) =>
            {
                if (dto.HomeTeamId == dto.AwayTeamId)
                    return Results.BadRequest("Los equipos no pueden ser iguales.");

                var match = new Match
                {
                    HomeTeamId = dto.HomeTeamId,
                    AwayTeamId = dto.AwayTeamId,
                    Kickoff = dto.Kickoff
                };

                db.Matches.Add(match);
                await db.SaveChangesAsync();
                return Results.Created($"/api/matches/{match.Id}", match);
            }).RequireAuthorization();

            // Listar partidos (futuros, historial)
            app.MapGet("/api/matches", async (AppDbContext db, bool? finished) =>
            {
                var q = db.Matches
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .AsQueryable();

                if (finished != null)
                    q = q.Where(m => m.Finished == finished);

                return await q.OrderByDescending(m => m.Kickoff).ToListAsync();
            }).RequireAuthorization();

            // Asignar roster
            app.MapPost("/api/matches/{id:int}/roster", async (int id, AssignRosterDto dto, AppDbContext db) =>
            {
                var match = await db.Matches.FindAsync(id);
                if (match is null) return Results.NotFound();

                // eliminar roster previo del mismo partido/equipo y volver a cargar
                var prev = db.MatchRosters.Where(r => r.MatchId == id && r.TeamId == dto.TeamId);
                db.MatchRosters.RemoveRange(prev);
                await db.SaveChangesAsync();

                var newRows = dto.PlayerIds.Select(pid => new MatchRoster
                {
                    MatchId = id,
                    TeamId = dto.TeamId,
                    PlayerId = pid
                }).ToList();

                db.MatchRosters.AddRange(newRows);
                await db.SaveChangesAsync();

                return Results.Ok(new { added = newRows.Count });
            }).RequireAuthorization();

            // Finalizar partido con marcador
            app.MapPost("/api/matches/{id:int}/finish", async (int id, FinishMatchDto dto, AppDbContext db) =>
            {
                var match = await db.Matches.FindAsync(id);
                if (match is null) return Results.NotFound();

                match.HomeScore = dto.HomeScore;
                match.AwayScore = dto.AwayScore;
                match.Finished = true;

                await db.SaveChangesAsync();
                return Results.Ok(match);
            }).RequireAuthorization();
        }
    }

    public record CreateMatchDto(int HomeTeamId, int AwayTeamId, DateTime Kickoff);
    public record AssignRosterDto(int TeamId, int[] PlayerIds);
    public record FinishMatchDto(int HomeScore, int AwayScore);
}
