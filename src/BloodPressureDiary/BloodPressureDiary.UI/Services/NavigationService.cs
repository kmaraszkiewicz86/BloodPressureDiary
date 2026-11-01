using BloodPressureDiary.UI.Interfaces;

namespace BloodPressureDiary.UI.Services;

/// <summary>Navigates between the measurements and settings tabs.</summary>
public sealed class NavigationService : INavigationService
{
    /// <inheritdoc />
    public Task OpenSettingsAsync() => Shell.Current.GoToAsync("//SettingsPage");

    /// <inheritdoc />
    public Task OpenBloodPressureMeasurementsAsync() => Shell.Current.GoToAsync("//MainPage");
}
