namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Creates portable files from saved measurements.</summary>
public interface IFileExportService
{
    /// <summary>Creates a CSV backup and returns its path.</summary>
    Task<string> CreateBackupAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates an HTML report and returns its path.</summary>
    Task<string> CreateReportAsync(CancellationToken cancellationToken = default);
}
