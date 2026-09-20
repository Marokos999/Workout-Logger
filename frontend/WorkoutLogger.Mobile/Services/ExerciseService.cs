using WorkoutLogger.Contracts.Responses;

namespace WorkoutLogger.Mobile.Services;

public class ExerciseService(HttpClient http) : ApiService(http)
{
    public Task<List<ExerciseResponse>?> GetAllAsync() =>
        GetAsync<List<ExerciseResponse>>("/api/exercises");

    public Task<List<ExerciseResponse>?> GetByMuscleGroupAsync(string muscleGroup) =>
        GetAsync<List<ExerciseResponse>>($"/api/exercises/muscle/{muscleGroup}");

    public Task<ExerciseResponse?> GetByIdAsync(Guid id) =>
        GetAsync<ExerciseResponse>($"/api/exercises/{id}");
}