using System.Globalization;
using System.Text.Json;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Models;
using Microsoft.Maui.Storage;

namespace BloodPressureDiary.UI.Services;

/// <summary>Stores reminder settings in a file and reads settings from previous versions.</summary>
public sealed class ReminderSettingsStorage(IFileSystem _fileSystem, IPreferences _preferences) : IReminderSettingsStorage
{
    private const string _fileName = "reminder-settings.json";
    private const string _enabledKey = "daily-reminder-enabled";
    private const string _timeKey = "daily-reminder-time";
    private const string _schedulesKey = "reminder-schedules";
    private const int _legacyReminderId = 7421;

    /// <summary>Reads saved reminder settings, including settings from earlier versions.</summary>
    public async Task<ReminderSettings> GetAsync()
    {
        var path = Path.Combine(_fileSystem.AppDataDirectory, _fileName);
        if (File.Exists(path))
        {
            await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete, 4096, true);
            return await JsonSerializer.DeserializeAsync<ReminderSettings>(stream) ?? new ReminderSettings();
        }

        var reminders = new List<ReminderSchedule>();
        var savedSchedules = _preferences.Get(_schedulesKey, string.Empty);
        if (!string.IsNullOrEmpty(savedSchedules))
        {
            try
            {
                reminders = JsonSerializer.Deserialize<List<ReminderSchedule>>(savedSchedules) ?? [];
            }
            catch (JsonException)
            {
                reminders = [];
            }
        }
        else
        {
            var savedTime = _preferences.Get(_timeKey, string.Empty);
            if (TimeSpan.TryParse(savedTime, CultureInfo.InvariantCulture, out var time)
                && time >= TimeSpan.Zero && time < TimeSpan.FromDays(1))
            {
                reminders.Add(new ReminderSchedule { Id = _legacyReminderId, Time = time, IntervalDays = 1, StartDate = DateTime.Today });
            }
        }

        var settings = new ReminderSettings { IsEnabled = _preferences.Get(_enabledKey, false), Reminders = reminders };
        try
        {
            await WriteAsync(settings, false);
            return settings;
        }
        catch (IOException) when (File.Exists(path))
        {
            await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete, 4096, true);
            return await JsonSerializer.DeserializeAsync<ReminderSettings>(stream) ?? new ReminderSettings();
        }
    }

    /// <summary>Saves reminder settings.</summary>
    public Task SaveAsync(ReminderSettings settings) => WriteAsync(settings, true);

    /// <summary>Writes settings to a temporary file before replacing the saved file.</summary>
    private async Task WriteAsync(ReminderSettings settings, bool overwrite)
    {
        var path = Path.Combine(_fileSystem.AppDataDirectory, _fileName);
        var temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, true))
            {
                await JsonSerializer.SerializeAsync(stream, settings);
            }

            File.Move(temporaryPath, path, overwrite);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}
