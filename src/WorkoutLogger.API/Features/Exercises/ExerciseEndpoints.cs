using WorkoutLogger.Contracts.Requests;

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

    group.MapPost("/", async (CreateExerciseRequest request, ExerciseService svc) =>
    {
      if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest("Name is required.");
      var exercise = await svc.CreateAsync(request.Name.Trim(), request.MuscleGroup.Trim(), request.Equipment.Trim());
      return Results.Created($"/api/exercises/{exercise.Id}",
          new WorkoutLogger.Contracts.Responses.ExerciseResponse(exercise.Id, exercise.Name, exercise.MuscleGroup, exercise.Equipment));
    });
  }
}