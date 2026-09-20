namespace WorkoutLogger.Contracts.Responses;

public record ExerciseResponse(Guid Id, string Name, string MuscleGroup, string Equipment);