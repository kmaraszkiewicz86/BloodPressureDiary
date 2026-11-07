using BloodPressureDiary.UI.Models;

namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Calculates reminder schedule occurrences.</summary>
public interface IReminderScheduleCalculator
{
    /// <summary>Returns the next occurrence after the specified local date and time.</summary>
    DateTime GetNextOccurrence(ReminderSchedule reminder, DateTime now);
}
