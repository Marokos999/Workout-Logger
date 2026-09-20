namespace WorkoutLogger.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());
        window.Created += async (_, _) =>
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (token is not null)
                await Shell.Current.GoToAsync("//workouts");
        };
        return window;
    }
}
