using BloodPressureDiary.UI.Models;

namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Schedules local platform notifications.</summary>
public interface IReminderScheduler
{
    /// <summary>Checks whether notifications can be shown, requesting permission when required.</summary>
    Task<bool> CheckPermissionAsync(bool requestPermission);

    /// <summary>Schedules the next occurrence of a reminder.</summary>
    void Schedule(ReminderSchedule reminder);

    /// <summary>Cancels a reminder by identifier.</summary>
    void Cancel(int reminderId);
}
