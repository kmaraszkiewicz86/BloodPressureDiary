namespace BloodPressureDiary.UI;
/// <summary>Creates the diary application window.</summary>
public partial class App : Application
{
    private readonly AppShell shell;
    /// <summary>Initializes application resources and shell.</summary>
    public App(AppShell shell)
    {
        InitializeComponent();
        this.shell = shell;
    }
    /// <summary>Creates the main application window.</summary>
    protected override Window CreateWindow(IActivationState? activationState) => new(shell);
}
