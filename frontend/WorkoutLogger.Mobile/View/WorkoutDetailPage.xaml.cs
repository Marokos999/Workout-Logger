using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class WorkoutDetailPage : ContentPage
{
    private readonly WorkoutDetailViewModel _vm;

    public WorkoutDetailPage(WorkoutDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    private bool _firstAppear = true;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Guid.TryParse(_vm.SessionId, out var id))
            _ = _vm.LoadAsync(id);

        if (!_firstAppear && _vm.IsActive)
            _vm.StartRestTimer();

        _firstAppear = false;
    }
}
