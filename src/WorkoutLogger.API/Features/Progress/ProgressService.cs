using Microsoft.EntityFrameworkCore;
using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.API.Features.Progress;

public class ProgressService(AppDbContext db)
{
    public async Task<List<ExerciseVolumeDto>> GetVolumeByExerciseAsync(Guid userId)
    {
        var sets = await db.WorkoutSets
            .Include(s => s.WorkoutSession)
            .Include(s => s.Exercise)
            .Where(s => s.WorkoutSession.UserId == userId)
            .ToListAsync();

        return sets
            .GroupBy(s => new { s.ExerciseId, s.Exercise.Name })
            .Select(g => new ExerciseVolumeDto(
                g.Key.ExerciseId,
                g.Key.Name,
                g.Sum(s => s.Reps * s.Weight),
                g.Max(s => s.Weight),
                g.Count()))
            .OrderByDescending(e => e.TotalVolume)
            .ToList();
    }

    public async Task<List<PersonalRecordDto>> GetPersonalRecordsAsync(Guid userId)
    {
        var sets = await db.WorkoutSets
            .Include(s => s.WorkoutSession)
            .Include(s => s.Exercise)
            .Where(s => s.WorkoutSession.UserId == userId)
            .ToListAsync();

        return sets
            .GroupBy(s => new { s.ExerciseId, s.Exercise.Name })
            .Select(g => new PersonalRecordDto(
                g.Key.ExerciseId,
                g.Key.Name,
                g.Max(s => s.Weight)))
            .OrderByDescending(e => e.MaxWeight)
            .ToList();
    }

    public async Task<List<ExerciseProgressDto>> GetExerciseProgressAsync(Guid userId, Guid exerciseId)
    {
        var sets = await db.WorkoutSets
            .Include(s => s.WorkoutSession)
            .Where(s => s.WorkoutSession.UserId == userId && s.ExerciseId == exerciseId)
            .ToListAsync();

        return sets
            .GroupBy(s => DateOnly.FromDateTime(s.WorkoutSession.StartedAt.Date))
            .Select(g => new ExerciseProgressDto(g.Key, g.Max(s => s.Weight)))
            .OrderBy(e => e.Date)
            .ToList();
    }

    public async Task<List<WorkoutFrequencyDto>> GetWorkoutFrequencyAsync(Guid userId)
    {
        var sessions = await db.WorkoutSessions
            .Where(s => s.UserId == userId && s.StartedAt >= DateTime.UtcNow.AddDays(-30))
            .ToListAsync();

        return sessions
            .GroupBy(s => s.StartedAt.Date)
            .Select(g => new WorkoutFrequencyDto(g.Key, g.Count()))
            .OrderBy(e => e.Date)
            .ToList();
    }
}

public record ExerciseVolumeDto(Guid ExerciseId, string ExerciseName, decimal TotalVolume, decimal MaxWeight, int TotalSets);
public record PersonalRecordDto(Guid ExerciseId, string ExerciseName, decimal MaxWeight);
public record WorkoutFrequencyDto(DateTime Date, int SessionCount);
public record ExerciseProgressDto(DateOnly Date, decimal MaxWeight);
