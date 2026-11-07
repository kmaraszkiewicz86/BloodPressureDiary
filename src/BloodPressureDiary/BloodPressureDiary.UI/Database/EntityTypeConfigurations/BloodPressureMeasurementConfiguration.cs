using BloodPressureDiary.UI.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodPressureDiary.UI.Database.EntityTypeConfigurations;

/// <summary>Configures the SQLite mapping for blood pressure measurements.</summary>
public sealed class BloodPressureMeasurementConfiguration : IEntityTypeConfiguration<BloodPressureMeasurement>
{
    /// <summary>Configures the database mapping for blood pressure measurements.</summary>
    public void Configure(EntityTypeBuilder<BloodPressureMeasurement> builder)
    {
        builder.ToTable("Measurements");
        builder.HasKey(bloodPressureMeasurement => bloodPressureMeasurement.Id);
        builder.Property(bloodPressureMeasurement => bloodPressureMeasurement.Systolic).IsRequired();
        builder.Property(bloodPressureMeasurement => bloodPressureMeasurement.Diastolic).IsRequired();
        builder.Property(bloodPressureMeasurement => bloodPressureMeasurement.Notes);
        builder.Property(bloodPressureMeasurement => bloodPressureMeasurement.MeasuredAt).IsRequired();
        builder.HasIndex(bloodPressureMeasurement => bloodPressureMeasurement.MeasuredAt);
    }
}
