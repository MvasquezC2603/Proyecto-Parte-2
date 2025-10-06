using BasketApi.Data;
using BasketApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Servicios
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer"),
        sql => sql.EnableRetryOnFailure()
    ));
builder.Services.AddControllers();

// 2) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3) Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseDefaultFiles();
app.UseStaticFiles();

// 4) Endpoints mínimos

// Listado
app.MapGet("/api/matches", async (AppDbContext db) =>
    await db.Matches.OrderByDescending(m => m.Id).ToListAsync()
);

// Crear
app.MapPost("/api/matches", async (AppDbContext db, Match input) =>
{
    db.Matches.Add(input);
    await db.SaveChangesAsync();
    return Results.Created($"/api/matches/{input.Id}", input);
});

// Update
app.MapPut("/api/matches/{id:int}", async (int id, AppDbContext db, Match input) =>
{
    var match = await db.Matches.FindAsync(id);
    if (match is null) return Results.NotFound();

    match.HomeTeam  = input.HomeTeam;
    match.AwayTeam  = input.AwayTeam;
    match.Quarter   = input.Quarter;
    match.ScoreHome = input.ScoreHome;
    match.ScoreAway = input.ScoreAway;
    match.FoulsHome = input.FoulsHome;
    match.FoulsAway = input.FoulsAway;
    match.StartAt   = input.StartAt;
    match.EndAt     = input.EndAt;
    match.Status    = input.Status;

    await db.SaveChangesAsync();
    return Results.Ok(match);
});

// In-progress (consultar)
app.MapGet("/api/matches/in-progress", async (AppDbContext db) =>
    await db.Matches
            .Where(m => m.Status == "in_progress")
            .OrderByDescending(m => m.Id)
            .FirstOrDefaultAsync()
);

// NUEVO: Start (crear 1 in_progress y limpiar cualquier anterior)
app.MapPost("/api/matches/start", async (AppDbContext db, StartMatchDto dto) =>
{
    // Cerrar cualquier in_progress "huérfano" que haya quedado
    var running = await db.Matches
        .Where(m => m.Status == "in_progress")
        .ToListAsync();

    if (running.Any())
    {
        foreach (var m in running)
        {
            m.Status = "finished";
            m.EndAt  = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();
    }

    var match = new Match
    {
        HomeTeam  = dto.HomeTeam,
        AwayTeam  = dto.AwayTeam,
        Quarter   = 1,
        ScoreHome = 0,
        ScoreAway = 0,
        FoulsHome = 0,
        FoulsAway = 0,
        StartAt   = DateTime.UtcNow,
        EndAt     = null,
        Status    = "in_progress"
    };

    db.Matches.Add(match);
    await db.SaveChangesAsync();

    return Results.Ok(match);
});

// Controllers (si los usas)
app.MapControllers();

// Migraciones al arrancar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();

// DTO para /start
public record StartMatchDto(string HomeTeam, string AwayTeam, int DurationSeconds);
