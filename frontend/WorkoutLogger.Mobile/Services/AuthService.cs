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

    public void Logout() =>
        SecureStorage.Remove("jwt_token");

}