using BloodPressureDiary.UI.Interfaces;
namespace BloodPressureDiary.UI.Services;
/// <summary>Navigates between shell tabs.</summary>
public sealed class NavigationService : INavigationService
{
    /// <summary>Opens the measurements tab.</summary>
    public Task ShowMeasurementsAsync() => Shell.Current.GoToAsync("//measurements");
    /// <summary>Opens the settings tab.</summary>
    public Task ShowSettingsAsync() => Shell.Current.GoToAsync("//settings");
}
