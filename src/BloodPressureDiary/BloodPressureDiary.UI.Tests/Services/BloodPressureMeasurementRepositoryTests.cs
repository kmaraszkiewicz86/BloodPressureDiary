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

    /// <summary>Returns only measurements inside the requested date range.</summary>
    [Fact]
    public async Task GetAllAsync_WithDateRange_FiltersMeasurements()
    {
        // Arrange
        fixture.Initialize();
        var start = new DateTime(2026, 1, 2);
        var end = new DateTime(2026, 1, 4);
        await fixture.Sut.AddAsync(fixture.CreateBloodPressureMeasurement(start.AddTicks(-1)));
        await fixture.Sut.AddAsync(fixture.CreateBloodPressureMeasurement(start));
        await fixture.Sut.AddAsync(fixture.CreateBloodPressureMeasurement(end.AddTicks(-1)));
        await fixture.Sut.AddAsync(fixture.CreateBloodPressureMeasurement(end));

        // Act
        var measurements = await fixture.Sut.GetAllAsync(start, end);

        // Assert
        Assert.Equal(2, measurements.Count);
        Assert.Equal(end.AddTicks(-1), measurements[0].MeasuredAt);
        Assert.Equal(start, measurements[1].MeasuredAt);
    }

    /// <summary>Saves every measurement in an imported collection.</summary>
    [Fact]
    public async Task AddRangeAsync_StoresMeasurements()
    {
        // Arrange
        fixture.Initialize();
        var measurements = new[]
        {
            fixture.CreateBloodPressureMeasurement(new DateTime(2026, 1, 1)),
            fixture.CreateBloodPressureMeasurement(new DateTime(2026, 1, 2))
        };

        // Act
        await fixture.Sut.AddRangeAsync(measurements);

        // Assert
        Assert.Equal(2, (await fixture.Sut.GetAllAsync()).Count);
    }
}
