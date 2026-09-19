using System.Text.Json;
using WorkoutLogger.API.Domain;

namespace WorkoutLogger.API.Infrastructure;

public static class DataSeeder
{
  public static async Task SeedExerciseAsync(AppDbContext db)
  {
     var folder = "Infrastructure";
     var file = "exercises.json";

    if(db.Exercises.Any()) return;

    var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, folder, file));

    var items = JsonSerializer.Deserialize<List<ExerciseSeedItem>>(
      json, new JsonSerializerOptions {PropertyNameCaseInsensitive = true})!;

    var exercise = items.Select(i => new Exercise
    {
      Id = Guid.NewGuid(),
      Name = i.Name,
      MuscleGroup = i.MuscleGroup,
      Equipment = i.Equipment
    })
    .ToList();

    db.Exercises.AddRange(exercise);
    await db.SaveChangesAsync();
  }

  public record ExerciseSeedItem(string Name, string MuscleGroup, string Equipment );
}