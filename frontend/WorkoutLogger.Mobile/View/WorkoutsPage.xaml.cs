using Microsoft.Extensions.DependencyInjection;
using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class WorkoutsPage : ContentPage
{
    private readonly WorkoutsViewModel _vm;

    public WorkoutsPage() : this(IPlatformApplication.Current!.Services.GetRequiredService<WorkoutsViewModel>()) { }

    public WorkoutsPage(WorkoutsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute(null);
    }
}
