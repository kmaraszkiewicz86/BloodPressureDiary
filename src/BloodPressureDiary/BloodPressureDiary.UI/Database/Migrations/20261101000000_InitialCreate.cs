using BloodPressureDiary.UI.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BloodPressureDiary.UI.Database.Migrations;

/// <summary>Creates the initial blood pressure measurement database schema.</summary>
[DbContext(typeof(DiaryDbContext))]
[Migration("20261101000000_InitialCreate")]
public sealed class InitialCreate : Migration
{
    /// <summary>Creates the measurements table and its date index.</summary>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Measurements",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Systolic = table.Column<int>(type: "INTEGER", nullable: false),
                Diastolic = table.Column<int>(type: "INTEGER", nullable: false),
                Pulse = table.Column<int>(type: "INTEGER", nullable: true),
                Notes = table.Column<string>(type: "TEXT", nullable: true),
                MeasuredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Measurements", bloodPressureMeasurement => bloodPressureMeasurement.Id));

        migrationBuilder.CreateIndex(name: "IX_Measurements_MeasuredAt", table: "Measurements", column: "MeasuredAt");
    }

    /// <summary>Removes the measurements table.</summary>
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Measurements");
}
