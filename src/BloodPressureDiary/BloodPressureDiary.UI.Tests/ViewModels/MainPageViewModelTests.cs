using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Tests.Fixtures;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.ViewModels;

/// <summary>Tests main page measurement entry behavior.</summary>
public sealed class MainPageViewModelTests(MainPageViewModelFixture fixture) : IClassFixture<MainPageViewModelFixture>
{
    /// <summary>Opens the new measurement form.</summary>
    [Fact]
    public void AddNewItemCommand_OpensForm()
    {
        // Arrange
        fixture.Reset();

        // Act
        fixture.Sut.AddNewItemCommand.Execute(null);

        // Assert
        Assert.True(fixture.Sut.IsAddingBloodPressureMeasurement);
    }

    /// <summary>Closes the new measurement form and clears the entered values.</summary>
    [Fact]
    public void CancelNewItemCommand_ClosesFormAndClearsInputs()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.AddNewItemCommand.Execute(null);
        fixture.EnterValidBloodPressureMeasurement();

        // Act
        fixture.Sut.CancelNewItemCommand.Execute(null);

        // Assert
        Assert.False(fixture.Sut.IsAddingBloodPressureMeasurement);
        Assert.Empty(fixture.Sut.SystolicInput);
        Assert.Empty(fixture.Sut.DiastolicInput);
        Assert.Empty(fixture.Sut.PulseInput);
        Assert.Empty(fixture.Sut.NotesInput);
    }

    /// <summary>Saves a valid measurement and refreshes the list.</summary>
    [Fact]
    public async Task SaveCommand_StoresValidBloodPressureMeasurement()
    {
        // Arrange
        fixture.Reset();
        fixture.Repository.GetAllAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<BloodPressureMeasurement>>([]));
        fixture.Sut.AddNewItemCommand.Execute(null);
        fixture.EnterValidBloodPressureMeasurement();

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        await fixture.Repository.Received(1).AddAsync(Arg.Is<BloodPressureMeasurement>(bloodPressureMeasurement =>
            bloodPressureMeasurement.Systolic == 120 && bloodPressureMeasurement.Diastolic == 80 && bloodPressureMeasurement.Pulse == 65 && bloodPressureMeasurement.Notes == "Morning"),
            Arg.Any<CancellationToken>());
        Assert.False(fixture.Sut.IsAddingBloodPressureMeasurement);
    }

    /// <summary>Shows validation and does not save when a required pressure is invalid.</summary>
    [Fact]
    public async Task SaveCommand_WithInvalidPressure_ShowsValidationWithoutSaving()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.AddNewItemCommand.Execute(null);
        fixture.Sut.SystolicInput = "0";
        fixture.Sut.DiastolicInput = "80";

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        await fixture.Repository.DidNotReceive().AddAsync(Arg.Any<BloodPressureMeasurement>(), Arg.Any<CancellationToken>());
        await fixture.Interaction.Received(1).ShowMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        Assert.True(fixture.Sut.IsAddingBloodPressureMeasurement);
    }

    /// <summary>Loads the selected history date range from the repository.</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task FilterHistoryCommand_LoadsSelectedRangeFromRepository(int filter)
    {
        // Arrange
        fixture.Reset();
        DateTime? expectedStart = filter switch
        {
            1 => DateTime.Today,
            2 => DateTime.Today.AddDays(-6),
            3 => DateTime.Today.AddMonths(-1),
            _ => null
        };
        var expectedEnd = expectedStart.HasValue ? DateTime.Today.AddDays(1) : (DateTime?)null;

        // Act
        await fixture.Sut.FilterHistoryCommand.ExecuteAsync(filter);

        // Assert
        await fixture.Repository.Received(1).GetAllAsync(expectedStart, expectedEnd, Arg.Any<CancellationToken>());
    }

    /// <summary>Moves between pages and resets to the first page when the filter changes.</summary>
    [Fact]
    public async Task HistoryPages_ShowNextMeasurementsAndResetForNewFilter()
    {
        // Arrange
        fixture.Reset();
        fixture.Repository.GetAllAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>()).Returns(Enumerable.Range(1, 12)
            .Select(id => new BloodPressureMeasurement { Id = id, MeasuredAt = DateTime.Today.AddMinutes(id) }).ToArray());
        await fixture.Sut.LoadCommand.ExecuteAsync(null);

        // Act
        fixture.Sut.NextPageCommand.Execute(null);

        // Assert
        Assert.Equal(2, fixture.Sut.BloodPressureMeasurements.Count);
        Assert.True(fixture.Sut.HasPreviousPage);
        Assert.False(fixture.Sut.HasNextPage);
        await fixture.Sut.FilterHistoryCommand.ExecuteAsync(1);
        Assert.Equal(10, fixture.Sut.BloodPressureMeasurements.Count);
        Assert.False(fixture.Sut.HasPreviousPage);
        Assert.True(fixture.Sut.HasNextPage);
    }

    /// <summary>Returns to the preceding page when the final measurement on the last page is deleted.</summary>
    [Fact]
    public async Task DeleteCommand_LastMeasurementOnPage_ReturnsToPreviousPage()
    {
        // Arrange
        fixture.Reset();
        var measurements = Enumerable.Range(1, 11)
            .Select(id => new BloodPressureMeasurement { Id = id, MeasuredAt = DateTime.Today.AddMinutes(id) }).ToArray();
        fixture.Repository.GetAllAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>()).Returns(measurements, measurements.TakeLast(10).ToArray());
        fixture.Interaction.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        await fixture.Sut.LoadCommand.ExecuteAsync(null);
        fixture.Sut.NextPageCommand.Execute(null);
        var row = fixture.Sut.BloodPressureMeasurements.Single();

        // Act
        await fixture.Sut.DeleteCommand.ExecuteAsync(row);

        // Assert
        Assert.Equal(10, fixture.Sut.BloodPressureMeasurements.Count);
        Assert.False(fixture.Sut.HasPreviousPage);
        Assert.False(fixture.Sut.HasNextPage);
    }

    /// <summary>Refreshes the history after a successful backup import.</summary>
    [Fact]
    public async Task ImportBackupCommand_Success_RefreshesHistory()
    {
        // Arrange
        fixture.Reset();
        fixture.ExportService.ImportBackupAsync(Arg.Any<CancellationToken>()).Returns(true);
        fixture.Repository.GetAllAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>()).Returns(new[] { new BloodPressureMeasurement { MeasuredAt = DateTime.Today } });

        // Act
        await fixture.Sut.ImportBackupCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(fixture.Sut.BloodPressureMeasurements);
        await fixture.Interaction.Received(1).ShowMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    /// <summary>Displays an error when backup import fails.</summary>
    [Fact]
    public async Task ImportBackupCommand_Failure_ShowsError()
    {
        // Arrange
        fixture.Reset();
        fixture.ExportService.ImportBackupAsync(Arg.Any<CancellationToken>()).Returns(Task.FromException<bool>(new FormatException()));

        // Act
        await fixture.Sut.ImportBackupCommand.ExecuteAsync(null);

        // Assert
        await fixture.Interaction.Received(1).ShowMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        await fixture.Repository.DidNotReceive().GetAllAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>());
    }
}
