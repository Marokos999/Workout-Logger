using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class WorkoutSummaryPage : ContentPage
{
    public WorkoutSummaryPage(WorkoutSummaryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
