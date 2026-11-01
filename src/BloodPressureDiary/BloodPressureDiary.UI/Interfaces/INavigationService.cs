namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Provides application navigation actions.</summary>
public interface INavigationService
{
    /// <summary>Opens the settings tab.</summary>
    Task OpenSettingsAsync();

    /// <summary>Opens the measurements tab.</summary>
    Task OpenBloodPressureMeasurementsAsync();
}
