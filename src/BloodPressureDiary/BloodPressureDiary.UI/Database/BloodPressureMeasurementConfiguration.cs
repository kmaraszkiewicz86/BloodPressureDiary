using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodPressureDiary.UI.Database;

/// <summary>Configures persisted blood pressure measurements.</summary>
public sealed class BloodPressureMeasurementConfiguration : IEntityTypeConfiguration<BloodPressureMeasurement>
{
    /// <summary>Configures the measurement table and its properties.</summary>
    public void Configure(EntityTypeBuilder<BloodPressureMeasurement> builder)
    {
        builder.ToTable("BloodPressureMeasurements");
        builder.HasKey(measurement => measurement.Id);
        builder.Property(measurement => measurement.Note).IsRequired().HasMaxLength(1000);
        builder.HasIndex(measurement => measurement.CreatedDate);
    }
}
