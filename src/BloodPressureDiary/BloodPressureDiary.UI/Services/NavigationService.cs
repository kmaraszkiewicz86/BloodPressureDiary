using BloodPressureDiary.UI.Interfaces;

namespace BloodPressureDiary.UI.Services;

/// <summary>Navigates between the measurements and settings tabs.</summary>
public sealed class NavigationService : INavigationService
{
    /// <summary>Opens the settings tab.</summary>
    public Task OpenSettingsAsync() => Shell.Current.GoToAsync("//SettingsPage");

    /// <summary>Opens the measurements tab.</summary>
    public Task OpenBloodPressureMeasurementsAsync() => Shell.Current.GoToAsync("//MainPage");
}
