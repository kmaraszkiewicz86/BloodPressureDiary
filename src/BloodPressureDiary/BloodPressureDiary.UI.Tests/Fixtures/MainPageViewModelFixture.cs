using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.ViewModels;

namespace BloodPressureDiary.UI.Tests.Fixtures;

/// <summary>Creates fresh service substitutes and a main page view model for each test.</summary>
public sealed class MainPageViewModelFixture : TestBase
{
    private TestState _state = new();

    /// <summary>Gets the measurement repository substitute.</summary>
    public IBloodPressureMeasurementRepository Repository => _state.Repository;
    /// <summary>Gets the user interaction substitute.</summary>
    public IUserInteractionService Interaction => _state.Interaction;
    /// <summary>Gets the file export substitute.</summary>
    public IFileExportService ExportService => _state.ExportService;
    /// <summary>Gets the file sharing substitute.</summary>
    public IFileSharingService SharingService => _state.SharingService;
    /// <summary>Gets the navigation substitute.</summary>
    public INavigationService NavigationService => _state.NavigationService;
    /// <summary>Gets the view model system under test.</summary>
    public MainPageViewModel Sut => _state.Sut;

    /// <summary>Fills the view model with a valid measurement for save tests.</summary>
    public void EnterValidBloodPressureMeasurement()
    {
        Sut.SystolicInput = "120";
        Sut.DiastolicInput = "80";
        Sut.PulseInput = "65";
        Sut.NotesInput = "Morning";
    }

    /// <summary>Creates fresh mocks and the view model.</summary>
    public void Reset() => _state = new TestState();

    private sealed class TestState : TestBase
    {
        /// <summary>Gets the measurement repository substitute.</summary>
        public IBloodPressureMeasurementRepository Repository { get; }
        /// <summary>Gets the user interaction substitute.</summary>
        public IUserInteractionService Interaction { get; }
        /// <summary>Gets the file export substitute.</summary>
        public IFileExportService ExportService { get; }
        /// <summary>Gets the file sharing substitute.</summary>
        public IFileSharingService SharingService { get; }
        /// <summary>Gets the navigation substitute.</summary>
        public INavigationService NavigationService { get; }
        /// <summary>Gets the main page ViewModel system under test.</summary>
        public MainPageViewModel Sut { get; }

        /// <summary>Creates fresh substitutes and a main page ViewModel.</summary>
        public TestState()
        {
            Repository = Freeze<IBloodPressureMeasurementRepository>();
            Interaction = Freeze<IUserInteractionService>();
            ExportService = Freeze<IFileExportService>();
            SharingService = Freeze<IFileSharingService>();
            NavigationService = Freeze<INavigationService>();
            Sut = Create<MainPageViewModel>();
        }
    }
}
