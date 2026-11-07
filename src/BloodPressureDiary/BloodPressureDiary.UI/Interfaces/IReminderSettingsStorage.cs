using BloodPressureDiary.UI.Models;

namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Reads and writes reminder settings.</summary>
public interface IReminderSettingsStorage
{
    /// <summary>Reads saved reminder settings, including settings from earlier versions.</summary>
    Task<ReminderSettings> GetAsync();

    /// <summary>Saves reminder settings.</summary>
    Task SaveAsync(ReminderSettings settings);
}
