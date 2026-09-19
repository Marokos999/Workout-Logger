using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WorkoutLogger.Tests.Integration.Helpers;

namespace WorkoutLogger.Tests.Integration.Progress;

public class ProgressEndpointsTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task AuthenticateAsync()
    {
        var token = await AuthHelper.GetTokenAsync(_client,
            $"prog_{Guid.NewGuid():N}@test.com",
            $"proguser_{Guid.NewGuid():N}");
        _client.SetBearerToken(token);
    }

    [Fact]
    public async Task GetVolume_WithAuth_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/api/progress/volume");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecords_WithAuth_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/api/progress/records");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFrequency_WithAuth_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/api/progress/frequency");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetVolume_WithoutAuth_Returns401()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/progress/volume");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRecords_NewUser_ReturnsEmptyList()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/progress/records");
        var list = await response.Content.ReadFromJsonAsync<JsonElement[]>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(list);
        Assert.Empty(list);
    }
}
