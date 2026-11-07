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
            return services
                .AddDatabase()
                .AddServices()
                .AddPlatformServices()
                .AddViewModels()
                .AddViews();
        }

        /// <summary>Registers the SQLite database and measurement repository.</summary>
        private IServiceCollection AddDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "blood-pressure-diary.db");
            services.AddDbContext<DiaryDbContext>(options => options.UseSqlite($"Data Source={databasePath}"));
            services.AddTransient<IBloodPressureMeasurementRepository, BloodPressureMeasurementRepository>();
            return services;
        }

        /// <summary>Registers the application services.</summary>
        private IServiceCollection AddServices()
        {
            services.AddTransient<IBloodPressureMeasurementReminderService, BloodPressureMeasurementReminderService>();
            services.AddTransient<IFileExportService, FileExportService>();
            services.AddTransient<IFileSharingService, FileSharingService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserInteractionService, UserInteractionService>();
            services.AddSingleton<IReminderSettingsStorage, ReminderSettingsStorage>();
            services.AddSingleton<IReminderService, ReminderService>();
            services.AddSingleton<IReminderScheduleCalculator, ReminderScheduleCalculator>();
            return services;
        }

        /// <summary>Registers MAUI platform APIs and the Android reminder scheduler.</summary>
        private IServiceCollection AddPlatformServices()
        {
            // Opens the system file picker when importing a CSV backup.
            services.AddSingleton<IFilePicker>(FilePicker.Default);

            // Provides the cache directory for exports and the app data directory for reminder settings.
            services.AddSingleton<IFileSystem>(FileSystem.Current);

            // Reads reminder preferences saved by previous versions for migration to the settings file.
            services.AddSingleton<IPreferences>(Preferences.Default);

#if ANDROID
            services.AddSingleton<IReminderScheduler, Platforms.Android.AndroidReminderScheduler>();
#endif
            return services;
        }

        /// <summary>Registers the view models.</summary>
        private IServiceCollection AddViewModels()
        {
            services.AddTransient<MainPageViewModel>();
            services.AddTransient<SettingsPageViewModel>();
            return services;
        }

        /// <summary>Registers the application pages and shell.</summary>
        private IServiceCollection AddViews()
        {
            services.AddTransient<MainPage>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<AppShell>();
            return services;
        }
    }
}
