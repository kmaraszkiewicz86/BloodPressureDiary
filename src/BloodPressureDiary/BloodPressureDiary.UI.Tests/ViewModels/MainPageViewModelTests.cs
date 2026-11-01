using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Tests.Fixtures;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.ViewModels;

/// <summary>Tests main page measurement entry behavior.</summary>
public sealed class MainPageViewModelTests(MainPageViewModelFixture fixture) : IClassFixture<MainPageViewModelFixture>
{
    /// <summary>Saves a valid measurement and refreshes the list.</summary>
    [Fact]
    public async Task SaveCommand_StoresValidBloodPressureMeasurement()
    {
        // Arrange
        fixture.Reset();
        fixture.Repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<BloodPressureMeasurement>>([]));
        fixture.EnterValidBloodPressureMeasurement();

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        await fixture.Repository.Received(1).AddAsync(Arg.Is<BloodPressureMeasurement>(bloodPressureMeasurement =>
            bloodPressureMeasurement.Systolic == 120 && bloodPressureMeasurement.Diastolic == 80 && bloodPressureMeasurement.Pulse == 65 && bloodPressureMeasurement.Notes == "Morning"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>Shows validation and does not save when a required pressure is invalid.</summary>
    [Fact]
    public async Task SaveCommand_WithInvalidPressure_ShowsValidationWithoutSaving()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.SystolicInput = "0";
        fixture.Sut.DiastolicInput = "80";

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        await fixture.Repository.DidNotReceive().AddAsync(Arg.Any<BloodPressureMeasurement>(), Arg.Any<CancellationToken>());
        await fixture.Interaction.Received(1).ShowMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }
}
