using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class CreateExercisePage : ContentPage
{
    public CreateExercisePage(CreateExerciseViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
