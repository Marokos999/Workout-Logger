using Microsoft.EntityFrameworkCore;
using WorkoutLogger.API.Domain;
using WorkoutLogger.API.Features.Progress;
using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.Tests.Unit.Progress;

public class ProgressServiceTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static readonly Guid UserId = Guid.NewGuid();

    private static async Task<(AppDbContext db, Guid exerciseId, Guid sessionId)> SeedDataAsync()
    {
        var db = CreateDb();
        var exerciseId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        db.Exercises.Add(new Exercise { Id = exerciseId, Name = "Bench Press", MuscleGroup = "Chest", Equipment = "Barbell" });
        db.WorkoutSessions.Add(new WorkoutSession
        {
            Id = sessionId,
            UserId = UserId,
            Name = "Push Day",
            StartedAt = DateTime.UtcNow.AddDays(-5)
        });
        db.WorkoutSets.AddRange(
            new WorkoutSet { Id = Guid.NewGuid(), WorkoutSessionId = sessionId, ExerciseId = exerciseId, SetNumber = 1, Reps = 8, Weight = 100m },
            new WorkoutSet { Id = Guid.NewGuid(), WorkoutSessionId = sessionId, ExerciseId = exerciseId, SetNumber = 2, Reps = 6, Weight = 110m },
            new WorkoutSet { Id = Guid.NewGuid(), WorkoutSessionId = sessionId, ExerciseId = exerciseId, SetNumber = 3, Reps = 4, Weight = 120m }
        );
        await db.SaveChangesAsync();

        return (db, exerciseId, sessionId);
    }

    [Fact]
    public async Task GetPersonalRecordsAsync_ReturnsMaxWeight()
    {
        var (db, exerciseId, _) = await SeedDataAsync();
        var svc = new ProgressService(db);

        var records = await svc.GetPersonalRecordsAsync(UserId);

        Assert.Single(records);
        Assert.Equal(120m, records[0].MaxWeight);
        Assert.Equal("Bench Press", records[0].ExerciseName);
    }

    [Fact]
    public async Task GetVolumeByExerciseAsync_CalculatesTotalVolume()
    {
        var (db, _, _) = await SeedDataAsync();
        var svc = new ProgressService(db);

        var volumes = await svc.GetVolumeByExerciseAsync(UserId);

        Assert.Single(volumes);
        // 8*100 + 6*110 + 4*120 = 800 + 660 + 480 = 1940
        Assert.Equal(1940m, volumes[0].TotalVolume);
        Assert.Equal(3, volumes[0].TotalSets);
    }

    [Fact]
    public async Task GetWorkoutFrequencyAsync_ReturnsOnlyLast30Days()
    {
        var db = CreateDb();
        var exerciseId = Guid.NewGuid();
        db.Exercises.Add(new Exercise { Id = exerciseId, Name = "Squat", MuscleGroup = "Legs", Equipment = "Barbell" });

        var recentSessionId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();

        db.WorkoutSessions.AddRange(
            new WorkoutSession { Id = recentSessionId, UserId = UserId, Name = "Recent", StartedAt = DateTime.UtcNow.AddDays(-5) },
            new WorkoutSession { Id = oldSessionId, UserId = UserId, Name = "Old", StartedAt = DateTime.UtcNow.AddDays(-45) }
        );
        await db.SaveChangesAsync();

        var svc = new ProgressService(db);
        var frequency = await svc.GetWorkoutFrequencyAsync(UserId);

        Assert.Single(frequency);
    }

    [Fact]
    public async Task GetPersonalRecordsAsync_OtherUserData_NotIncluded()
    {
        var (db, exerciseId, _) = await SeedDataAsync();
        var otherUserId = Guid.NewGuid();
        var svc = new ProgressService(db);

        var records = await svc.GetPersonalRecordsAsync(otherUserId);

        Assert.Empty(records);
    }
}
