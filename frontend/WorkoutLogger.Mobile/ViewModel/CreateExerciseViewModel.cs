using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class CreateExerciseViewModel(ExerciseService exercises) : ObservableObject
{
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _muscleGroup = "";
    [ObservableProperty] private string _equipment = "";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _isSaving;
    public bool IsNotSaving => !IsSaving;

    partial void OnIsSavingChanged(bool value) => OnPropertyChanged(nameof(IsNotSaving));

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Naziv je obavezan."; return; }
        if (string.IsNullOrWhiteSpace(MuscleGroup)) { ErrorMessage = "Mišićna grupa je obavezna."; return; }
        if (string.IsNullOrWhiteSpace(Equipment)) { ErrorMessage = "Oprema je obavezna."; return; }

        IsSaving = true;
        var result = await exercises.CreateAsync(Name.Trim(), MuscleGroup.Trim(), Equipment.Trim());
        IsSaving = false;

        if (result is null) { ErrorMessage = "Greška pri kreiranju vježbe."; return; }
        await Shell.Current.GoToAsync("..");
    }
}
