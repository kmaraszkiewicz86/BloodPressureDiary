using System.Globalization;
using BloodPressureDiary.UI.Database;
using CommunityToolkit.Mvvm.ComponentModel;
using BloodPressureDiary.UI.Resources.Localization;
namespace BloodPressureDiary.UI.ViewModels;
/// <summary>Provides an editable measurement row.</summary>
public sealed class MeasurementRowViewModel : ObservableObject
{
    private string systolic = "", diastolic = "", pulse = "", note = "";
    private DateTime date = DateTime.Today;
    private TimeSpan time = DateTime.Now.TimeOfDay;
    private bool isEditing;
    /// <summary>Gets the stored identifier.</summary>
    public int Id { get; private set; }
    /// <summary>Gets or sets systolic input.</summary>
    public string Systolic { get => systolic; set => SetProperty(ref systolic, value); }
    /// <summary>Gets or sets diastolic input.</summary>
    public string Diastolic { get => diastolic; set => SetProperty(ref diastolic, value); }
    /// <summary>Gets or sets optional pulse input.</summary>
    public string Pulse { get => pulse; set => SetProperty(ref pulse, value); }
    /// <summary>Gets or sets the note.</summary>
    public string Note { get => note; set => SetProperty(ref note, value); }
    /// <summary>Gets or sets the measurement date.</summary>
    public DateTime Date { get => date; set => SetProperty(ref date, value); }
    /// <summary>Gets or sets the measurement time.</summary>
    public TimeSpan Time { get => time; set => SetProperty(ref time, value); }
    /// <summary>Gets or sets whether this row is editable.</summary>
    public bool IsEditing { get => isEditing; set { if (SetProperty(ref isEditing, value)) OnPropertyChanged(nameof(IsReadOnly)); } }
    /// <summary>Gets whether this row is read only.</summary>
    public bool IsReadOnly => !IsEditing;
    /// <summary>Copies persisted values into the row.</summary>
    public void Load(Measurement measurement)
    {
        Id = measurement.Id; Date = measurement.RecordedAt.Date; Time = measurement.RecordedAt.TimeOfDay;
        Systolic = measurement.Systolic.ToString(CultureInfo.CurrentCulture);
        Diastolic = measurement.Diastolic.ToString(CultureInfo.CurrentCulture);
        Pulse = measurement.Pulse?.ToString(CultureInfo.CurrentCulture) ?? ""; Note = measurement.Note;
    }
    /// <summary>Validates input and creates a measurement.</summary>
    public Measurement? ToMeasurement()
    {
        if (!int.TryParse(Systolic, out var s) || s < 1 || s > 300 ||
            !int.TryParse(Diastolic, out var d) || d < 1 || d > 300 || s <= d ||
            (!string.IsNullOrWhiteSpace(Pulse) && (!int.TryParse(Pulse, out var pulseValue) || pulseValue < 1 || pulseValue > 300)) ||
            Note.Length > 1000 || Time < TimeSpan.Zero || Time >= TimeSpan.FromDays(1)) return null;
        return new Measurement { Id = Id, RecordedAt = Date.Date + Time, Systolic = s, Diastolic = d,
            Pulse = string.IsNullOrWhiteSpace(Pulse) ? null : int.Parse(Pulse), Note = Note.Trim() };
    }
}
