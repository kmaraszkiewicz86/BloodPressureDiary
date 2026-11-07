namespace BloodPressureDiary.UI.Models;

/// <summary>Represents the saved reminder settings.</summary>
public sealed class ReminderSettings
{
    /// <summary>Gets or sets whether reminders are enabled.</summary>
    public bool IsEnabled { get; set; }

    /// <summary>Gets or sets the saved reminder schedules.</summary>
    public List<ReminderSchedule> Reminders { get; set; } = [];
}
