namespace WorkoutLogger.API.Domain;

public class WorkoutSet
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid WorkoutSessionId { get; set; }
  public Guid ExerciseId { get; set; }
  public int SetNumber { get; set; }
  public int Reps { get; set; }
  public decimal Weight { get; set; }
  public string? Notes { get; set; }

  public WorkoutSession WorkoutSession { get; set; } = null!;
  public Exercise Exercise { get; set; } = null!;
}