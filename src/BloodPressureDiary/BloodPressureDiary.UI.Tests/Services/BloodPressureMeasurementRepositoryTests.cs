using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Tests.Fixtures;

namespace BloodPressureDiary.UI.Tests.Services;

/// <summary>Tests measurement repository behavior.</summary>
public sealed class BloodPressureMeasurementRepositoryTests(BloodPressureMeasurementRepositoryFixture fixture) : IClassFixture<BloodPressureMeasurementRepositoryFixture>
{
    /// <summary>Returns stored measurements from newest to oldest.</summary>
    [Fact]
    public async Task GetAllAsync_OrdersBloodPressureMeasurementsFromNewestToOldest()
    {
        // Arrange
        fixture.Initialize();
        await fixture.Sut.AddAsync(fixture.CreateBloodPressureMeasurement(new DateTime(2026, 1, 1, 8, 0, 0)));
        await fixture.Sut.AddAsync(fixture.CreateBloodPressureMeasurement(new DateTime(2026, 1, 2, 8, 0, 0), systolic: 130, diastolic: 85));

        // Act
        var bloodPressureMeasurements = await fixture.Sut.GetAllAsync();

        // Assert
        Assert.Equal(130, bloodPressureMeasurements[0].Systolic);
        Assert.Equal(120, bloodPressureMeasurements[1].Systolic);
    }
}
