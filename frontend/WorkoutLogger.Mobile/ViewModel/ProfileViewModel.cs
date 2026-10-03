using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkoutLogger.Mobile.Services;

namespace WorkoutLogger.Mobile.ViewModel;

public partial class ProfileViewModel(AuthService auth) : ObservableObject
{
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _email = "";

    public string Initials => Username.Length > 0 ? Username[..1].ToUpper() : "?";

    partial void OnUsernameChanged(string value) => OnPropertyChanged(nameof(Initials));

    [RelayCommand]
    private async Task LoadAsync()
    {
        Username = await auth.GetClaimAsync("name") ?? "—";
        Email = await auth.GetClaimAsync("email") ?? "—";
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        auth.Logout();
        await Shell.Current.GoToAsync("//login");
    }
}
