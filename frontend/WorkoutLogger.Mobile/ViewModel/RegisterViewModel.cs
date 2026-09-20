using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class RegisterViewModel(AuthService auth) : ObservableObject
{
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _errorMessage = "";

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = "";
        var token = await auth.RegisterAsync(Username, Email, Password);
        if (token is null) { ErrorMessage = "Registracija neuspješna."; return; }
        await Shell.Current.GoToAsync("//workouts");
    }
}
