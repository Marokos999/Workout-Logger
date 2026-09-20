using WorkoutLogger.Mobile.View;

namespace WorkoutLogger.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("create-workout", typeof(CreateWorkoutPage));
        Routing.RegisterRoute("workout-detail", typeof(WorkoutDetailPage));
        Routing.RegisterRoute("add-set", typeof(AddSetPage));
        Routing.RegisterRoute("exercise-detail", typeof(ExerciseDetailPage));
    }
}
