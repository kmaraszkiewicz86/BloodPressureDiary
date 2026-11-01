using Microsoft.EntityFrameworkCore;

namespace BloodPressureDiary.UI.Database.Migrations;

internal static class DiaryModel
{
    internal static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");
        modelBuilder.Entity<BloodPressureMeasurement>(entity =>
        {
            entity.ToTable("BloodPressureMeasurements");
            entity.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnType("INTEGER");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CreatedDate).HasColumnType("TEXT");
            entity.Property(x => x.Systolic).HasColumnType("INTEGER");
            entity.Property(x => x.Diastolic).HasColumnType("INTEGER");
            entity.Property(x => x.Pulse).HasColumnType("INTEGER");
            entity.Property(x => x.Note).IsRequired().HasMaxLength(1000).HasColumnType("TEXT");
            entity.HasIndex(x => x.CreatedDate);
        });
    }
}
