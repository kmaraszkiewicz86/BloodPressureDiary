namespace BloodPressureDiary.UI.Interfaces;
/// <summary>Defines SettingsService operations.</summary>
public interface ISettingsService
{
    /// <summary>Gets whether daily reminders are enabled.</summary>
    bool Enabled { get; }
    /// <summary>Gets the local daily reminder time.</summary>
    TimeSpan Time { get; }
    /// <summary>Stores reminder preferences.</summary>
    void Save(bool enabled, TimeSpan time);
}
