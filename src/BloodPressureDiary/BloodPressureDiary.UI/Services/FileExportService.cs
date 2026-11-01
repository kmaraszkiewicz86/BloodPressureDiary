using System.Globalization;
using System.Net;
using System.Text;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;

namespace BloodPressureDiary.UI.Services;

/// <summary>Exports stored measurements to CSV and HTML files in the app cache.</summary>
public sealed class FileExportService(IBloodPressureMeasurementRepository _repository) : IFileExportService
{
    /// <inheritdoc />
    public async Task<string> CreateBackupAsync(CancellationToken cancellationToken = default)
    {
        var bloodPressureMeasurements = await _repository.GetAllAsync(cancellationToken);
        var builder = new StringBuilder();
        builder.AppendLine("MeasuredAt,Systolic,Diastolic,Pulse,Notes");

        foreach (var bloodPressureMeasurement in bloodPressureMeasurements)
        {
            builder.Append(Csv(bloodPressureMeasurement.MeasuredAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))).Append(',')
                .Append(bloodPressureMeasurement.Systolic.ToString(CultureInfo.InvariantCulture)).Append(',')
                .Append(bloodPressureMeasurement.Diastolic.ToString(CultureInfo.InvariantCulture)).Append(',')
                .Append(bloodPressureMeasurement.Pulse?.ToString(CultureInfo.InvariantCulture) ?? string.Empty).Append(',')
                .AppendLine(Csv(bloodPressureMeasurement.Notes ?? string.Empty));
        }

        return await WriteAsync("blood-pressure-backup", ".csv", builder.ToString(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> CreateReportAsync(CancellationToken cancellationToken = default)
    {
        var bloodPressureMeasurements = await _repository.GetAllAsync(cancellationToken);
        var builder = new StringBuilder();
        builder.Append("<!doctype html><html lang=\"").Append(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
            .AppendLine("\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>")
            .Append(WebUtility.HtmlEncode(AppResources.ReportTitle))
            .AppendLine("</title><style>body{font:16px system-ui,sans-serif;margin:2rem}table{border-collapse:collapse;width:100%}th,td{border:1px solid #bbb;padding:.55rem;text-align:left}th{background:#eef2f7}</style></head><body><h1>")
            .Append(WebUtility.HtmlEncode(AppResources.ReportTitle))
            .AppendLine("</h1><table><thead><tr>")
            .Append("<th>").Append(WebUtility.HtmlEncode(AppResources.DateAndTime)).Append("</th>")
            .Append("<th>").Append(WebUtility.HtmlEncode(AppResources.Systolic)).Append("</th>")
            .Append("<th>").Append(WebUtility.HtmlEncode(AppResources.Diastolic)).Append("</th>")
            .Append("<th>").Append(WebUtility.HtmlEncode(AppResources.Pulse)).Append("</th>")
            .Append("<th>").Append(WebUtility.HtmlEncode(AppResources.Notes)).AppendLine("</th></tr></thead><tbody>");

        foreach (var bloodPressureMeasurement in bloodPressureMeasurements)
        {
            builder.Append("<tr><td>").Append(WebUtility.HtmlEncode(bloodPressureMeasurement.MeasuredAt.ToString("g", CultureInfo.CurrentCulture)))
                .Append("</td><td>").Append(bloodPressureMeasurement.Systolic)
                .Append("</td><td>").Append(bloodPressureMeasurement.Diastolic)
                .Append("</td><td>").Append(bloodPressureMeasurement.Pulse?.ToString(CultureInfo.InvariantCulture) ?? string.Empty)
                .Append("</td><td>").Append(WebUtility.HtmlEncode(bloodPressureMeasurement.Notes ?? string.Empty)).AppendLine("</td></tr>");
        }

        builder.AppendLine("</tbody></table></body></html>");
        return await WriteAsync("blood-pressure-report", ".html", builder.ToString(), cancellationToken);
    }

    /// <summary>Escapes and quotes a CSV value.</summary>
    private static string Csv(string value) => $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";

    /// <summary>Writes the supplied contents to a cache file and returns its path.</summary>
    private static async Task<string> WriteAsync(string prefix, string extension, string contents, CancellationToken cancellationToken)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, $"{prefix}-{DateTime.Now:yyyyMMdd-HHmmss}{extension}");
        await File.WriteAllTextAsync(path, contents, new UTF8Encoding(true), cancellationToken);
        return path;
    }
}
