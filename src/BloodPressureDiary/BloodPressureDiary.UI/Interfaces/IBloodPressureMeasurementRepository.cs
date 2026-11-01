using BloodPressureDiary.UI.Database;

namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Reads and writes blood pressure measurements.</summary>
public interface IBloodPressureMeasurementRepository
{
    /// <summary>Returns measurements ordered from newest to oldest.</summary>
    Task<IReadOnlyList<BloodPressureMeasurement>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a measurement.</summary>
    Task AddAsync(BloodPressureMeasurement bloodPressureMeasurement, CancellationToken cancellationToken = default);

    /// <summary>Updates a measurement.</summary>
    Task UpdateAsync(BloodPressureMeasurement bloodPressureMeasurement, CancellationToken cancellationToken = default);

    /// <summary>Removes a measurement by identifier.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
