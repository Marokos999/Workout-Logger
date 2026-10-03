using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WorkoutLogger.Tests.Integration.Helpers;

namespace WorkoutLogger.Tests.Integration.Auth;

public class AuthEndpointsTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_NewUser_Returns200WithToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            username = "newuser",
            email = "new@test.com",
            password = "Pass123!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("token", out var token));
        Assert.NotEmpty(token.GetString()!);
        Assert.True(json.TryGetProperty("refreshToken", out var rt));
        Assert.NotEmpty(rt.GetString()!);
    }

    [Fact]
    public async Task Refresh_ValidToken_Returns200WithNewTokens()
    {
        var reg = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            username = "refreshuser",
            email = "refresh@test.com",
            password = "Pass123!"
        });
        var regJson = await reg.Content.ReadFromJsonAsync<JsonElement>();
        var refreshToken = regJson.GetProperty("refreshToken").GetString()!;

        var response = await _client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("token", out var newToken));
        Assert.NotEmpty(newToken.GetString()!);
    }

    [Fact]
    public async Task Refresh_UsedToken_Returns401()
    {
        var reg = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            username = "reusedtoken",
            email = "reused@test.com",
            password = "Pass123!"
        });
        var regJson = await reg.Content.ReadFromJsonAsync<JsonElement>();
        var refreshToken = regJson.GetProperty("refreshToken").GetString()!;

        await _client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken });
        var secondResponse = await _client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken });

        Assert.Equal(HttpStatusCode.Unauthorized, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var payload = new { username = "user1", email = "dup@test.com", password = "Pass123!" };
        await _client.PostAsJsonAsync("/api/auth/register", payload);

        var response = await _client.PostAsJsonAsync("/api/auth/register",
            new { username = "user2", email = "dup@test.com", password = "Pass456!" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            username = "loginuser",
            email = "login@test.com",
            password = "Pass123!"
        });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "login@test.com",
            password = "Pass123!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("token", out _));
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            username = "authuser",
            email = "auth@test.com",
            password = "Pass123!"
        });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "auth@test.com",
            password = "WrongPass!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutToken_Returns401()
    {
        var response = await _client.PostAsync("/api/auth/logout", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("", "valid@test.com", "Pass123!")]
    [InlineData("user", "", "Pass123!")]
    [InlineData("user", "valid@test.com", "abc")]
    public async Task Register_InvalidInput_Returns400(string username, string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register",
            new { username, email, password });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("", "Pass123!")]
    [InlineData("valid@test.com", "")]
    public async Task Login_MissingFields_Returns400(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { email, password });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
