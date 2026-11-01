using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Services;
using BloodPressureDiary.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace BloodPressureDiary.UI;
/// <summary>Configures application dependencies.</summary>
public static class MauiProgram
{
    /// <summary>Builds the diary application.</summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        builder.Services.AddDbContextFactory<DiaryContext>(options =>
            options.UseSqlite($"Data Source={Path.Combine(FileSystem.AppDataDirectory, "diary.db")}"));
        builder.Services.AddSingleton<IBloodPressureMeasurementService, BloodPressureMeasurementService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IReminderService, AndroidReminderService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<AppShell>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        var app = builder.Build();
        using var context = app.Services.GetRequiredService<IDbContextFactory<DiaryContext>>().CreateDbContext();
        context.Database.Migrate();
        return app;
    }
}
