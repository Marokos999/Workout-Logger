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
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private bool _isLoadingMore;
    [ObservableProperty] private bool _hasMore;

    private int _currentPage = 1;
    private const int PageSize = 20;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        _currentPage = 1;
        var result = await workouts.GetSessionsAsync(_currentPage, PageSize);
        Sessions = new ObservableCollection<WorkoutSessionResponse>(result?.Items ?? []);
        HasMore = result?.HasNextPage ?? false;
        IsBusy = false;
        IsRefreshing = false;
    }

    [RelayCommand(CanExecute = nameof(CanLoadMore))]
    private async Task LoadMoreAsync()
    {
        if (IsLoadingMore) return;
        IsLoadingMore = true;
        _currentPage++;
        var result = await workouts.GetSessionsAsync(_currentPage, PageSize);
        foreach (var item in result?.Items ?? [])
            Sessions.Add(item);
        HasMore = result?.HasNextPage ?? false;
        IsLoadingMore = false;
    }

    private bool CanLoadMore() => HasMore && !IsLoadingMore;

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