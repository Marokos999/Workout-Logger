namespace WorkoutLogger.Contracts.Responses;

public record ExerciseVolumeResponse(Guid ExerciseId, string ExerciseName, decimal TotalVolume, decimal MaxWeight, int TotalSets);
public record PersonalRecordResponse(Guid ExerciseId, string ExerciseName, decimal MaxWeight);
public record WorkoutFrequencyResponse(DateTime Date, int SessionCount);
public record ExerciseProgressPoint(DateOnly Date, decimal MaxWeight);