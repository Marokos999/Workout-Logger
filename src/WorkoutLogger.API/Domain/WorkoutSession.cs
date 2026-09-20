namespace WorkoutLogger.API.Domain;

public class WorkoutSession
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid UserId { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = default!;
  public DateTime StartedAt { get; set; }
  public DateTime? EndedAt { get; set; }
  public string? Notes { get; set; }

  public User User { get; set; } = null!;
  public ICollection<WorkoutSet> WorkoutSets { get; set; } = [];
}