namespace BloodPressureDiary.UI.Interfaces;
/// <summary>Defines NavigationService operations.</summary>
public interface INavigationService
{
    /// <summary>Opens the measurements tab.</summary>
    Task ShowMeasurementsAsync();
    /// <summary>Opens the settings tab.</summary>
    Task ShowSettingsAsync();
}
