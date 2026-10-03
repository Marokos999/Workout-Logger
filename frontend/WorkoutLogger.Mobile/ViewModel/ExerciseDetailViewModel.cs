using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

[QueryProperty(nameof(ExerciseId), "id")]
public partial class ExerciseDetailViewModel(ExerciseService exercises, ProgressService progress) : ObservableObject
{
    [ObservableProperty] private ExerciseResponse? _exercise;
    [ObservableProperty] private decimal _personalRecord;
    [ObservableProperty] private bool _hasPr;
    public bool IsEmpty => !HasPr;
    partial void OnHasPrChanged(bool value) => OnPropertyChanged(nameof(IsEmpty));
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private ObservableCollection<ExerciseProgressPoint> _history = [];

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

    [RelayCommand]
    private async Task LoadAsync(Guid id)
    {
        IsBusy = true;
        var exerciseTask = exercises.GetByIdAsync(id);
        var historyTask = progress.GetExerciseProgressAsync(id);

        await Task.WhenAll(exerciseTask, historyTask);

        Exercise = await exerciseTask;

        var points = await historyTask ?? [];
        History = new ObservableCollection<ExerciseProgressPoint>(
            points.OrderByDescending(p => p.Date));

        if (points.Count > 0)
        {
            PersonalRecord = points.Max(p => p.MaxWeight);
            HasPr = true;
        }

        IsBusy = false;
    }
}
