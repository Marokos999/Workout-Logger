using Microsoft.EntityFrameworkCore;
using WorkoutLogger.API.Domain;
using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.API.Features.Workouts;

public class WorkoutService(AppDbContext db)
{
  public async Task<List<WorkoutSession>> GetSessionAsync(Guid userId) =>
        await db.WorkoutSessions
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.StartedAt)
                .ToListAsync();

  public async Task<WorkoutSession?> GetSessionByIdAsync(Guid id, Guid userId) =>
        await db.WorkoutSessions
                .Include(u => u.WorkoutSets)
                .ThenInclude(s => s.Exercise)
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);


  public async Task<WorkoutSession> CreateSessionAsync(Guid userId, string name, string? notes)
  {
    var session = new WorkoutSession
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Name = name,
      Notes = notes,
      StartedAt = DateTime.UtcNow
    };

    db.WorkoutSessions.Add(session);
    await db.SaveChangesAsync();
    return session;
  }

  public async Task<bool> EndSessionAsync(Guid id, Guid userId)
  {
    var session = await db.WorkoutSessions.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

    if(session is  null) return false;
    session.EndedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteSessionAsync(Guid id, Guid userId)
  {
    var session = await db.WorkoutSessions.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
    if(session is null) return false;
    db.WorkoutSessions.Remove(session);
    await db.SaveChangesAsync();
    return true;
  }

  public async Task<WorkoutSet> AddSetAsync(Guid sessonId, Guid userId, Guid exerciseId, int setNumber, int reps, decimal weight, string? notes)
  {
    var set = new WorkoutSet
    {
      Id = Guid.NewGuid(),
      WorkoutSessionId = sessonId,
      ExerciseId = exerciseId,
      SetNumber = setNumber,
      Reps = reps,
      Weight = weight,
      Notes =  notes
    };

    db.WorkoutSets.Add(set);
    await db.SaveChangesAsync();
    return set;
  }

  public async Task<bool> DeleteSetAsync(Guid setId, Guid userId)
  {
    var set = await db.WorkoutSets
                      .Include(s => s.WorkoutSession)
                      .FirstOrDefaultAsync(i => i.Id == setId && i.WorkoutSession.UserId == userId);

    if(set is null) return false;
    db.WorkoutSets.Remove(set);
    await db.SaveChangesAsync();
    return true;
  }

}