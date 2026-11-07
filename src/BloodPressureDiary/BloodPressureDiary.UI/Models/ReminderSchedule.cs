using System.Globalization;
using System.Text.Json.Serialization;
using BloodPressureDiary.UI.Resources.Localization;

namespace BloodPressureDiary.UI.Models;

/// <summary>Represents a recurring blood pressure measurement reminder.</summary>
public sealed class ReminderSchedule
{
    /// <summary>Gets or sets the reminder identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the reminder time.</summary>
    public TimeSpan Time { get; set; }

    /// <summary>Gets the reminder time for display.</summary>
    [JsonIgnore]
    public string DisplayTime => Time.ToString(@"hh\:mm", CultureInfo.InvariantCulture);

    /// <summary>Gets or sets the interval between reminder days.</summary>
    public int IntervalDays { get; set; } = 1;

    /// <summary>Gets the reminder interval for display.</summary>
    [JsonIgnore]
    public string DisplayInterval => string.Format(CultureInfo.CurrentCulture, AppResources.ReminderIntervalFormat, IntervalDays);

    /// <summary>Gets or sets the first day of the reminder schedule.</summary>
    public DateTime StartDate { get; set; } = DateTime.Today;
}
