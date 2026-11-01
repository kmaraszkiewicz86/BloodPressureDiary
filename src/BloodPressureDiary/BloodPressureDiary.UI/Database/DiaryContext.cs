using Microsoft.EntityFrameworkCore;
namespace BloodPressureDiary.UI.Database;
/// <summary>Provides access to the diary database.</summary>
public sealed class DiaryContext : DbContext
{
    /// <summary>Initializes the context with injected options.</summary>
    public DiaryContext(DbContextOptions<DiaryContext> options) : base(options) { }
    /// <summary>Gets the recorded measurements.</summary>
    public DbSet<Measurement> Measurements => Set<Measurement>();
    /// <summary>Configures the measurement table.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Measurement>();
        entity.ToTable("Measurements");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Note).IsRequired().HasMaxLength(1000);
        entity.HasIndex(x => x.RecordedAt);
    }
}
