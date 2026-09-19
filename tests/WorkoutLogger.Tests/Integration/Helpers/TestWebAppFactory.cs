using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkoutLogger.API.Domain;
using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.Tests.Integration.Helpers;

public class TestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-secret-key-minimum-32-characters-long!",
                ["Jwt:Issuer"] = "WorkoutLogger",
                ["Jwt:Audience"] = "WorkoutLoggerUsers",
                ["Jwt:ExpiresInMinutes"] = "60"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        db.Exercises.AddRange(
            new Exercise { Id = Guid.NewGuid(), Name = "Bench Press", MuscleGroup = "Chest", Equipment = "Barbell" },
            new Exercise { Id = Guid.NewGuid(), Name = "Squat", MuscleGroup = "Legs", Equipment = "Barbell" },
            new Exercise { Id = Guid.NewGuid(), Name = "Deadlift", MuscleGroup = "Back", Equipment = "Barbell" }
        );
        await db.SaveChangesAsync();
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}
