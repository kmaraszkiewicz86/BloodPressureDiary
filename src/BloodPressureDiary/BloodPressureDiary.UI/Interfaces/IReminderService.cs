namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Stores reminder preferences and manages the daily local reminder.</summary>
public interface IReminderService
{
    /// <summary>Gets whether daily reminders are enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Gets the configured reminder time.</summary>
    TimeSpan ReminderTime { get; }

    /// <summary>Updates the reminder preference and schedule.</summary>
    Task<bool> SetReminderAsync(bool enabled, TimeSpan time, bool requestPermission = true);

    /// <summary>Restores a previously enabled reminder without prompting for permission.</summary>
    Task RestoreAsync();
}

/// <summary>Schedules local platform notifications.</summary>
public interface IReminderScheduler
{
    /// <summary>Schedules a daily notification and reports whether notifications can be shown.</summary>
    Task<bool> ScheduleDailyAsync(TimeSpan time, bool requestPermission);

    /// <summary>Cancels the daily notification.</summary>
    Task CancelAsync();
}
