using System.Globalization;
using System.Net;
using System.Text;
using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;
using Microsoft.VisualBasic.FileIO;

namespace BloodPressureDiary.UI.Services;

/// <summary>Exports measurements to CSV and HTML files and imports CSV backups.</summary>
public sealed class FileExportService(IBloodPressureMeasurementRepository _repository, IFilePicker _filePicker, IFileSystem _fileSystem) : IFileExportService
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
    public async Task<bool> ImportBackupAsync(CancellationToken cancellationToken = default)
    {
        var file = await _filePicker.PickAsync(new PickOptions { PickerTitle = AppResources.ImportBackup });
        if (file is null)
        {
            return false;
        }

        await using var stream = await file.OpenReadAsync();
        using var parser = new TextFieldParser(stream, Encoding.UTF8) { TextFieldType = FieldType.Delimited, HasFieldsEnclosedInQuotes = true, TrimWhiteSpace = false };
        parser.SetDelimiters(",");
        var header = parser.ReadFields();
        if (header is null || !header.SequenceEqual(new[] { "MeasuredAt", "Systolic", "Diastolic", "Pulse", "Notes" }))
        {
            throw new FormatException("Invalid backup header.");
        }

        var measurements = new List<BloodPressureMeasurement>();
        while (!parser.EndOfData)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var values = parser.ReadFields();
            if (values is null || values.Length != 5
                || !DateTime.TryParseExact(values[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var measuredAt)
                || !int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var systolic) || systolic <= 0
                || !int.TryParse(values[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var diastolic) || diastolic <= 0)
            {
                throw new FormatException("Invalid backup measurement.");
            }

            int? pulse = null;
            if (!string.IsNullOrEmpty(values[3]))
            {
                if (!int.TryParse(values[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedPulse) || parsedPulse <= 0)
                {
                    throw new FormatException("Invalid backup pulse.");
                }
                pulse = parsedPulse;
            }

            measurements.Add(new BloodPressureMeasurement
            {
                MeasuredAt = measuredAt, Systolic = systolic, Diastolic = diastolic,
                Pulse = pulse, Notes = string.IsNullOrEmpty(values[4]) ? null : values[4]
            });
        }

        await _repository.AddRangeAsync(measurements, cancellationToken);
        return true;
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
    private async Task<string> WriteAsync(string prefix, string extension, string contents, CancellationToken cancellationToken)
    {
        var path = Path.Combine(_fileSystem.CacheDirectory, $"{prefix}-{DateTime.Now:yyyyMMdd-HHmmss}{extension}");
        await File.WriteAllTextAsync(path, contents, new UTF8Encoding(true), cancellationToken);
        return path;
    }
}
