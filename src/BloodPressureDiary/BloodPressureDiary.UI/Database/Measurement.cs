namespace BloodPressureDiary.UI.Database;
/// <summary>Stores one manually entered blood pressure measurement.</summary>
public sealed class Measurement
{
    /// <summary>Gets or sets the identifier.</summary>
    public int Id { get; set; }
    /// <summary>Gets or sets the local measurement date and time.</summary>
    public DateTime RecordedAt { get; set; }
    /// <summary>Gets or sets systolic pressure in mmHg.</summary>
    public int Systolic { get; set; }
    /// <summary>Gets or sets diastolic pressure in mmHg.</summary>
    public int Diastolic { get; set; }
    /// <summary>Gets or sets the optional pulse in beats per minute.</summary>
    public int? Pulse { get; set; }
    /// <summary>Gets or sets the optional note.</summary>
    public string Note { get; set; } = string.Empty;
}
