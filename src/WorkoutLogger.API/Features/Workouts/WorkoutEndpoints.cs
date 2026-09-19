using System.Security.Claims;

namespace WorkoutLogger.API.Features.Workouts;

public static class WorkoutEndpoints
{
  public static void MapWorkoutEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/workouts").RequireAuthorization();

    group.MapGet("/", async (ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
      return Results.Ok(await service.GetSessionAsync(userId));
    });

    group.MapGet("/{id:guid}", async (Guid id,ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var session = await service.GetSessionByIdAsync(id, userId);
      return session is null ? Results.NotFound() : Results.Ok(session);
    });

    group.MapPost("/", async (CreateSessionRequest request ,ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var session = await service.CreateSessionAsync(userId, request.Name, request.Notes);
      return Results.Created($"/api/workouts/{session.Id}", session);
    });

    group.MapPatch("/{id:guid}/end", async (Guid id,ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
      return await service.EndSessionAsync(id, userId) ? Results.Ok() : Results.NotFound();
    });

    group.MapDelete("/{id:guid}", async (Guid id,ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
      return await service.DeleteSessionAsync(id, userId) ? Results.Ok() : Results.NotFound();
    });

    group.MapPost("/{id:guid}/sets", async (Guid id,AddSetRequest  request ,ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var set = await service.AddSetAsync(id, userId, request.ExerciseId, request.SetNumber, request.Reps, request.Weight, request.Notes);
      return Results.Created($"/api/workouts/{id}/set/{set.Id}", set);
    });

    group.MapDelete("/sets/{setId:guid}", async (Guid setId,ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
      return await service.DeleteSetAsync(setId, userId) ? Results.NoContent() : Results.NotFound();
    });
  }
}

public record CreateSessionRequest(string Name, string? Notes);
public record AddSetRequest(Guid ExerciseId, int SetNumber, int Reps, decimal Weight, string? Notes);