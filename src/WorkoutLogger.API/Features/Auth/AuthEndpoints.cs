namespace WorkoutLogger.API.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest req, AuthService auth) =>
        {
            var token = await auth.RegisterAsync(req.Username, req.Email, req.Password);
            return token is null
                ? Results.Conflict("Email already exists.")
                : Results.Ok(new { token });
        });

        group.MapPost("/login", async (LoginRequest req, AuthService auth) =>
        {
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
