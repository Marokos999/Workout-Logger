using Microsoft.EntityFrameworkCore;
using WorkoutLogger.API.Domain;
using WorkoutLogger.API.Features.Workouts;
using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.Tests.Unit.Workouts;

public class WorkoutServiceTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid OtherUserId = Guid.NewGuid();

    [Fact]
    public async Task CreateSessionAsync_ReturnsSessionWithCorrectUserId()
    {
        using var db = CreateDb();
        var svc = new WorkoutService(db);

        var session = await svc.CreateSessionAsync(UserId, "Push Day", null);

        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(UserId, session.UserId);
        Assert.Equal("Push Day", session.Name);
    }

    [Fact]
    public async Task GetSessionAsync_ReturnsOnlyUserSessions()
    {
        using var db = CreateDb();
        var svc = new WorkoutService(db);

        await svc.CreateSessionAsync(UserId, "Push Day", null);
        await svc.CreateSessionAsync(UserId, "Pull Day", null);
        await svc.CreateSessionAsync(OtherUserId, "Leg Day", null);

        var sessions = await svc.GetSessionAsync(UserId);

        Assert.Equal(2, sessions.Count);
        Assert.All(sessions, s => Assert.Equal(UserId, s.UserId));
    }

    [Fact]
    public async Task GetSessionByIdAsync_WrongUser_ReturnsNull()
    {
        using var db = CreateDb();
        var svc = new WorkoutService(db);
        var session = await svc.CreateSessionAsync(UserId, "Push Day", null);

        var result = await svc.GetSessionByIdAsync(session.Id, OtherUserId);

        Assert.Null(result);
    }

    [Fact]
    public async Task EndSessionAsync_SetsEndedAt()
    {
        using var db = CreateDb();
        var svc = new WorkoutService(db);
        var session = await svc.CreateSessionAsync(UserId, "Push Day", null);

        var result = await svc.EndSessionAsync(session.Id, UserId);

        Assert.True(result);
        var updated = await db.WorkoutSessions.FindAsync(session.Id);
        Assert.NotEqual(default, updated!.EndedAt);
    }

    [Fact]
    public async Task EndSessionAsync_WrongUser_ReturnsFalse()
    {
        using var db = CreateDb();
        var svc = new WorkoutService(db);
        var session = await svc.CreateSessionAsync(UserId, "Push Day", null);

        var result = await svc.EndSessionAsync(session.Id, OtherUserId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteSessionAsync_RemovesSession()
    {
        using var db = CreateDb();
        var svc = new WorkoutService(db);
        var session = await svc.CreateSessionAsync(UserId, "Push Day", null);

        var result = await svc.DeleteSessionAsync(session.Id, UserId);

        Assert.True(result);
        Assert.Equal(0, db.WorkoutSessions.Count());
    }

    [Fact]
    public async Task AddSetAsync_ReturnsSetLinkedToSession()
    {
        using var db = CreateDb();
        var svc = new WorkoutService(db);
        var session = await svc.CreateSessionAsync(UserId, "Push Day", null);
        var exerciseId = Guid.NewGuid();
        db.Exercises.Add(new Exercise { Id = exerciseId, Name = "Bench Press", MuscleGroup = "Chest", Equipment = "Barbell" });
        await db.SaveChangesAsync();

        var set = await svc.AddSetAsync(session.Id, UserId, exerciseId, 1, 8, 100m, null);

        Assert.NotEqual(Guid.Empty, set.Id);
        Assert.Equal(session.Id, set.WorkoutSessionId);
        Assert.Equal(100m, set.Weight);
        Assert.Equal(8, set.Reps);
    }

    [Fact]
    public async Task DeleteSetAsync_RemovesSet()
    {
        using var db = CreateDb();
        var svc = new WorkoutService(db);
        var session = await svc.CreateSessionAsync(UserId, "Push Day", null);
        var exerciseId = Guid.NewGuid();
        db.Exercises.Add(new Exercise { Id = exerciseId, Name = "Bench Press", MuscleGroup = "Chest", Equipment = "Barbell" });
        await db.SaveChangesAsync();
        var set = await svc.AddSetAsync(session.Id, UserId, exerciseId, 1, 8, 100m, null);

        var result = await svc.DeleteSetAsync(set.Id, UserId);

        Assert.True(result);
        Assert.Equal(0, db.WorkoutSets.Count());
    }
}
