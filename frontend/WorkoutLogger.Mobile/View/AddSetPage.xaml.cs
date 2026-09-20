using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class AddSetPage : ContentPage
{
    private readonly AddSetViewModel _vm;

    public AddSetPage(AddSetViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute(null);
    }

    private void OnSetNumberDecrement(object? sender, EventArgs e)
    {
        if (_vm.SetNumber > 1) _vm.SetNumber--;
    }

    private void OnSetNumberIncrement(object? sender, EventArgs e) => _vm.SetNumber++;

    private void OnRepsDecrement(object? sender, EventArgs e)
    {
        if (_vm.Reps > 1) _vm.Reps--;
    }

    private void OnRepsIncrement(object? sender, EventArgs e) => _vm.Reps++;
}
