using Microsoft.EntityFrameworkCore;
using WorkoutLogger.API.Domain;
using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.API.Features.Progress;

public class ProgressService(AppDbContext db)
{
  public async Task<List<ExerciseVolumeDto>> GetVolumeByExerciseAsync(Guid userId) =>
     await db.WorkoutSets
            .Include(s => s.WorkoutSession)
            .Include(s => s.Exercise)
            .Where(s => s.WorkoutSession.UserId == userId)
            .GroupBy(s => new {s.ExerciseId, s.Exercise.Name})
            .Select(g => new ExerciseVolumeDto(
              g.Key.ExerciseId,
              g.Key.Name,
              g.Sum(s => s.Reps * s.Weight),
              g.Max(s => s.Weight),
              g.Count()))
            .OrderByDescending(e => e.TotalVolume)
            .ToListAsync();
  public async Task<List<PersonalRecordDto>> GetPersonalRecordsAsync(Guid userId) =>
    await db.WorkoutSets
            .Include(s => s.WorkoutSession)
            .Include(s => s.Exercise)
            .Where(s => s.WorkoutSession.UserId == userId)
            .GroupBy(s => new {s.ExerciseId, s.Exercise.Name})
            .Select(g => new PersonalRecordDto(
              g.Key.ExerciseId,
              g.Key.Name,
              g.Max(s => s.Weight)))
            .OrderByDescending(e => e.MaxWeight)
            .ToListAsync();

  public async Task<List<WorkoutFrequencyDto>> GetWorkoutFrequencyAsync(Guid userId) =>
    await db.WorkoutSessions
            .Where(s => s.UserId == userId && s.StartedAt >= DateTime.UtcNow.AddDays(-30))
            .GroupBy(s => s.StartedAt.Date)
            .Select(g => new WorkoutFrequencyDto(g.Key, g.Count()))
            .OrderBy(e => e.Date)
            .ToListAsync();
}

public record ExerciseVolumeDto(Guid ExerciseId, string ExerciseName, decimal TotalVolume, decimal MaxWeight, int TotalSets);
public record PersonalRecordDto(Guid ExerciseId, string ExerciseName, decimal MaxWeight);
public record WorkoutFrequencyDto(DateTime Date, int SessionCount);