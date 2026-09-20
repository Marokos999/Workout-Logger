using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class ExercisesViewModel(ExerciseService exercises) : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ExerciseResponse> _exercises = [];
    [ObservableProperty] private bool _isBusy;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        var result = await exercises.GetAllAsync();
        Exercises = new ObservableCollection<ExerciseResponse>(result ?? []);
        IsBusy = false;
    }

    [RelayCommand]
    private static async Task SelectAsync(ExerciseResponse exercise) =>
        await Shell.Current.GoToAsync($"exercise-detail?id={exercise.Id}");
}