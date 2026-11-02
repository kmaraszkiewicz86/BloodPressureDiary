using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Tests.Fixtures;
using Microsoft.Maui.Storage;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.Services;

/// <summary>Tests CSV backup and HTML report file operations.</summary>
public sealed class FileExportServiceTests(FileExportServiceFixture fixture) : IClassFixture<FileExportServiceFixture>
{
    /// <summary>Exports CSV that can be imported without losing quoted or multiline notes.</summary>
    [Fact]
    public async Task Backup_RoundTripsMeasurementWithQuotedMultilineNotes()
    {
        // Arrange
        fixture.Initialize();
        var measurement = fixture.CreateBloodPressureMeasurement("First, \"quoted\" line\r\nSecond line");
        fixture.Repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[] { measurement });
        var path = await fixture.Sut.CreateBackupAsync();
        fixture.FilePicker.PickAsync(Arg.Any<PickOptions>()).Returns(new FileResult(path));

        // Act
        var imported = await fixture.Sut.ImportBackupAsync();

        // Assert
        Assert.True(imported);
        await fixture.Repository.Received(1).AddRangeAsync(Arg.Is<IReadOnlyList<BloodPressureMeasurement>>(items =>
            items.Count == 1 && items[0].MeasuredAt == measurement.MeasuredAt && items[0].Systolic == 120 &&
            items[0].Diastolic == 80 && items[0].Pulse == 65 && items[0].Notes == measurement.Notes), Arg.Any<CancellationToken>());
    }

    /// <summary>Rejects an invalid backup before storing any measurements.</summary>
    [Fact]
    public async Task ImportBackupAsync_InvalidRow_DoesNotSaveAnyMeasurements()
    {
        // Arrange
        fixture.Initialize();
        var path = Path.Combine(fixture.DirectoryPath, "invalid.csv");
        await File.WriteAllTextAsync(path, "MeasuredAt,Systolic,Diastolic,Pulse,Notes\n2026-01-02 08:30:15,120,80,,ok\ninvalid,0,80,,bad\n");
        fixture.FilePicker.PickAsync(Arg.Any<PickOptions>()).Returns(new FileResult(path));

        // Act
        await Assert.ThrowsAsync<FormatException>(() => fixture.Sut.ImportBackupAsync());

        // Assert
        await fixture.Repository.DidNotReceive().AddRangeAsync(Arg.Any<IReadOnlyList<BloodPressureMeasurement>>(), Arg.Any<CancellationToken>());
    }

    /// <summary>Rejects a file without the expected backup columns.</summary>
    [Fact]
    public async Task ImportBackupAsync_InvalidHeader_DoesNotSaveMeasurements()
    {
        // Arrange
        fixture.Initialize();
        var path = Path.Combine(fixture.DirectoryPath, "invalid-header.csv");
        await File.WriteAllTextAsync(path, "wrong,columns\n");
        fixture.FilePicker.PickAsync(Arg.Any<PickOptions>()).Returns(new FileResult(path));

        // Act
        await Assert.ThrowsAsync<FormatException>(() => fixture.Sut.ImportBackupAsync());

        // Assert
        await fixture.Repository.DidNotReceive().AddRangeAsync(Arg.Any<IReadOnlyList<BloodPressureMeasurement>>(), Arg.Any<CancellationToken>());
    }

    /// <summary>Does not import anything when file selection is cancelled.</summary>
    [Fact]
    public async Task ImportBackupAsync_Cancelled_ReturnsFalse()
    {
        // Arrange
        fixture.Initialize();
        fixture.FilePicker.PickAsync(Arg.Any<PickOptions>()).Returns((FileResult?)null);

        // Act
        var imported = await fixture.Sut.ImportBackupAsync();

        // Assert
        Assert.False(imported);
        await fixture.Repository.DidNotReceive().AddRangeAsync(Arg.Any<IReadOnlyList<BloodPressureMeasurement>>(), Arg.Any<CancellationToken>());
    }

    /// <summary>Escapes measurement notes in HTML reports.</summary>
    [Fact]
    public async Task CreateReportAsync_EncodesNotes()
    {
        // Arrange
        fixture.Initialize();
        fixture.Repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[] { fixture.CreateBloodPressureMeasurement("<test & note>") });

        // Act
        var path = await fixture.Sut.CreateReportAsync();

        // Assert
        Assert.Contains("&lt;test &amp; note&gt;", await File.ReadAllTextAsync(path));
    }
}
