using CommunityToolkit.Mvvm.ComponentModel;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

[QueryProperty(nameof(ExerciseId), "id")]
public partial class ExerciseDetailViewModel(ExerciseService exercises) : ObservableObject
{
    [ObservableProperty] private ExerciseResponse? _exercise;

    private string _exerciseId = "";
    public string ExerciseId
    {
        get => _exerciseId;
        set
        {
            _exerciseId = value;
            if (Guid.TryParse(value, out var id))
                _ = LoadAsync(id);
        }
    }

    private async Task LoadAsync(Guid id)
    {
        Exercise = await exercises.GetByIdAsync(id);
    }
}
