using System.Security.Claims;

namespace WorkoutLogger.API.Features.Progress;

public static class ProgressEndpoints
{
  public static void MapProgressEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/progress").RequireAuthorization();

    group.MapGet("/volume", async (System.Security.Claims.ClaimsPrincipal user, ProgressService svc) =>
    {
        var userId = Guid.Parse(user.FindFirstValue("sub")!);
        return Results.Ok(await svc.GetVolumeByExerciseAsync(userId));
    });

    group.MapGet("/records", async (System.Security.Claims.ClaimsPrincipal user, ProgressService svc) =>
    {
        var userId = Guid.Parse(user.FindFirstValue("sub")!);
        return Results.Ok(await svc.GetPersonalRecordsAsync(userId));
    });

    group.MapGet("/frequency", async (System.Security.Claims.ClaimsPrincipal user, ProgressService svc) =>
    {
        var userId = Guid.Parse(user.FindFirstValue("sub")!);
        return Results.Ok(await svc.GetWorkoutFrequencyAsync(userId));
    });

    group.MapGet("/exercise/{exerciseId}", async (Guid exerciseId, System.Security.Claims.ClaimsPrincipal user, ProgressService svc) =>
    {
        var userId = Guid.Parse(user.FindFirstValue("sub")!);
        return Results.Ok(await svc.GetExerciseProgressAsync(userId, exerciseId));
    });
  }
}