namespace WorkoutLogger.Contracts.Responses;

public record WorkoutSessionResponse(
    Guid Id, string Name, string? Notes,
    DateTime StartedAt, DateTime? EndedAt,
    List<WorkoutSetResponse> WorkoutSets);

public record WorkoutSetResponse(
    Guid Id, Guid ExerciseId, string ExerciseName,
    int SetNumber, int Reps, decimal Weight, string? Notes);

public record PagedResponse<T>(List<T> Items, int Page, int PageSize, int TotalCount)
{
    public bool HasNextPage => Page * PageSize < TotalCount;
}