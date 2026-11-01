using Microsoft.EntityFrameworkCore;

namespace BloodPressureDiary.UI.Database;

/// <summary>Provides access to the local blood pressure diary database.</summary>
public sealed class DiaryDbContext(DbContextOptions<DiaryDbContext> _options) : DbContext(_options)
{
    /// <summary>Gets the saved blood pressure measurements.</summary>
    public DbSet<BloodPressureMeasurement> BloodPressureMeasurements => Set<BloodPressureMeasurement>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BloodPressureMeasurementConfiguration());
    }
}
