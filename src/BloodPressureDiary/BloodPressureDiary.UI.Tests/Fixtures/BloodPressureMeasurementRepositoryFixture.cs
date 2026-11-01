using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Services;

namespace BloodPressureDiary.UI.Tests.Fixtures;

/// <summary>Creates an isolated database and repository for each repository test.</summary>
public sealed class BloodPressureMeasurementRepositoryFixture : TestBase, IDisposable
{
    private DiaryDbContext? _context;

    /// <summary>Gets the current repository system under test.</summary>
    public BloodPressureMeasurementRepository Sut { get; private set; } = null!;

    /// <summary>Creates a measurement record for repository tests.</summary>
    public BloodPressureMeasurement CreateBloodPressureMeasurement(DateTime measuredAt, int systolic = 120, int diastolic = 80)
    {
        var bloodPressureMeasurement = Create<BloodPressureMeasurement>();
        bloodPressureMeasurement.Id = 0;
        bloodPressureMeasurement.Systolic = systolic;
        bloodPressureMeasurement.Diastolic = diastolic;
        bloodPressureMeasurement.Pulse = null;
        bloodPressureMeasurement.Notes = null;
        bloodPressureMeasurement.MeasuredAt = measuredAt;
        return bloodPressureMeasurement;
    }

    /// <summary>Creates fresh database state and a repository.</summary>
    public void Initialize()
    {
        _context?.Dispose();
        _context = new DiaryDbContext(CreateInMemoryOptions<DiaryDbContext>());
        Sut = new BloodPressureMeasurementRepository(_context);
    }

    /// <inheritdoc />
    public void Dispose() => _context?.Dispose();
}
