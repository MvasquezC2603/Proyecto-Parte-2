using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BasketApi.Data;
using BasketApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// -------------------- DB --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer"),
        sql => sql.EnableRetryOnFailure()
    ));

// -------------------- Identity --------------------
// ApplicationUser = tu clase que hereda de IdentityUser
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

builder.Services.AddControllers();

// -------------------- Swagger --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    // Soporte para JWT en Swagger
    o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BasketApi", Version = "v1" });
    var scheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce: Bearer {tu token JWT}"
    };
    o.AddSecurityDefinition("Bearer", scheme);
    o.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { scheme, new List<string>() }
    });
});

// -------------------- CORS --------------------
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});

// -------------------- JWT --------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Jwt:Key no puede ser null/vacío");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// -------------------- Pipeline --------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// -------------------- AUTH ENDPOINTS --------------------

// Registro (opcional). Crea un usuario Identity.
app.MapPost("/api/auth/register", async (
    UserManager<ApplicationUser> userManager,
    RegisterDto dto) =>
{
    var user = new ApplicationUser
    {
        UserName = dto.Username,
        Email = dto.Email
    };
    var result = await userManager.CreateAsync(user, dto.Password);
    if (!result.Succeeded)
        return Results.BadRequest(result.Errors.Select(e => e.Description));

    return Results.Ok(new { message = "Usuario creado" });
});

// Login: genera JWT usando Identity
app.MapPost("/api/auth/login", async (
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    LoginDto dto) =>
{
    var user = await userManager.FindByNameAsync(dto.Username)
               ?? await userManager.FindByEmailAsync(dto.Username);
    if (user is null) return Results.Unauthorized();

    var ok = await userManager.CheckPasswordAsync(user, dto.Password);
    if (!ok) return Results.Unauthorized();

    // Claims básicos + roles
    var roles = await userManager.GetRolesAsync(user);
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName ?? "")
    };
    claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSection["ExpiresMinutes"] ?? "120"));

    var token = new JwtSecurityToken(
        issuer: jwtSection["Issuer"],
        audience: jwtSection["Audience"],
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new
    {
        token = jwt,
        username = user.UserName,
        roles
    });
});

// -------------------- MATCHES (protegidos) --------------------
app.MapGet("/api/matches", async (AppDbContext db) =>
    await db.Matches.OrderByDescending(m => m.Id).ToListAsync()
).RequireAuthorization();

app.MapPost("/api/matches", async (AppDbContext db, Match input) =>
{
    db.Matches.Add(input);
    await db.SaveChangesAsync();
    return Results.Created($"/api/matches/{input.Id}", input);
}).RequireAuthorization();

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
}).RequireAuthorization();

app.MapGet("/api/matches/in-progress", async (AppDbContext db) =>
    await db.Matches.Where(m => m.Status == "in_progress")
                    .OrderByDescending(m => m.Id)
                    .FirstOrDefaultAsync()
).RequireAuthorization();

app.MapPost("/api/matches/start", async (AppDbContext db, StartMatchDto dto) =>
{
    var running = await db.Matches.Where(m => m.Status == "in_progress").ToListAsync();
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
}).RequireAuthorization();

// Controllers (si los tienes)
app.MapControllers();

// -------------------- Migraciones + Seed Identity --------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Crea rol Admin si no existe
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // Crea usuario admin si no existe
    var admin = await userManager.FindByNameAsync("admin");
    if (admin is null)
    {
        admin = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@local"
        };
        var created = await userManager.CreateAsync(admin, "admin123");
        if (created.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            // Si falla creación, puedes loguearlo o revisar errores
        }
    }
}

app.Run();

// -------------------- DTOs --------------------
public record StartMatchDto(string HomeTeam, string AwayTeam, int DurationSeconds);
public record LoginDto(string Username, string Password);
public record RegisterDto(string Username, string Email, string Password);

