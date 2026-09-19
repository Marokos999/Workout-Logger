using Microsoft.EntityFrameworkCore;
using WorkoutLogger.API.Domain;
using WorkoutLogger.API.Infrastructure;

namespace WorkoutLogger.API.Features.Exercises;

public class ExerciseService(AppDbContext db)
{
  public async Task<List<Exercise>> GetAllAsync() =>
      await db.Exercises.OrderBy(e => e.MuscleGroup)
                        .ThenBy(e => e.Name)
                        .ToListAsync();


  public async Task<List<Exercise>> GetByMuscleGroupAsync(string muscleGroup) =>
      await db.Exercises.Where(m => m.MuscleGroup == muscleGroup)
                        .OrderBy(e => e.Name)
                        .ToListAsync();


  public async Task<Exercise?> GetByIdAsync(Guid id) =>
      await db.Exercises.FindAsync(id);
}