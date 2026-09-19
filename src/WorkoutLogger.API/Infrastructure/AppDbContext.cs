using Microsoft.EntityFrameworkCore;
using WorkoutLogger.API.Domain;

namespace WorkoutLogger.API.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

  public DbSet<User> Users { get; set; }
  public DbSet<WorkoutSession> WorkoutSessions { get; set; }
  public DbSet<Exercise> Exercises { get; set; }
  public DbSet<WorkoutSet> WorkoutSets { get; set; }


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>(e =>
    {
      e.HasKey(u => u.Id);
      e.HasIndex(u => u.Email).IsUnique();
      e.HasIndex(u => u.Username).IsUnique();
    });

    modelBuilder.Entity<WorkoutSession>(e =>
    {
      e.HasKey(w => w.Id);
      e.HasOne(w => w.User)
        .WithMany(u => u.WorkoutSessions)
        .HasForeignKey(w => w.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<WorkoutSet>(e =>
    {
      e.HasKey(w => w.Id);
      e.Property(w => w.Weight).HasPrecision(8, 2);
      e.HasOne(w => w.WorkoutSession)
        .WithMany(u => u.WorkoutSets)
        .HasForeignKey(w => w.WorkoutSessionId)
        .OnDelete(DeleteBehavior.Cascade);
      e.HasOne(w => w.Exercise)
        .WithMany(ex => ex.WorkoutSets)
        .HasForeignKey(w => w.ExerciseId)
        .OnDelete(DeleteBehavior.Restrict);
    });
  }

}