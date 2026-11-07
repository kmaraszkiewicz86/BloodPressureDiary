using BloodPressureDiary.UI.Interfaces;

namespace BloodPressureDiary.UI.Services;

/// <summary>Checks whether a blood pressure measurement reminder is needed today.</summary>
public sealed class BloodPressureMeasurementReminderService(IBloodPressureMeasurementRepository _repository) : IBloodPressureMeasurementReminderService
{
    /// <summary>Checks whether a reminder should be shown when no measurement exists today.</summary>
    public async Task<bool> ShouldShowReminderAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        return !await _repository.ExistsAsync(today, today.AddDays(1), cancellationToken);
    }
}
