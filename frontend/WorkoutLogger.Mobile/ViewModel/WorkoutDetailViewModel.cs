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

    [ObservableProperty] private int _restSeconds = DefaultRest;
    [ObservableProperty] private bool _isTimerRunning;
    [ObservableProperty] private bool _timerFinished;

    private CancellationTokenSource? _timerCts;
    private const int DefaultRest = 90;

    public string RestDisplay => $"{RestSeconds / 60}:{RestSeconds % 60:D2}";

    partial void OnRestSecondsChanged(int value) => OnPropertyChanged(nameof(RestDisplay));

    public void StartRestTimer()
    {
        _timerCts?.Cancel();
        _timerCts = new CancellationTokenSource();
        RestSeconds = DefaultRest;
        IsTimerRunning = true;
        TimerFinished = false;
        _ = RunTimerAsync(_timerCts.Token);
    }

    [RelayCommand]
    private void ResetTimer()
    {
        _timerCts?.Cancel();
        RestSeconds = DefaultRest;
        IsTimerRunning = false;
        TimerFinished = false;
    }

    private async Task RunTimerAsync(CancellationToken ct)
    {
        while (RestSeconds > 0 && !ct.IsCancellationRequested)
        {
            await Task.Delay(1000, ct).ConfigureAwait(false);
            if (ct.IsCancellationRequested) break;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RestSeconds--;
                if (RestSeconds == 0) { IsTimerRunning = false; TimerFinished = true; }
            });
        }
    }

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
        await Shell.Current.GoToAsync($"workout-summary?sessionId={_sessionId}");
    }

    [RelayCommand]
    private async Task DeleteSetAsync(Guid setId)
    {
        await workouts.DeleteSetAsync(setId);
        if (Guid.TryParse(_sessionId, out var id)) await LoadAsync(id);
    }
}
