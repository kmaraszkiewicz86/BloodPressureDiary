using BloodPressureDiary.UI.Interfaces;

namespace BloodPressureDiary.UI.Services;

/// <summary>Provides a no-op reminder implementation on platforms without local reminder support.</summary>
public sealed class UnsupportedReminderScheduler : IReminderScheduler
{
    /// <inheritdoc />
    public Task<bool> ScheduleDailyAsync(TimeSpan time, bool requestPermission) => Task.FromResult(false);

    /// <inheritdoc />
    public Task CancelAsync() => Task.CompletedTask;
}
