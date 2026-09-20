using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class CreateWorkoutViewModel(WorkoutService workouts) : ObservableObject
{
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _notes = "";
    [ObservableProperty] private string _errorMessage = "";

    [RelayCommand]
    private async Task CreateAsync()
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Naziv je obavezan."; return; }
        var result = await workouts.CreateSessionAsync(Name, Notes);
        if (result is null) { ErrorMessage = "Greška pri kreiranju."; return; }
        await Shell.Current.GoToAsync($"../workout-detail?sessionId={result.Id}");
    }
}
