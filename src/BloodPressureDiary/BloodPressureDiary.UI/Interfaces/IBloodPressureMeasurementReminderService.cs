namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Determines whether a blood pressure measurement reminder is needed.</summary>
public interface IBloodPressureMeasurementReminderService
{
    /// <summary>Checks whether a reminder should be shown when no measurement exists today.</summary>
    Task<bool> ShouldShowReminderAsync(CancellationToken cancellationToken = default);
}
