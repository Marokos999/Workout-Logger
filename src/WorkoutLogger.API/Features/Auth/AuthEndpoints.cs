using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.API.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").RequireRateLimiting("auth");

        group.MapPost("/register", async (RegisterRequest req, AuthService auth) =>
        {
            var err = Validate.Required(req.Username, "Username")
                ?? Validate.MaxLength(req.Username, 50, "Username")
                ?? Validate.Required(req.Email, "Email")
                ?? Validate.MaxLength(req.Email, 100, "Email")
                ?? Validate.Required(req.Password, "Password")
                ?? Validate.MinLength(req.Password, 6, "Password");
            if (err is not null) return err;

            var token = await auth.RegisterAsync(req.Username, req.Email, req.Password);
            return token is null
                ? Results.Conflict("Email već postoji.")
                : Results.Ok(new { token });
        });

        group.MapPost("/login", async (LoginRequest req, AuthService auth) =>
        {
            var err = Validate.Required(req.Email, "Email")
                ?? Validate.Required(req.Password, "Password");
            if (err is not null) return err;

            var token = await auth.LoginAsync(req.Email, req.Password);
            return token is null
                ? Results.Unauthorized()
                : Results.Ok(new { token });
        });
        group.MapPost("/logout", (HttpContext ctx) =>
        {
            return Results.Ok();
        }).RequireAuthorization();
    }
}

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Email, string Password);
