using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

[QueryProperty(nameof(SessionId), "sessionId")]
public partial class WorkoutSummaryViewModel(WorkoutService workouts) : ObservableObject
{
    [ObservableProperty] private string _workoutName = "";
    [ObservableProperty] private string _duration = "";
    [ObservableProperty] private int _totalSets;
    [ObservableProperty] private decimal _totalVolume;
    [ObservableProperty] private List<ExerciseSummary> _exercises = [];

    private string _sessionId = "";
    public string SessionId
    {
        get => _sessionId;
        set { _sessionId = value; if (Guid.TryParse(value, out var id)) _ = LoadAsync(id); }
    }

    private async Task LoadAsync(Guid id)
    {
        var session = await workouts.GetSessionByIdAsync(id);
        if (session is null) return;

        WorkoutName = session.Name;

        if (session.EndedAt.HasValue)
        {
            var span = session.EndedAt.Value - session.StartedAt;
            Duration = span.TotalHours >= 1
                ? $"{(int)span.TotalHours}h {span.Minutes}min"
                : $"{(int)span.TotalMinutes}min {span.Seconds}s";
        }

        TotalSets = session.WorkoutSets.Count;
        TotalVolume = session.WorkoutSets.Sum(s => s.Reps * s.Weight);

        Exercises = session.WorkoutSets
            .GroupBy(s => new { s.ExerciseId, s.ExerciseName })
            .Select(g => new ExerciseSummary(
                g.Key.ExerciseName,
                g.Count(),
                g.Max(s => s.Weight),
                g.Max(s => s.Reps * s.Weight)))
            .OrderByDescending(e => e.BestVolume)
            .ToList();
    }

    [RelayCommand]
    private static async Task DoneAsync() =>
        await Shell.Current.GoToAsync("//workouts");
}

public record ExerciseSummary(string Name, int Sets, decimal MaxWeight, decimal BestVolume);
