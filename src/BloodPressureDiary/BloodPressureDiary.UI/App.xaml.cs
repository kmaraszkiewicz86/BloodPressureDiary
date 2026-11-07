using BloodPressureDiary.UI.Interfaces;

namespace BloodPressureDiary.UI;

/// <summary>Provides the application window.</summary>
public partial class App(AppShell _appShell, IReminderService _reminderService) : Application
{
    private bool _isInitialized;

    /// <summary>Creates the application window and restores reminder alarms when it is created.</summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (!_isInitialized)
        {
            InitializeComponent();
            _isInitialized = true;
        }

        var window = new Window(_appShell);
        window.Created += OnWindowCreated;
        return window;
    }

    /// <summary>Restores reminder alarms when the application window is created.</summary>
    private async void OnWindowCreated(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            window.Created -= OnWindowCreated;
        }

        await _reminderService.RestoreAsync();
    }
}
