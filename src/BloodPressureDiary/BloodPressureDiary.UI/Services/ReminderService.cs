using BloodPressureDiary.UI.Interfaces;
using Microsoft.Maui.Storage;
using System.Globalization;

namespace BloodPressureDiary.UI.Services;

/// <summary>Persists reminder settings and delegates scheduling to the platform.</summary>
public sealed class ReminderService(IReminderScheduler _scheduler) : IReminderService
{
    private const string _enabledKey = "daily-reminder-enabled";
    private const string _timeKey = "daily-reminder-time";

    /// <inheritdoc />
    public bool IsEnabled => Preferences.Default.Get(_enabledKey, false);

    /// <inheritdoc />
    public TimeSpan ReminderTime => TimeSpan.TryParse(Preferences.Default.Get(_timeKey, "20:00:00"), CultureInfo.InvariantCulture, out var time)
        ? time
        : new TimeSpan(20, 0, 0);

    /// <inheritdoc />
    public async Task<bool> SetReminderAsync(bool enabled, TimeSpan time, bool requestPermission = true)
    {
        if (!enabled)
        {
            Preferences.Default.Set(_enabledKey, false);
            Preferences.Default.Set(_timeKey, time.ToString("c", CultureInfo.InvariantCulture));
            await _scheduler.CancelAsync();
            return true;
        }

        if (!await _scheduler.ScheduleDailyAsync(time, requestPermission))
        {
            return false;
        }

        Preferences.Default.Set(_timeKey, time.ToString("c", CultureInfo.InvariantCulture));
        Preferences.Default.Set(_enabledKey, true);
        return true;
    }

    /// <inheritdoc />
    public async Task RestoreAsync()
    {
        if (!IsEnabled)
        {
            await _scheduler.CancelAsync();
            return;
        }

        var savedTime = Preferences.Default.Get(_timeKey, string.Empty);
        if (!TimeSpan.TryParse(savedTime, CultureInfo.InvariantCulture, out var time))
        {
            Preferences.Default.Set(_enabledKey, false);
            await _scheduler.CancelAsync();
            return;
        }

        await _scheduler.ScheduleDailyAsync(time, false);
    }
}
