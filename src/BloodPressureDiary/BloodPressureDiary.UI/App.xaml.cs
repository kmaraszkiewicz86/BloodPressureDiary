namespace BloodPressureDiary.UI;

/// <summary>Provides the application window.</summary>
public partial class App(AppShell _appShell) : Application
{
    private bool _isInitialized;

    /// <inheritdoc />
    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (!_isInitialized)
        {
            InitializeComponent();
            _isInitialized = true;
        }

        return new(_appShell);
    }
}
