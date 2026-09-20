using Microsoft.Extensions.DependencyInjection;
using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class LoginPage : ContentPage
{
    public LoginPage() : this(IPlatformApplication.Current!.Services.GetRequiredService<LoginViewModel>()) { }

    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
