using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BasketApi.Models;
using BasketApi.Data;

namespace BasketApi.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/auth/register", async (UserDto dto, AppDbContext db) =>
            {
                if (await db.Users.AnyAsync(u => u.Username == dto.Username))
                    return Results.BadRequest("El usuario ya existe");

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = "Admin"
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Usuario registrado correctamente" });
            });

            app.MapPost("/api/auth/login", async (UserDto dto, AppDbContext db, IConfiguration cfg) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
                if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                    return Results.Unauthorized();

                var jwtCfg = cfg.GetSection("Jwt");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCfg["Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtCfg["ExpiresMinutes"]!));

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var token = new JwtSecurityToken(
                    issuer: jwtCfg["Issuer"],
                    audience: jwtCfg["Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new
                {
                    accessToken = jwt,
                    expiresAt = expires
                });
            });
        }
    }

    // DTO para login/register
    public record UserDto(string Username, string Password);
}
