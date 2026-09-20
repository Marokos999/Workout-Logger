using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class LoginViewModel(AuthService auth) : ObservableObject
{
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _errorMessage = "";

    [RelayCommand]
    private static async Task GoToRegisterAsync() => await Shell.Current.GoToAsync("register");

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = "";
        var token = await auth.LoginAsync(Email, Password);
        if (token is null) { ErrorMessage = "Pogrešni podaci."; return; }
        await Shell.Current.GoToAsync("//workouts");
    }
}
