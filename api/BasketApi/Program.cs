using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BasketApi.Data;
using BasketApi.Endpoints;
using BasketApi.Models; // para usar User en el seed
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// DbContext: SQL Server
// -------------------------------
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")!)); // usa ! para evitar la advertencia de null

// -------------------------------
// JWT Auth
// -------------------------------
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        var cfg = builder.Configuration.GetSection("Jwt");
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = cfg["Issuer"],
            ValidAudience = cfg["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(cfg["Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

// -------------------------------
// CORS (Angular local, agrega tu dominio prod si aplica)
// -------------------------------
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("app", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
        .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer auth"
    };
    opt.AddSecurityDefinition("Bearer", scheme);
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme
            { Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
          Array.Empty<string>() }
    });
});

var app = builder.Build();

// -------------------------------
// Aplicar migraciones + seed admin
// -------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Aplica todas las migraciones pendientes (crea la BD y tablas si no existen)
    db.Database.Migrate();

    // Seed: crea el admin por defecto si la tabla está vacía
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin"
        });
        db.SaveChanges();
    }
}

// -------------------------------
// Pipeline
// -------------------------------
app.UseCors("app");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

// Endpoints personalizados
app.MapAuthEndpoints();
app.MapTeamEndpoints();
app.MapPlayerEndpoints();
app.MapMatchEndpoints();

app.Run();
