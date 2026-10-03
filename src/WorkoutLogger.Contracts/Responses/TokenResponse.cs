namespace WorkoutLogger.Contracts.Responses;

public record AuthResponse(string Token, string RefreshToken);

public record TokenResponse(string Token);