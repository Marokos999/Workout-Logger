using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

[QueryProperty(nameof(SessionId), "sessionId")]
public partial class WorkoutDetailViewModel(WorkoutService workouts) : ObservableObject
{
    [ObservableProperty] private WorkoutSessionResponse? _session;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _hasEnded;

    partial void OnSessionChanged(WorkoutSessionResponse? value)
    {
        IsActive = value?.EndedAt is null;
        HasEnded = value?.EndedAt is not null;
    }

    private string _sessionId = "";
    public string SessionId
    {
        get => _sessionId;
        set { _sessionId = value; if (Guid.TryParse(value, out var id)) _ = LoadAsync(id); }
    }

    public async Task LoadAsync(Guid id)
    {
        IsBusy = true;
        Session = await workouts.GetSessionByIdAsync(id);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task GoToAddSetAsync() =>
        await Shell.Current.GoToAsync($"add-set?sessionId={_sessionId}");

    [RelayCommand]
    private async Task EndWorkoutAsync()
    {
        if (!Guid.TryParse(_sessionId, out var id)) return;
        await workouts.PatchEndSessionAsync(id);
        await LoadAsync(id);
    }

    [RelayCommand]
    private async Task DeleteSetAsync(Guid setId)
    {
        await workouts.DeleteSetAsync(setId);
        if (Guid.TryParse(_sessionId, out var id)) await LoadAsync(id);
    }
}
