using BloodPressureDiary.UI.Models;

namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Stores and manages local reminder schedules.</summary>
public interface IReminderService
{
    /// <summary>Gets whether reminders are enabled.</summary>
    Task<bool> IsEnabledAsync();

    /// <summary>Gets the configured reminder schedules.</summary>
    Task<IReadOnlyList<ReminderSchedule>> GetRemindersAsync();

    /// <summary>Updates the reminder preference and schedules.</summary>
    Task<bool> SetRemindersAsync(bool enabled, IReadOnlyList<ReminderSchedule> reminders, bool requestPermission = true);

    /// <summary>Restores previously enabled reminders without prompting for permission.</summary>
    Task RestoreAsync();

    /// <summary>Schedules the next occurrence of a configured reminder when enabled.</summary>
    Task<bool> RescheduleAsync(int reminderId);
}
