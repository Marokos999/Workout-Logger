using WorkoutLogger.Contracts.Requests;
using WorkoutLogger.Contracts.Responses;

namespace WorkoutLogger.Mobile.Services;

public class AuthService(HttpClient http) : ApiService(http)
{
    public async Task<string?> LoginAsync(string email, string password)
    {
        var result = await PostAsync<TokenResponse>("/api/auth/login", new LoginRequest(email, password));
        if (result?.Token is null) return null;
        await SecureStorage.SetAsync("jwt_token", result.Token);
        return result.Token;
    }

    public async Task<string?> RegisterAsync(string username, string email, string password)
    {
        var result = await PostAsync<TokenResponse>("/api/auth/register", new RegisterRequest(username, email, password));
        if (result?.Token is null) return null;
        await SecureStorage.SetAsync("jwt_token", result.Token);
        return result.Token;
    }

    public async Task<string?> GetTokenAsync() =>
        await SecureStorage.GetAsync("jwt_token");

    public async Task<string?> GetClaimAsync(string claimName)
    {
        var token = await GetTokenAsync();
        if (token is null) return null;
        var parts = token.Split('.');
        if (parts.Length != 3) return null;
        var payload = parts[1].Replace('-', '+').Replace('_', '/');
        payload += (payload.Length % 4) switch { 2 => "==", 3 => "=", _ => "" };
        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty(claimName, out var v) ? v.GetString() : null;
    }

    public void Logout() =>
        SecureStorage.Remove("jwt_token");

}