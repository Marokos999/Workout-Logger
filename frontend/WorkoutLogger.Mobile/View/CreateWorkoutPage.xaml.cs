using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class CreateWorkoutPage : ContentPage
{
    public CreateWorkoutPage(CreateWorkoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
