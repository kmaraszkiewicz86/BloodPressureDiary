using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace BloodPressureDiary.UI.Database.Migrations;
/// <summary>Describes the current diary database schema.</summary>
[DbContext(typeof(DiaryContext))]
public sealed class DiaryContextModelSnapshot : Microsoft.EntityFrameworkCore.Infrastructure.ModelSnapshot
{
    /// <summary>Builds the current database model.</summary>
    protected override void BuildModel(ModelBuilder modelBuilder) => DiaryModel.Configure(modelBuilder);
}
internal static class DiaryModel
{
    internal static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");
        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.ToTable("Measurements");
            entity.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnType("INTEGER");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RecordedAt).HasColumnType("TEXT");
            entity.Property(x => x.Systolic).HasColumnType("INTEGER");
            entity.Property(x => x.Diastolic).HasColumnType("INTEGER");
            entity.Property(x => x.Pulse).HasColumnType("INTEGER");
            entity.Property(x => x.Note).IsRequired().HasMaxLength(1000).HasColumnType("TEXT");
            entity.HasIndex(x => x.RecordedAt);
        });
    }
}
