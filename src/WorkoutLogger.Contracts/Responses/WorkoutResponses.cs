namespace WorkoutLogger.Contracts.Responses;

public record WorkoutSessionResponse(
    Guid Id, string Name, string? Notes,
    DateTime StartedAt, DateTime? EndedAt,
    List<WorkoutSetResponse> WorkoutSets);

public record WorkoutSetResponse(
    Guid Id, Guid ExerciseId, string ExerciseName,
    int SetNumber, int Reps, decimal Weight, string? Notes);