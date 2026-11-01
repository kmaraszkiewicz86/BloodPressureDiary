using BloodPressureDiary.UI.Interfaces;
namespace BloodPressureDiary.UI.Services;
/// <summary>Stores reminder preferences on the device.</summary>
public sealed class SettingsService : ISettingsService
{
    /// <summary>Gets whether daily reminders are enabled.</summary>
    public bool Enabled => Preferences.Default.Get("ReminderEnabled", false);
    /// <summary>Gets the local daily reminder time.</summary>
    public TimeSpan Time => TimeSpan.FromTicks(Preferences.Default.Get("ReminderTime", TimeSpan.FromHours(8).Ticks));
    /// <summary>Stores reminder preferences.</summary>
    public void Save(bool enabled, TimeSpan time)
    {
        Preferences.Default.Set("ReminderTime", time.Ticks);
        Preferences.Default.Set("ReminderEnabled", enabled);
    }
}
