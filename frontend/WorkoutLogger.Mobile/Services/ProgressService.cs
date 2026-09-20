using WorkoutLogger.Contracts.Responses;

namespace WorkoutLogger.Mobile.Services;

public class ProgressService(HttpClient http) : ApiService(http)
{
    public Task<List<PersonalRecordResponse>?> GetPersonalRecordsAsync() =>
        GetAsync<List<PersonalRecordResponse>>("/api/progress/records");

    public Task<List<ExerciseVolumeResponse>?> GetVolumeAsync() =>
        GetAsync<List<ExerciseVolumeResponse>>("/api/progress/volume");

    public Task<List<WorkoutFrequencyResponse>?> GetFrequencyAsync() =>
        GetAsync<List<WorkoutFrequencyResponse>>("/api/progress/frequency");

    public Task<List<ExerciseProgressPoint>?> GetExerciseProgressAsync(Guid exerciseId) =>
        GetAsync<List<ExerciseProgressPoint>>($"/api/progress/exercise/{exerciseId}");
}