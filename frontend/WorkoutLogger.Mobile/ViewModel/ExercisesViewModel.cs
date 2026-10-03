using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class ExercisesViewModel(ExerciseService exercises) : ObservableObject
{
    private List<ExerciseResponse> _allExercises = [];
    [ObservableProperty] private ObservableCollection<ExerciseResponse> _exercises = [];
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _searchText = "";

    partial void OnSearchTextChanged(string value)
    {
        var q = value.Trim();
        Exercises = string.IsNullOrEmpty(q)
            ? new ObservableCollection<ExerciseResponse>(_allExercises)
            : new ObservableCollection<ExerciseResponse>(
                _allExercises.Where(e =>
                    e.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    e.MuscleGroup.Contains(q, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        var result = await exercises.GetAllAsync();
        _allExercises = result ?? [];
        Exercises = new ObservableCollection<ExerciseResponse>(_allExercises);
        IsBusy = false;
    }

    [RelayCommand]
    private static async Task SelectAsync(ExerciseResponse exercise) =>
        await Shell.Current.GoToAsync($"exercise-detail?id={exercise.Id}");

    [RelayCommand]
    private static async Task GoToCreateAsync() =>
        await Shell.Current.GoToAsync("create-exercise");
}