using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Services;

namespace BloodPressureDiary.UI.Tests.Fixtures;

/// <summary>Creates fresh dependencies for reminder eligibility tests.</summary>
public sealed class BloodPressureMeasurementReminderServiceFixture : TestBase
{
    private TestState _state = new();

    /// <summary>Gets the measurement repository substitute.</summary>
    public IBloodPressureMeasurementRepository Repository => _state.Repository;

    /// <summary>Gets the reminder service under test.</summary>
    public BloodPressureMeasurementReminderService Sut => _state.Sut;

    /// <summary>Creates fresh dependencies and a reminder service.</summary>
    public void Initialize() => _state = new TestState();

    private sealed class TestState : TestBase
    {
        /// <summary>Gets the measurement repository substitute.</summary>
        public IBloodPressureMeasurementRepository Repository { get; }

        /// <summary>Gets the reminder service under test.</summary>
        public BloodPressureMeasurementReminderService Sut { get; }

        /// <summary>Creates the repository substitute and the reminder service.</summary>
        public TestState()
        {
            Repository = Freeze<IBloodPressureMeasurementRepository>();
            Sut = Create<BloodPressureMeasurementReminderService>();
        }
    }
}
