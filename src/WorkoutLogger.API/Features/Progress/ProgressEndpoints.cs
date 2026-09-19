using System.Security.Claims;
using Npgsql.Replication;

namespace WorkoutLogger.API.Features.Progress;

public static class ProgressEndpoints
{
  public static void MapProgressEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/progress").RequireAuthorization();

    group.MapGet("/volume", async (ClaimsPrincipal user, ProgressService svc) =>
    {
        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Results.Ok(await svc.GetVolumeByExerciseAsync(userId));
    });

    group.MapGet("/records", async (ClaimsPrincipal user, ProgressService svc) =>
    {
        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Results.Ok(await svc.GetPersonalRecordsAsync(userId));
    });

    group.MapGet("/frequency", async (ClaimsPrincipal user, ProgressService svc) =>
    {
        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Results.Ok(await svc.GetWorkoutFrequencyAsync(userId));
    });
  }
}