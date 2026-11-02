namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Creates portable files from saved measurements.</summary>
public interface IFileExportService
{
    /// <summary>Creates a CSV backup for the specified date range and returns its path.</summary>
    Task<string> CreateBackupAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    /// <summary>Imports measurements from a selected CSV backup and returns whether a file was selected.</summary>
    Task<bool> ImportBackupAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates an HTML report for the specified date range and returns its path.</summary>
    Task<string> CreateReportAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
}
