using BloodPressureDiary.UI.Database;
namespace BloodPressureDiary.UI.Interfaces;
/// <summary>Defines MeasurementService operations.</summary>
public interface IMeasurementService
{
    /// <summary>Loads measurements from newest to oldest.</summary>
    Task<IReadOnlyList<Measurement>> LoadAsync();
    /// <summary>Inserts or updates a measurement.</summary>
    Task SaveAsync(Measurement measurement);
    /// <summary>Deletes a measurement by identifier.</summary>
    Task DeleteAsync(int id);
}
