namespace WorkoutLogger.Contracts.Requests;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Username, string Email, string Password);