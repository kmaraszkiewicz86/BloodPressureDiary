using BloodPressureDiary.UI.Database;
namespace BloodPressureDiary.UI.Interfaces;
/// <summary>Defines BloodPressureMeasurementService operations.</summary>
public interface IBloodPressureMeasurementService
{
    /// <summary>Loads measurements from newest to oldest.</summary>
    Task<IReadOnlyList<BloodPressureMeasurement>> LoadAsync();
    /// <summary>Inserts or updates a measurement.</summary>
    Task SaveAsync(BloodPressureMeasurement measurement);
    /// <summary>Deletes a measurement by identifier.</summary>
    Task DeleteAsync(int id);
}
