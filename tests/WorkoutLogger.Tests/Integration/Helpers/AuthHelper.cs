using System.Net.Http.Json;
using System.Text.Json;

namespace WorkoutLogger.Tests.Integration.Helpers;

public static class AuthHelper
{
    public static async Task<string> GetTokenAsync(
        HttpClient client,
        string email = "test@example.com",
        string username = "testuser",
        string password = "Test123!")
    {
        await client.PostAsJsonAsync("/api/auth/register", new { username, email, password });

        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("token").GetString()!;
    }

    public static void SetBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
