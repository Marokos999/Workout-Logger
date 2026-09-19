namespace WorkoutLogger.API.Domain;

public class Exercise
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = default!;
  public string MuscleGroup  { get; set; } = default!;
  public string Equipment { get; set; } = default!;

  public ICollection<WorkoutSet> WorkoutSets {get; set;} = [];
}