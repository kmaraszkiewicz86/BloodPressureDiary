using Microsoft.EntityFrameworkCore;

namespace BloodPressureDiary.UI.Database;

/// <summary>Provides access to the diary database.</summary>
public sealed class DiaryContext : DbContext
{
    /// <summary>Initializes the context with injected options.</summary>
    public DiaryContext(DbContextOptions<DiaryContext> options) : base(options)
    {
    }

    /// <summary>Gets the recorded blood pressure measurements.</summary>
    public DbSet<BloodPressureMeasurement> BloodPressureMeasurements => Set<BloodPressureMeasurement>();
}
