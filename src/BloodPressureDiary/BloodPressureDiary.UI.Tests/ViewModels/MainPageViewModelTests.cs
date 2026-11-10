using BloodPressureDiary.UI.Database.Entities;
using BloodPressureDiary.UI.Resources.Localization;
using BloodPressureDiary.UI.Tests.Fixtures;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.ViewModels;

/// <summary>Tests main page measurement entry behavior.</summary>
public sealed class MainPageViewModelTests(MainPageViewModelFixture fixture) : IClassFixture<MainPageViewModelFixture>
{
    /// <summary>Opens the selected export options.</summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ExportCommand_OpensSelectedOptions(bool report)
    {
        // Arrange
        fixture.Reset();
        // Act
        if (report)
        {
            fixture.Sut.ExportReportCommand.Execute(null);
        }
        else
        {
            fixture.Sut.ExportBackupCommand.Execute(null);
        }

        // Assert
        Assert.True(fixture.Sut.IsExporting);
        Assert.Equal(report ? AppResources.ExportReport : AppResources.ExportBackup, fixture.Sut.ExportWindowTitle);
    }

    /// <summary>Opens the new measurement form.</summary>
    [Fact]
    public void AddBloodPressureMeasurementCommand_OpensForm()
    {
        // Arrange
        fixture.Reset();

        // Act
        fixture.Sut.AddBloodPressureMeasurementCommand.Execute(null);

        // Assert
        Assert.True(fixture.Sut.IsAddingBloodPressureMeasurement);
    }

    /// <summary>Closes the new measurement form and clears the entered values.</summary>
    [Fact]
    public void CancelNewItemCommand_ClosesFormAndClearsInputs()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.AddBloodPressureMeasurementCommand.Execute(null);
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
        fixture.Sut.AddBloodPressureMeasurementCommand.Execute(null);
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
        fixture.Sut.AddBloodPressureMeasurementCommand.Execute(null);
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

    /// <summary>Combines a custom history range with the selected period.</summary>
    [Fact]
    public async Task ApplyHistoryDateRangeCommand_CombinesDateRangeAndPeriod()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.SelectedHistoryFilter = 2;
        fixture.Sut.UseHistoryDateRange = true;
        fixture.Sut.HistoryStartDate = DateTime.Today.AddDays(-20);
        fixture.Sut.HistoryEndDate = DateTime.Today.AddDays(-2);

        // Act
        await fixture.Sut.ApplyHistoryDateRangeCommand.ExecuteAsync(null);

        // Assert
        await fixture.Repository.Received(1).GetAllAsync(DateTime.Today.AddDays(-6), DateTime.Today.AddDays(-1), Arg.Any<CancellationToken>());
    }

    /// <summary>Rejects a future date in the history range.</summary>
    [Fact]
    public async Task ApplyHistoryDateRangeCommand_FutureDate_ShowsValidation()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.UseHistoryDateRange = true;
        fixture.Sut.HistoryEndDate = DateTime.Today.AddDays(1);

        // Act
        await fixture.Sut.ApplyHistoryDateRangeCommand.ExecuteAsync(null);

        // Assert
        await fixture.Interaction.Received(1).ShowMessageAsync(AppResources.ValidationTitle, AppResources.InvalidDateRange, AppResources.Cancel);
        await fixture.Repository.DidNotReceive().GetAllAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>());
    }

    /// <summary>Rejects a history range whose start is after its end.</summary>
    [Fact]
    public async Task ApplyHistoryDateRangeCommand_ReversedRange_ShowsValidation()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.UseHistoryDateRange = true;
        fixture.Sut.HistoryStartDate = DateTime.Today;
        fixture.Sut.HistoryEndDate = DateTime.Today.AddDays(-1);

        // Act
        await fixture.Sut.ApplyHistoryDateRangeCommand.ExecuteAsync(null);

        // Assert
        await fixture.Interaction.Received(1).ShowMessageAsync(AppResources.ValidationTitle, AppResources.InvalidDateRange, AppResources.Cancel);
        await fixture.Repository.DidNotReceive().GetAllAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>());
    }

    /// <summary>Creates an HTML report using the selected period and custom date range.</summary>
    [Fact]
    public async Task ConfirmExportCommand_Report_UsesCombinedDateRange()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.ExportReportCommand.Execute(null);
        fixture.Sut.SelectedExportFilter = 2;
        fixture.Sut.UseExportDateRange = true;
        fixture.Sut.ExportStartDate = DateTime.Today.AddDays(-3);
        fixture.Sut.ExportEndDate = DateTime.Today.AddDays(-1);
        fixture.ExportService.CreateReportAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>()).Returns("report.html");

        // Act
        await fixture.Sut.ConfirmExportCommand.ExecuteAsync(null);

        // Assert
        await fixture.ExportService.Received(1).CreateReportAsync(DateTime.Today.AddDays(-3), DateTime.Today, Arg.Any<CancellationToken>());
        await fixture.SharingService.Received(1).ShareAsync("report.html", AppResources.ShareReport);
        Assert.False(fixture.Sut.IsExporting);
    }

    /// <summary>Creates a CSV backup for the selected export range.</summary>
    [Fact]
    public async Task ConfirmExportCommand_Backup_UsesSelectedDateRange()
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.ExportBackupCommand.Execute(null);
        fixture.Sut.UseExportDateRange = true;
        fixture.Sut.ExportStartDate = DateTime.Today.AddDays(-5);
        fixture.Sut.ExportEndDate = DateTime.Today.AddDays(-1);
        fixture.ExportService.CreateBackupAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>()).Returns("backup.csv");

        // Act
        await fixture.Sut.ConfirmExportCommand.ExecuteAsync(null);

        // Assert
        await fixture.ExportService.Received(1).CreateBackupAsync(DateTime.Today.AddDays(-5), DateTime.Today, Arg.Any<CancellationToken>());
        await fixture.SharingService.Received(1).ShareAsync("backup.csv", AppResources.ShareBackup);
    }

    /// <summary>Rejects invalid dates without creating an export file.</summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConfirmExportCommand_InvalidDateRange_ShowsValidation(bool futureDate)
    {
        // Arrange
        fixture.Reset();
        fixture.Sut.ExportReportCommand.Execute(null);
        fixture.Sut.UseExportDateRange = true;
        fixture.Sut.ExportStartDate = DateTime.Today;
        fixture.Sut.ExportEndDate = DateTime.Today.AddDays(futureDate ? 1 : -1);

        // Act
        await fixture.Sut.ConfirmExportCommand.ExecuteAsync(null);

        // Assert
        await fixture.Interaction.Received(1).ShowMessageAsync(AppResources.ValidationTitle, AppResources.InvalidDateRange, AppResources.Cancel);
        await fixture.ExportService.DidNotReceive().CreateReportAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>());
        Assert.True(fixture.Sut.IsExporting);
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
