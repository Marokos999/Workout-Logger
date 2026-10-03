using WorkoutLogger.Contracts.Requests;
using WorkoutLogger.Contracts.Responses;

namespace WorkoutLogger.Mobile.Services;

public class AuthService(HttpClient http) : ApiService(http)
{
    private const string TokenKey = "jwt_token";
    private const string RefreshKey = "refresh_token";

    public async Task<string?> LoginAsync(string email, string password)
    {
        var result = await PostAsync<AuthResponse>("/api/auth/login", new LoginRequest(email, password));
        if (result is null) return null;
        await StoreTokensAsync(result);
        return result.Token;
    }

    public async Task<string?> RegisterAsync(string username, string email, string password)
    {
        var result = await PostAsync<AuthResponse>("/api/auth/register", new RegisterRequest(username, email, password));
        if (result is null) return null;
        await StoreTokensAsync(result);
        return result.Token;
    }

    public async Task<bool> RefreshAsync()
    {
        var refreshToken = await SecureStorage.GetAsync(RefreshKey);
        if (refreshToken is null) return false;

        var result = await PostAsync<AuthResponse>("/api/auth/refresh", new RefreshRequest(refreshToken));
        if (result is null) return false;

        await StoreTokensAsync(result);
        return true;
    }

    public async Task<string?> GetTokenAsync() =>
        await SecureStorage.GetAsync(TokenKey);

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

    public void Logout()
    {
        SecureStorage.Remove(TokenKey);
        SecureStorage.Remove(RefreshKey);
    }

    private static async Task StoreTokensAsync(AuthResponse response)
    {
        await SecureStorage.SetAsync(TokenKey, response.Token);
        await SecureStorage.SetAsync(RefreshKey, response.RefreshToken);
    }
}
