namespace WorkoutLogger.API.Features.Exercises;

public static class ExerciseEndpoints
{
  public static void MapExerciseEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/exercises").RequireAuthorization();

    group.MapGet("/", async(ExerciseService svc) =>
    Results.Ok(await svc.GetAllAsync()));

    group.MapGet("/muscle/{muscleGroup}", async(ExerciseService svc, string muscleGroup) =>
    Results.Ok(await svc.GetByMuscleGroupAsync(muscleGroup)));

    group.MapGet("/{id:guid}", async(ExerciseService svc, Guid id) =>
    {
      var exercise = await svc.GetByIdAsync(id);
      return exercise is null ? Results.NotFound() : Results.Ok(exercise);
    });
  }
}