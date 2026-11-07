using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Models;

namespace BloodPressureDiary.UI.Services;

/// <summary>Calculates recurring reminder dates from the saved start date.</summary>
public sealed class ReminderScheduleCalculator : IReminderScheduleCalculator
{
    /// <summary>Returns the next occurrence after the specified local date and time.</summary>
    public DateTime GetNextOccurrence(ReminderSchedule reminder, DateTime now)
    {
        var startDate = reminder.StartDate.Date;
        var daysSinceStart = (now.Date - startDate).Days;
        var nextDate = startDate;
        if (daysSinceStart >= 0)
        {
            var daysUntilNext = (reminder.IntervalDays - daysSinceStart % reminder.IntervalDays) % reminder.IntervalDays;
            nextDate = now.Date.AddDays(daysUntilNext);
        }

        var next = nextDate.Add(reminder.Time);
        return next > now ? next : next.AddDays(reminder.IntervalDays);
    }
}
