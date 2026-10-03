using System.Security.Claims;
using WorkoutLogger.API.Infrastructure;
using WorkoutLogger.Contracts.Responses;

namespace WorkoutLogger.API.Features.Workouts;

public static class WorkoutEndpoints
{
  public static void MapWorkoutEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/workouts").RequireAuthorization();

    group.MapGet("/", async (ClaimsPrincipal user, WorkoutService service,
        int page = 1, int pageSize = 20) =>
    {
      if (page < 1) page = 1;
      if (pageSize is < 1 or > 100) pageSize = 20;
      var userId = Guid.Parse(user.FindFirstValue("sub")!);
      var (items, total) = await service.GetSessionsPagedAsync(userId, page, pageSize);
      return Results.Ok(new PagedResponse<WorkoutSessionResponse>(
          items.Select(ToDto).ToList(), page, pageSize, total));
    });

    group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue("sub")!);
      var session = await service.GetSessionByIdAsync(id, userId);
      return session is null ? Results.NotFound() : Results.Ok(ToDto(session));
    });

    group.MapPost("/", async (CreateSessionRequest request, ClaimsPrincipal user, WorkoutService service) =>
    {
      var err = Validate.Required(request.Name, "Name") ?? Validate.MaxLength(request.Name, 100, "Name");
      if (err is not null) return err;
      var userId = Guid.Parse(user.FindFirstValue("sub")!);
      var session = await service.CreateSessionAsync(userId, request.Name, request.Notes);
      return Results.Created($"/api/workouts/{session.Id}", ToDto(session));
    });

    group.MapPatch("/{id:guid}/end", async (Guid id, ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue("sub")!);
      return await service.EndSessionAsync(id, userId) ? Results.Ok() : Results.NotFound();
    });

    group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue("sub")!);
      return await service.DeleteSessionAsync(id, userId) ? Results.Ok() : Results.NotFound();
    });

    group.MapPost("/{id:guid}/sets", async (Guid id, AddSetRequest request, ClaimsPrincipal user, WorkoutService service) =>
    {
      var err = Validate.Range(request.SetNumber, 1, 100, "SetNumber")
          ?? Validate.Range(request.Reps, 1, 1000, "Reps")
          ?? Validate.Min(request.Weight, 0, "Weight");
      if (err is not null) return err;
      var userId = Guid.Parse(user.FindFirstValue("sub")!);
      var set = await service.AddSetAsync(id, userId, request.ExerciseId, request.SetNumber, request.Reps, request.Weight, request.Notes);
      return Results.Created($"/api/workouts/{id}/set/{set.Id}", set);
    });

    group.MapDelete("/sets/{setId:guid}", async (Guid setId, ClaimsPrincipal user, WorkoutService service) =>
    {
      var userId = Guid.Parse(user.FindFirstValue("sub")!);
      return await service.DeleteSetAsync(setId, userId) ? Results.NoContent() : Results.NotFound();
    });
  }
  private static WorkoutSessionResponse ToDto(WorkoutLogger.API.Domain.WorkoutSession s) => new(
      s.Id, s.Name, s.Notes, s.StartedAt, s.EndedAt,
      s.WorkoutSets.Select(ws => new WorkoutSetResponse(
          ws.Id, ws.ExerciseId, ws.Exercise?.Name ?? "", ws.SetNumber, ws.Reps, ws.Weight, ws.Notes
      )).ToList()
  );
}

public record CreateSessionRequest(string Name, string? Notes);
public record AddSetRequest(Guid ExerciseId, int SetNumber, int Reps, decimal Weight, string? Notes);