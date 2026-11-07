using BloodPressureDiary.UI.Database.Entities;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Services;
using Microsoft.Maui.Storage;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.Fixtures;

/// <summary>Creates fresh file service dependencies and temporary files for each test.</summary>
public sealed class FileExportServiceFixture : TestBase, IDisposable
{
    private TestState _state = new();

    /// <summary>Gets the repository substitute.</summary>
    public IBloodPressureMeasurementRepository Repository => _state.Repository;
    /// <summary>Gets the file picker substitute.</summary>
    public IFilePicker FilePicker => _state.FilePicker;
    /// <summary>Gets the service under test.</summary>
    public FileExportService Sut => _state.Sut;
    /// <summary>Gets the temporary directory for the current test.</summary>
    public string DirectoryPath => _state.DirectoryPath;

    /// <summary>Creates fresh dependencies and a temporary directory.</summary>
    public void Initialize()
    {
        _state.Dispose();
        _state = new TestState();
    }

    /// <summary>Creates a test measurement.</summary>
    public BloodPressureMeasurement CreateBloodPressureMeasurement(string? notes = null) => new()
    {
        MeasuredAt = new DateTime(2026, 1, 2, 8, 30, 15), Systolic = 120, Diastolic = 80, Pulse = 65, Notes = notes
    };

    /// <summary>Deletes the temporary directory used by the current test.</summary>
    public void Dispose() => _state.Dispose();

    private sealed class TestState : TestBase, IDisposable
    {
        /// <summary>Gets the repository substitute.</summary>
        public IBloodPressureMeasurementRepository Repository { get; }
        /// <summary>Gets the file picker substitute.</summary>
        public IFilePicker FilePicker { get; }
        /// <summary>Gets the temporary file directory.</summary>
        public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "blood-pressure-tests-" + Guid.NewGuid().ToString("N"));
        /// <summary>Gets the service under test.</summary>
        public FileExportService Sut { get; }

        /// <summary>Creates the service and its substitutes.</summary>
        public TestState()
        {
            Directory.CreateDirectory(DirectoryPath);
            Repository = Freeze<IBloodPressureMeasurementRepository>();
            FilePicker = Freeze<IFilePicker>();
            Freeze<IFileSystem>().CacheDirectory.Returns(DirectoryPath);
            Sut = Create<FileExportService>();
        }

        /// <summary>Deletes the temporary file directory.</summary>
        public void Dispose() => Directory.Delete(DirectoryPath, true);
    }
}
