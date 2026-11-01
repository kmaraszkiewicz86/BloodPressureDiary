using System.Globalization;
using BloodPressureDiary.UI.Database;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BloodPressureDiary.UI.ViewModels;

/// <summary>Holds a measurement and its temporary inline edit values.</summary>
public sealed class BloodPressureMeasurementRowViewModel : ObservableObject
{
    /// <summary>Creates a row for a saved measurement.</summary>
    public BloodPressureMeasurementRowViewModel(BloodPressureMeasurement bloodPressureMeasurement)
    {
        BloodPressureMeasurement = bloodPressureMeasurement;
        SystolicInput = bloodPressureMeasurement.Systolic.ToString(CultureInfo.InvariantCulture);
        DiastolicInput = bloodPressureMeasurement.Diastolic.ToString(CultureInfo.InvariantCulture);
        PulseInput = bloodPressureMeasurement.Pulse?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        NotesInput = bloodPressureMeasurement.Notes ?? string.Empty;
        MeasuredDate = bloodPressureMeasurement.MeasuredAt.Date;
        MeasuredTime = bloodPressureMeasurement.MeasuredAt.TimeOfDay;
    }

    /// <summary>Gets the saved measurement.</summary>
    public BloodPressureMeasurement BloodPressureMeasurement { get; }

    /// <summary>Gets or sets whether the row is being edited.</summary>
    public bool IsEditing
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(IsNotEditing));
            }
        }
    }
    /// <summary>Gets whether the row is currently displayed outside edit mode.</summary>
    public bool IsNotEditing => !IsEditing;
    /// <summary>Gets or sets the editable systolic value.</summary>
    public string SystolicInput { get; set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets or sets the editable diastolic value.</summary>
    public string DiastolicInput { get; set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets or sets the editable pulse value.</summary>
    public string PulseInput { get; set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets or sets the editable note.</summary>
    public string NotesInput { get; set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets or sets the editable date.</summary>
    public DateTime MeasuredDate { get; set => SetProperty(ref field, value); }
    /// <summary>Gets or sets the editable time.</summary>
    public TimeSpan MeasuredTime { get; set => SetProperty(ref field, value); }
    /// <summary>Gets a localized date and time for display.</summary>
    public string DisplayDateTime => BloodPressureMeasurement.MeasuredAt.ToString("g", CultureInfo.CurrentCulture);
    /// <summary>Gets the optional pulse display value.</summary>
    public string PulseDisplay => BloodPressureMeasurement.Pulse?.ToString(CultureInfo.CurrentCulture) ?? "—";
    /// <summary>Gets the note display value.</summary>
    public string NotesDisplay => BloodPressureMeasurement.Notes ?? string.Empty;

    /// <summary>Copies current values into the inline editor.</summary>
    public void BeginEdit()
    {
        SystolicInput = BloodPressureMeasurement.Systolic.ToString(CultureInfo.InvariantCulture);
        DiastolicInput = BloodPressureMeasurement.Diastolic.ToString(CultureInfo.InvariantCulture);
        PulseInput = BloodPressureMeasurement.Pulse?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        NotesInput = BloodPressureMeasurement.Notes ?? string.Empty;
        MeasuredDate = BloodPressureMeasurement.MeasuredAt.Date;
        MeasuredTime = BloodPressureMeasurement.MeasuredAt.TimeOfDay;
        IsEditing = true;
    }

    /// <summary>Discards inline edits.</summary>
    public void CancelEdit() => IsEditing = false;

    /// <summary>Updates the saved values and ends inline editing.</summary>
    public void ApplyEdit(int systolic, int diastolic, int? pulse)
    {
        BloodPressureMeasurement.Systolic = systolic;
        BloodPressureMeasurement.Diastolic = diastolic;
        BloodPressureMeasurement.Pulse = pulse;
        BloodPressureMeasurement.Notes = string.IsNullOrWhiteSpace(NotesInput) ? null : NotesInput.Trim();
        BloodPressureMeasurement.MeasuredAt = MeasuredDate.Date.Add(MeasuredTime);
        OnPropertyChanged(nameof(DisplayDateTime));
        OnPropertyChanged(nameof(PulseDisplay));
        OnPropertyChanged(nameof(NotesDisplay));
        IsEditing = false;
    }
}
