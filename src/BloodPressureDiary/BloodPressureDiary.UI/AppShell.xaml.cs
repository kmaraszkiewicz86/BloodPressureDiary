namespace BloodPressureDiary.UI;
/// <summary>Hosts measurement and settings tabs.</summary>
public partial class AppShell : Shell
{
    /// <summary>Initializes tabs with injected views.</summary>
    public AppShell(MainPage measurements, SettingsPage settings)
    {
        InitializeComponent(); MeasurementsTab.Content = measurements; SettingsTab.Content = settings;
    }
}
