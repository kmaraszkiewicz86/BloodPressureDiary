using BloodPressureDiary.UI.Database.Entities;
using BloodPressureDiary.UI.Database.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace BloodPressureDiary.UI.Database;

/// <summary>Provides access to the local blood pressure diary database.</summary>
public sealed class DiaryDbContext(DbContextOptions<DiaryDbContext> _options) : DbContext(_options)
{
    /// <summary>Gets the saved blood pressure measurements.</summary>
    /// <remarks>The context provides the set without a setter or separate initialization; measurements can still be added, updated, and removed.</remarks>
    public DbSet<BloodPressureMeasurement> BloodPressureMeasurements => Set<BloodPressureMeasurement>();

    /// <summary>Applies the blood pressure measurement entity configuration.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BloodPressureMeasurementConfiguration());
    }
}
