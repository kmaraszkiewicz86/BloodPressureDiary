using BloodPressureDiary.UI.Tests.Fixtures;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.Services;

/// <summary>Tests whether a measurement reminder is needed.</summary>
public sealed class BloodPressureMeasurementReminderServiceTests(BloodPressureMeasurementReminderServiceFixture fixture) : IClassFixture<BloodPressureMeasurementReminderServiceFixture>
{
    /// <summary>Shows a reminder only when no measurement exists today.</summary>
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task ShouldShowReminderAsync_UsesTodaysMeasurementStatus(bool measurementExists, bool expected)
    {
        // Arrange
        fixture.Initialize();
        var today = DateTime.Today;
        fixture.Repository.ExistsAsync(today, today.AddDays(1), Arg.Any<CancellationToken>()).Returns(measurementExists);

        // Act
        var shouldShow = await fixture.Sut.ShouldShowReminderAsync();

        // Assert
        Assert.Equal(expected, shouldShow);
        await fixture.Repository.Received(1).ExistsAsync(today, today.AddDays(1), Arg.Any<CancellationToken>());
    }
}
