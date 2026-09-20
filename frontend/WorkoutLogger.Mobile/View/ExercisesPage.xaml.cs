using Microsoft.Extensions.DependencyInjection;
using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class ExercisesPage : ContentPage
{
    private readonly ExercisesViewModel _vm;

    public ExercisesPage() : this(IPlatformApplication.Current!.Services.GetRequiredService<ExercisesViewModel>()) { }

    public ExercisesPage(ExercisesViewModel vm)
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
