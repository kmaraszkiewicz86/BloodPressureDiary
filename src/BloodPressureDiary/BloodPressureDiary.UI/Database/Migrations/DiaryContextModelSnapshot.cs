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
