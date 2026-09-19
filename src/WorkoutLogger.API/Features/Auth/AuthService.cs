using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkoutLogger.API.Domain;
using WorkoutLogger.API.Infrastructure;


namespace WorkoutLogger.API.Features.Auth;

public class AuthService(AppDbContext context, IConfiguration config)
{
  public async Task<string?> RegisterAsync(string username, string email, string password)
  {
    if(await context.Users.AnyAsync(u => u.Email == email))
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

    return GenerateToken(user);
  }

  public async Task<string?> LoginAsync(string email, string password)
  {
    var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        return null;

    return GenerateToken(user);
  }

  private string? GenerateToken(User user)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

    var claims = new []
    {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
    };
    var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
  }

}