using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkoutLogger.API.Domain;
using WorkoutLogger.API.Infrastructure;
using WorkoutLogger.Contracts.Responses;

namespace WorkoutLogger.API.Features.Auth;

public class AuthService(AppDbContext context, IConfiguration config)
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    public async Task<AuthResponse?> RegisterAsync(string username, string email, string password)
    {
        if (await context.Users.AnyAsync(u => u.Email == email))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse?> RefreshAsync(string refreshToken)
    {
        var stored = await context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (stored is null || stored.IsUsed || stored.ExpiresAt < DateTime.UtcNow)
            return null;

        stored.IsUsed = true;
        await context.SaveChangesAsync();

        return await IssueTokensAsync(stored.User);
    }

    private async Task<AuthResponse> IssueTokensAsync(User user)
    {
        var accessToken = GenerateJwt(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id);
        return new AuthResponse(accessToken, refreshToken);
    }

    private async Task<string> CreateRefreshTokenAsync(Guid userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime)
        });
        await context.SaveChangesAsync();
        return token;
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("name", user.Username),
            new Claim("email", user.Email)
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
