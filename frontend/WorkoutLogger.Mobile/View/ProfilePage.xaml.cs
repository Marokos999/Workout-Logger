using WorkoutLogger.Mobile.Services;
using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile.View;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;

    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute(null);
    }

    private void OnLanguageSr(object? sender, EventArgs e)
    {
        LocalizationService.Instance.SetLanguage("sr");
        BtnSr.BackgroundColor = Color.FromArgb("#512BD4");
        BtnSr.TextColor = Colors.White;
        BtnEn.BackgroundColor = Color.FromArgb("#EDE9FA");
        BtnEn.TextColor = Color.FromArgb("#512BD4");
    }

    private void OnLanguageEn(object? sender, EventArgs e)
    {
        LocalizationService.Instance.SetLanguage("en");
        BtnEn.BackgroundColor = Color.FromArgb("#512BD4");
        BtnEn.TextColor = Colors.White;
        BtnSr.BackgroundColor = Color.FromArgb("#EDE9FA");
        BtnSr.TextColor = Color.FromArgb("#512BD4");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var isEn = LocalizationService.Instance.IsEnglish;
        BtnSr.BackgroundColor = isEn ? Color.FromArgb("#EDE9FA") : Color.FromArgb("#512BD4");
        BtnSr.TextColor = isEn ? Color.FromArgb("#512BD4") : Colors.White;
        BtnEn.BackgroundColor = isEn ? Color.FromArgb("#512BD4") : Color.FromArgb("#EDE9FA");
        BtnEn.TextColor = isEn ? Colors.White : Color.FromArgb("#512BD4");
    }
}
