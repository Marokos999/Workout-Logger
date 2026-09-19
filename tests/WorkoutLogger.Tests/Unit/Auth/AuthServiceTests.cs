using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkoutLogger.API.Features.Auth;
using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.Tests.Unit.Auth;

public class AuthServiceTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static IConfiguration CreateConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-secret-key-minimum-32-characters-long!",
                ["Jwt:Issuer"] = "WorkoutLogger",
                ["Jwt:Audience"] = "WorkoutLoggerUsers",
                ["Jwt:ExpiresInMinutes"] = "60"
            })
            .Build();

    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsToken()
    {
        using var db = CreateDb();
        var svc = new AuthService(db, CreateConfig());

        var token = await svc.RegisterAsync("marko", "marko@test.com", "Pass123!");

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsNull()
    {
        using var db = CreateDb();
        var svc = new AuthService(db, CreateConfig());

        await svc.RegisterAsync("marko", "marko@test.com", "Pass123!");
        var result = await svc.RegisterAsync("marko2", "marko@test.com", "Pass456!");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        using var db = CreateDb();
        var svc = new AuthService(db, CreateConfig());
        await svc.RegisterAsync("marko", "marko@test.com", "Pass123!");

        var token = await svc.LoginAsync("marko@test.com", "Pass123!");

        Assert.NotNull(token);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        using var db = CreateDb();
        var svc = new AuthService(db, CreateConfig());
        await svc.RegisterAsync("marko", "marko@test.com", "Pass123!");

        var token = await svc.LoginAsync("marko@test.com", "WrongPass!");

        Assert.Null(token);
    }

    [Fact]
    public async Task LoginAsync_NonExistentEmail_ReturnsNull()
    {
        using var db = CreateDb();
        var svc = new AuthService(db, CreateConfig());

        var token = await svc.LoginAsync("nobody@test.com", "Pass123!");

        Assert.Null(token);
    }

    [Fact]
    public async Task RegisterAsync_PasswordIsHashed_NotStoredAsPlainText()
    {
        using var db = CreateDb();
        var svc = new AuthService(db, CreateConfig());
        const string password = "Pass123!";

        await svc.RegisterAsync("marko", "marko@test.com", password);

        var user = db.Users.First();
        Assert.NotEqual(password, user.PasswordHash);
        Assert.StartsWith("$2", user.PasswordHash);
    }
}
