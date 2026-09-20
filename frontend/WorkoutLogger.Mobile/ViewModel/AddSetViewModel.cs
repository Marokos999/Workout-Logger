using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

[QueryProperty(nameof(SessionId), "sessionId")]
public partial class AddSetViewModel(WorkoutService workouts, ExerciseService exercises) : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ExerciseResponse> _exercises = [];
    [ObservableProperty] private ExerciseResponse? _selectedExercise;
    [ObservableProperty] private int _setNumber = 1;
    [ObservableProperty] private int _reps = 10;
    [ObservableProperty] private decimal _weight = 0;
    [ObservableProperty] private string _notes = "";
    [ObservableProperty] private string _errorMessage = "";

    public string SessionId { get; set; } = "";

    [RelayCommand]
    private async Task LoadAsync()
    {
        var result = await exercises.GetAllAsync();
        Exercises = new ObservableCollection<ExerciseResponse>(result ?? []);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = "";
        if (SelectedExercise is null) { ErrorMessage = "Odaberi vježbu."; return; }
        if (!Guid.TryParse(SessionId, out var sessionId)) return;
        await workouts.AddSetAsync(sessionId, SelectedExercise.Id, SetNumber, Reps, Weight, Notes.Length > 0 ? Notes : null);
        await Shell.Current.GoToAsync("..");
    }
}
