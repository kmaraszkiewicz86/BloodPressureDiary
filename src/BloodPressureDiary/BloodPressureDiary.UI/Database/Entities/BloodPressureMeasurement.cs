namespace BloodPressureDiary.UI.Database.Entities;

/// <summary>Represents a manually recorded blood pressure measurement.</summary>
public sealed class BloodPressureMeasurement
{
    /// <summary>Gets or sets the measurement identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets systolic pressure in mmHg.</summary>
    public int Systolic { get; set; }

    /// <summary>Gets or sets diastolic pressure in mmHg.</summary>
    public int Diastolic { get; set; }

    /// <summary>Gets or sets the optional pulse in beats per minute.</summary>
    public int? Pulse { get; set; }

    /// <summary>Gets or sets the optional note.</summary>
    public string? Notes { get; set; }

    /// <summary>Gets or sets the local date and time of the measurement.</summary>
    public DateTime MeasuredAt { get; set; }
}
