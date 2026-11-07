using BloodPressureDiary.UI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BloodPressureDiary.UI.Database.Migrations;

/// <summary>Represents the current database model snapshot.</summary>
[DbContext(typeof(DiaryDbContext))]
public sealed class DiaryDbContextModelSnapshot : ModelSnapshot
{
    /// <summary>Builds the saved database model snapshot.</summary>
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.14")
            .HasAnnotation("Relational:MaxIdentifierLength", 64);

        modelBuilder.Entity("BloodPressureDiary.UI.Database.Entities.BloodPressureMeasurement", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            entity.Property<int>("Diastolic").HasColumnType("INTEGER");
            entity.Property<DateTime>("MeasuredAt").HasColumnType("TEXT");
            entity.Property<string>("Notes").HasColumnType("TEXT");
            entity.Property<int?>("Pulse").HasColumnType("INTEGER");
            entity.Property<int>("Systolic").HasColumnType("INTEGER");
            entity.HasKey("Id");
            entity.HasIndex("MeasuredAt");
            entity.ToTable("Measurements");
        });
    }
}
