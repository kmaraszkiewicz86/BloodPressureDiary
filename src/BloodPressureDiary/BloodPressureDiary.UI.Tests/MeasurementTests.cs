using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Tests.Fixtures;
namespace BloodPressureDiary.UI.Tests;
/// <summary>Verifies validation and persistence.</summary>
public sealed class MeasurementTests : IClassFixture<DiaryFixture>
{
    private readonly DiaryFixture fixture;
    /// <summary>Initializes the test fixture.</summary>
    public MeasurementTests(DiaryFixture fixture) => this.fixture = fixture;
    [Theory]
    [InlineData("", "80", "")]
    [InlineData("120", "0", "")]
    [InlineData("80", "120", "")]
    [InlineData("301", "80", "")]
    [InlineData("120", "80", "abc")]
    [InlineData("120", "80", "301")]
    /// <summary>Verifies InvalidValuesAreRejected.</summary>
    public void InvalidValuesAreRejected(string systolic, string diastolic, string pulse)
    {
        // Arrange
        var row = fixture.CreateRow(); row.Systolic = systolic; row.Diastolic = diastolic; row.Pulse = pulse;
        // Act
        var result = row.ToMeasurement();
        // Assert
        Assert.Null(result);
    }
    [Fact]
    /// <summary>Verifies OptionalPulseMayBeEmpty.</summary>
    public void OptionalPulseMayBeEmpty()
    {
        // Arrange
        var row = fixture.CreateRow();
        // Act
        var result = row.ToMeasurement();
        // Assert
        Assert.NotNull(result); Assert.Null(result.Pulse);
    }
    [Fact]
    /// <summary>Verifies StoreInsertsMeasurement.</summary>
    public async Task StoreInsertsMeasurement()
    {
        // Arrange
        var store = fixture.CreateStore(); var measurement = fixture.CreateRow().ToMeasurement()!;
        // Act
        await store.SaveAsync(measurement);
        // Assert
        var saved = Assert.Single(await store.LoadAsync()); Assert.Equal(120, saved.Systolic);
    }
    [Fact]
    /// <summary>Verifies StoreUpdatesMeasurement.</summary>
    public async Task StoreUpdatesMeasurement()
    {
        // Arrange
        var store = fixture.CreateStore(); var measurement = fixture.CreateRow().ToMeasurement()!;
        await store.SaveAsync(measurement); measurement.Note = "updated";
        // Act
        await store.SaveAsync(measurement);
        // Assert
        Assert.Equal("updated", Assert.Single(await store.LoadAsync()).Note);
    }
    [Fact]
    /// <summary>Verifies StoreDeletesMeasurement.</summary>
    public async Task StoreDeletesMeasurement()
    {
        // Arrange
        var store = fixture.CreateStore(); var measurement = fixture.CreateRow().ToMeasurement()!;
        await store.SaveAsync(measurement);
        // Act
        await store.DeleteAsync(measurement.Id);
        // Assert
        Assert.Empty(await store.LoadAsync());
    }
    [Fact]
    /// <summary>Verifies StoreOrdersNewestFirst.</summary>
    public async Task StoreOrdersNewestFirst()
    {
        // Arrange
        var store = fixture.CreateStore(); var older = fixture.CreateRow().ToMeasurement()!;
        var newer = fixture.CreateRow().ToMeasurement()!; newer.RecordedAt = older.RecordedAt.AddDays(1);
        await store.SaveAsync(older); await store.SaveAsync(newer);
        // Act
        var result = await store.LoadAsync();
        // Assert
        Assert.Equal(newer.Id, result[0].Id);
    }


}
