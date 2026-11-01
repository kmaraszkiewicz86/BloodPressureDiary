using BloodPressureDiary.UI.Tests.Fixtures;
using NSubstitute;
namespace BloodPressureDiary.UI.Tests;
/// <summary>Verifies reminder preferences and navigation.</summary>
public sealed class SettingsViewModelTests : IClassFixture<DiaryFixture>
{
    private readonly DiaryFixture fixture;
    /// <summary>Initializes the test fixture.</summary>
    public SettingsViewModelTests(DiaryFixture fixture) => this.fixture = fixture;
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    /// <summary>Verifies PreferencesPersistOnlyWhenSchedulingSucceeds.</summary>
    public async Task PreferencesPersistOnlyWhenSchedulingSucceeds(bool allowed)
    {
        // Arrange
        var state = fixture.CreateSettings();
        state.Sut.Enabled = true;
        state.Sut.Time = TimeSpan.FromHours(14);
        state.Reminders.ConfigureAsync(true, state.Sut.Time).Returns(allowed);
        // Act
        await state.Sut.SaveCommand.ExecuteAsync(null);
        // Assert
        state.Settings.Received(allowed ? 1 : 0).Save(true, TimeSpan.FromHours(14));
    }
    [Fact]
    /// <summary>Verifies DisableCancelsReminderBeforeSaving.</summary>
    public async Task DisableCancelsReminderBeforeSaving()
    {
        // Arrange
        var state = fixture.CreateSettings();
        state.Sut.Enabled = false;
        state.Sut.Time = TimeSpan.FromHours(8);
        state.Reminders.ConfigureAsync(false, state.Sut.Time).Returns(true);
        // Act
        await state.Sut.SaveCommand.ExecuteAsync(null);
        // Assert
        Received.InOrder(() =>
        {
            state.Reminders.ConfigureAsync(false, TimeSpan.FromHours(8));
            state.Settings.Save(false, TimeSpan.FromHours(8));
        });
    }
    [Fact]
    /// <summary>Verifies MeasurementsUsesNavigationInterface.</summary>
    public async Task MeasurementsUsesNavigationInterface()
    {
        // Arrange
        var state = fixture.CreateSettings();
        // Act
        await state.Sut.MeasurementsCommand.ExecuteAsync(null);
        // Assert
        await state.Navigation.Received().ShowMeasurementsAsync();
    }
}
