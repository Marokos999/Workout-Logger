using Microsoft.Extensions.DependencyInjection;
using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class ProgressPage : ContentPage
{
    private readonly ProgressViewModel _vm;

    public ProgressPage() : this(IPlatformApplication.Current!.Services.GetRequiredService<ProgressViewModel>()) { }

    public ProgressPage(ProgressViewModel vm)
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
