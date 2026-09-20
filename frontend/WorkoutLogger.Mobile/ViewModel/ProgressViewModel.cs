using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using WorkoutLogger.Contracts.Responses;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class ProgressViewModel(ProgressService progress, AuthService auth) : ObservableObject
{
    [ObservableProperty] private ObservableCollection<PersonalRecordResponse> _records = [];
    [ObservableProperty] private ObservableCollection<ExerciseVolumeResponse> _volume = [];
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isChartBusy;

    // Volume bar chart
    [ObservableProperty] private ISeries[] _volumeSeries = [];
    [ObservableProperty] private Axis[] _volumeXAxes = [];
    [ObservableProperty] private Axis[] _volumeYAxes =
    [
        new Axis { Name = "kg", MinLimit = 0 }
    ];

    // Exercise progress line chart
    [ObservableProperty] private ISeries[] _progressSeries = [];
    [ObservableProperty] private Axis[] _progressXAxes = [];
    [ObservableProperty] private Axis[] _progressYAxes =
    [
        new Axis { Name = "Max težina (kg)", MinLimit = 0 }
    ];

    [ObservableProperty] private PersonalRecordResponse? _selectedRecord;

    partial void OnSelectedRecordChanged(PersonalRecordResponse? value)
    {
        if (value is not null)
            LoadExerciseProgressCommand.Execute(value.ExerciseId);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        var records = await progress.GetPersonalRecordsAsync();
        var volumeData = await progress.GetVolumeAsync();

        Records = new ObservableCollection<PersonalRecordResponse>(records ?? []);
        Volume = new ObservableCollection<ExerciseVolumeResponse>(volumeData ?? []);

        BuildVolumeChart(volumeData ?? []);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task LoadExerciseProgressAsync(Guid exerciseId)
    {
        IsChartBusy = true;
        var data = await progress.GetExerciseProgressAsync(exerciseId);
        BuildProgressChart(data ?? []);
        IsChartBusy = false;
    }

    private void BuildVolumeChart(List<ExerciseVolumeResponse> data)
    {
        var top = data.Take(6).ToList();

        VolumeSeries =
        [
            new ColumnSeries<decimal>
            {
                Values = top.Select(e => e.TotalVolume).ToArray(),
                Fill = new SolidColorPaint(SKColor.Parse("#512BD4")),
                MaxBarWidth = 40
            }
        ];

        VolumeXAxes =
        [
            new Axis
            {
                Labels = top.Select(e => e.ExerciseName.Length > 10
                    ? e.ExerciseName[..10] + "…"
                    : e.ExerciseName).ToArray(),
                LabelsRotation = -30
            }
        ];
    }

    private void BuildProgressChart(List<ExerciseProgressPoint> data)
    {
        if (data.Count == 0)
        {
            ProgressSeries = [];
            ProgressXAxes = [];
            return;
        }

        ProgressSeries =
        [
            new LineSeries<DateTimePoint>
            {
                Values = data
                    .Select(p => new DateTimePoint(
                        p.Date.ToDateTime(TimeOnly.MinValue),
                        (double)p.MaxWeight))
                    .ToArray(),
                Fill = null,
                GeometrySize = 8,
                Stroke = new SolidColorPaint(SKColor.Parse("#F59E0B"), 2),
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#F59E0B"), 2)
            }
        ];

        ProgressXAxes =
        [
            new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd.MM"))
        ];
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        auth.Logout();
        await Shell.Current.GoToAsync("//login");
    }
}
