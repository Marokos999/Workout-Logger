namespace WorkoutLogger.API.Domain;

public class User
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Username { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string PasswordHash { get; set; } = default!;
  public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;


  public ICollection<WorkoutSession> WorkoutSessions { get; set; } = [];

}