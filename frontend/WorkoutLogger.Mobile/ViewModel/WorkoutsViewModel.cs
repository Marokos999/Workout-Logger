using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class WorkoutsViewModel(WorkoutService workouts) : ObservableObject
{
    [ObservableProperty] private ObservableCollection<WorkoutSessionResponse> _sessions = [];
    [ObservableProperty] private bool _isBusy;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        var result = await workouts.GetSessionsAsync();
        Sessions = new ObservableCollection<WorkoutSessionResponse>(result ?? []);
        IsBusy = false;
    }

    [RelayCommand]
    private static async Task GoToCreateAsync() => await Shell.Current.GoToAsync("create-workout");

    [RelayCommand]
    private static async Task SelectAsync(WorkoutSessionResponse session) =>
        await Shell.Current.GoToAsync($"workout-detail?sessionId={session.Id}");

    [RelayCommand]
    private async Task DeleteAsync(Guid id)
    {
        await workouts.DeleteSessionAsync(id);
        await LoadAsync();
    }
}