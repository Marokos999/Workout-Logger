namespace WorkoutLogger.Contracts.Requests;

public record CreateSessionRequest(string Name, string? Notes);
public record AddSetRequest(Guid ExerciseId, int SetNumber, int Reps, decimal Weight, string? Notes);
public record CreateExerciseRequest(string Name, string MuscleGroup, string Equipment);