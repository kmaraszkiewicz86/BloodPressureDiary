using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Services;
using BloodPressureDiary.UI.ViewModels;
using BloodPressureDiary.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BloodPressureDiary.UI.Extensions;

/// <summary>Registers the application database, services, view models, and views.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers the dependencies used by the blood pressure diary.</summary>
        public IServiceCollection AddBloodPressureDiaryServices()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "blood-pressure-diary.db");
            services.AddDbContext<DiaryDbContext>(options => options.UseSqlite($"Data Source={databasePath}"));
            services.AddTransient<IBloodPressureMeasurementRepository, BloodPressureMeasurementRepository>();
            services.AddTransient<IBloodPressureMeasurementReminderService, BloodPressureMeasurementReminderService>();
            services.AddTransient<IFileExportService, FileExportService>();
            services.AddSingleton<IFilePicker>(FilePicker.Default);
            services.AddSingleton<IFileSystem>(FileSystem.Current);
            services.AddTransient<IFileSharingService, FileSharingService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserInteractionService, UserInteractionService>();
            services.AddSingleton<IReminderService, ReminderService>();
#if ANDROID
            services.AddSingleton<IReminderScheduler, Platforms.Android.AndroidReminderScheduler>();
#else
            services.AddSingleton<IReminderScheduler, UnsupportedReminderScheduler>();
#endif

            services.AddTransient<MainPageViewModel>();
            services.AddTransient<SettingsPageViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<AppShell>();

            return services;
        }
    }
}
