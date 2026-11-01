namespace BloodPressureDiary.UI.Interfaces;
/// <summary>Defines ReminderService operations.</summary>
public interface IReminderService
{
    /// <summary>Requests notification access and schedules or cancels the daily reminder.</summary>
    Task<bool> ConfigureAsync(bool enabled, TimeSpan time);
}
