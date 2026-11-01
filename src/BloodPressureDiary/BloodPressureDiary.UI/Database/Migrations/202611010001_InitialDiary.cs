using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace BloodPressureDiary.UI.Database.Migrations;
/// <summary>Creates the measurement table and date index.</summary>
[DbContext(typeof(DiaryContext))]
[Migration("202611010001_InitialDiary")]
public sealed class InitialDiary : Migration
{
    /// <summary>Creates diary storage.</summary>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("BloodPressureMeasurements", columns: table => new
        {
            Id = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
            CreatedDate = table.Column<DateTime>(nullable: false),
            Systolic = table.Column<int>(nullable: false),
            Diastolic = table.Column<int>(nullable: false),
            Pulse = table.Column<int>(nullable: true),
            Note = table.Column<string>(maxLength: 1000, nullable: false)
        }, constraints: table => table.PrimaryKey("PK_BloodPressureMeasurements", x => x.Id));
        migrationBuilder.CreateIndex("IX_BloodPressureMeasurements_CreatedDate", "BloodPressureMeasurements", "CreatedDate");
    }
    /// <summary>Removes diary storage.</summary>
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("BloodPressureMeasurements");
    /// <summary>Describes the initial database model.</summary>
    protected override void BuildTargetModel(ModelBuilder modelBuilder) => DiaryModel.Configure(modelBuilder);
}
