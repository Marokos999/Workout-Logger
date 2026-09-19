using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WorkoutLogger.Tests.Integration.Helpers;

namespace WorkoutLogger.Tests.Integration.Workouts;

public class WorkoutEndpointsTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task AuthenticateAsync()
    {
        var token = await AuthHelper.GetTokenAsync(_client,
            $"workout_{Guid.NewGuid():N}@test.com",
            $"user_{Guid.NewGuid():N}");
        _client.SetBearerToken(token);
    }

    [Fact]
    public async Task GetWorkouts_WithoutAuth_Returns401()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/workouts");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetWorkouts_WithAuth_Returns200_EmptyList()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/api/workouts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<JsonElement[]>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task CreateSession_Returns201_WithId()
    {
        await AuthenticateAsync();

        var response = await _client.PostAsJsonAsync("/api/workouts", new
        {
            name = "Push Day",
            notes = (string?)null
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("id", out var id));
        Assert.NotEmpty(id.GetString()!);
    }

    [Fact]
    public async Task GetSessionById_Returns200()
    {
        await AuthenticateAsync();
        var createResponse = await _client.PostAsJsonAsync("/api/workouts", new { name = "Pull Day", notes = (string?)null });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var response = await _client.GetAsync($"/api/workouts/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task EndSession_Returns200()
    {
        await AuthenticateAsync();
        var createResponse = await _client.PostAsJsonAsync("/api/workouts", new { name = "Leg Day", notes = (string?)null });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var response = await _client.PatchAsync($"/api/workouts/{id}/end", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSession_Returns200()
    {
        await AuthenticateAsync();
        var createResponse = await _client.PostAsJsonAsync("/api/workouts", new { name = "Cardio", notes = (string?)null });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var response = await _client.DeleteAsync($"/api/workouts/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddSet_Returns201()
    {
        await AuthenticateAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkoutLogger.API.Infrastructure.AppDbContext>();
        var exercise = db.Exercises.First();

        var createResponse = await _client.PostAsJsonAsync("/api/workouts", new { name = "Push Day", notes = (string?)null });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var sessionId = created.GetProperty("id").GetString();

        var response = await _client.PostAsJsonAsync($"/api/workouts/{sessionId}/sets", new
        {
            exerciseId = exercise.Id,
            setNumber = 1,
            reps = 8,
            weight = 100.0,
            notes = (string?)null
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
