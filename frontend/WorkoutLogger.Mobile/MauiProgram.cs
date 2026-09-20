using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using WorkoutLogger.Mobile.Services;
using WorkoutLogger.Mobile.View;
using WorkoutLogger.Mobile.ViewModel;

namespace WorkoutLogger.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        LiveCharts.Configure(config => config.AddSkiaSharp());

        const string baseUrl = "http://10.0.2.2:5107/";

        // HTTP klijenti
        builder.Services.AddHttpClient<AuthService>(c => c.BaseAddress = new Uri(baseUrl));
        builder.Services.AddTransient<JwtHandler>();
        builder.Services.AddHttpClient<WorkoutService>(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<JwtHandler>();
        builder.Services.AddHttpClient<ExerciseService>(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<JwtHandler>();
        builder.Services.AddHttpClient<ProgressService>(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<JwtHandler>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<WorkoutsViewModel>();
        builder.Services.AddTransient<CreateWorkoutViewModel>();
        builder.Services.AddTransient<ExercisesViewModel>();
        builder.Services.AddTransient<ExerciseDetailViewModel>();
        builder.Services.AddTransient<WorkoutDetailViewModel>();
        builder.Services.AddTransient<AddSetViewModel>();
        builder.Services.AddTransient<ProgressViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<WorkoutsPage>();
        builder.Services.AddTransient<CreateWorkoutPage>();
        builder.Services.AddTransient<WorkoutDetailPage>();
        builder.Services.AddTransient<AddSetPage>();
        builder.Services.AddTransient<ExercisesPage>();
        builder.Services.AddTransient<ExerciseDetailPage>();
        builder.Services.AddTransient<ProgressPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
