using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodPressureDiary.UI;

/// <summary>Configures and builds the MAUI application.</summary>
public static class MauiProgram
{
    /// <summary>Creates the application, registers dependencies, and applies database migrations.</summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddBloodPressureDiaryServices();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<DiaryDbContext>().Database.Migrate();
        }

        return app;
    }
}
