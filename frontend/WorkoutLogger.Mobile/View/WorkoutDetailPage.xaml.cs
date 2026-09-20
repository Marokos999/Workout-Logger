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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Guid.TryParse(_vm.SessionId, out var id))
            _ = _vm.LoadAsync(id);
    }
}
