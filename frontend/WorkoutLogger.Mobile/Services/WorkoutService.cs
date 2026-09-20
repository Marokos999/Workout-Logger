using WorkoutLogger.Contracts.Requests;
using WorkoutLogger.Contracts.Responses;

namespace WorkoutLogger.Mobile.Services;

public class WorkoutService(HttpClient http) : ApiService(http)
{
    public Task<List<WorkoutSessionResponse>?> GetSessionsAsync() =>
        GetAsync<List<WorkoutSessionResponse>>("/api/workouts");

    public Task<WorkoutSessionResponse?> GetSessionByIdAsync(Guid id) =>
        GetAsync<WorkoutSessionResponse>($"/api/workouts/{id}");

    public Task<WorkoutSessionResponse?> CreateSessionAsync(string name, string? notes) =>
        PostAsync<WorkoutSessionResponse>("/api/workouts", new CreateSessionRequest(name, notes));

    public Task PatchEndSessionAsync(Guid id) =>
        PatchAsync($"/api/workouts/{id}/end");

    public Task DeleteSessionAsync(Guid id) =>
        DeleteAsync($"/api/workouts/{id}");

    public Task<WorkoutSetResponse?> AddSetAsync(Guid sessionId, Guid exerciseId, int setNumber, int reps, decimal weight, string? notes) =>
        PostAsync<WorkoutSetResponse>($"/api/workouts/{sessionId}/sets",
            new AddSetRequest(exerciseId, setNumber, reps, weight, notes));

    public Task DeleteSetAsync(Guid setId) =>
        DeleteAsync($"/api/workouts/sets/{setId}");
}
