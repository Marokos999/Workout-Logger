using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WorkoutLogger.Tests.Integration.Helpers;

namespace WorkoutLogger.Tests.Integration.Exercises;

public class ExerciseEndpointsTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task AuthenticateAsync()
    {
        var token = await AuthHelper.GetTokenAsync(_client,
            $"ex_{Guid.NewGuid():N}@test.com",
            $"exuser_{Guid.NewGuid():N}");
        _client.SetBearerToken(token);
    }

    [Fact]
    public async Task GetExercises_WithoutAuth_Returns401()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/exercises");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetExercises_WithAuth_ReturnsSeededExercises()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/exercises");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<JsonElement[]>();
        Assert.NotNull(list);
        Assert.True(list.Length >= 3);
    }

    [Fact]
    public async Task GetByMuscleGroup_Returns200_FilteredList()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/exercises/muscle/Chest");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<JsonElement[]>();
        Assert.NotNull(list);
        Assert.All(list, e => Assert.Equal("Chest", e.GetProperty("muscleGroup").GetString()));
    }

    [Fact]
    public async Task GetById_Returns200_CorrectExercise()
    {
        await AuthenticateAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkoutLogger.API.Infrastructure.AppDbContext>();
        var exercise = db.Exercises.First();

        var response = await _client.GetAsync($"/api/exercises/{exercise.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(exercise.Name, json.GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync($"/api/exercises/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
