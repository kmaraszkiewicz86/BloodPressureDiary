using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BloodPressureDiary.UI.Services;
/// <summary>Persists measurements using EF Core.</summary>
public sealed class BloodPressureMeasurementService : IBloodPressureMeasurementService
{
    private readonly IDbContextFactory<DiaryContext> factory;
    /// <summary>Initializes the measurement service.</summary>
    public BloodPressureMeasurementService(IDbContextFactory<DiaryContext> factory) => this.factory = factory;
    /// <summary>Loads measurements from newest to oldest.</summary>
    public async Task<IReadOnlyList<BloodPressureMeasurement>> LoadAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BloodPressureMeasurements
            .AsNoTracking()
            .OrderByDescending(measurement => measurement.CreatedDate)
            .ThenByDescending(measurement => measurement.Id)
            .ToListAsync();
    }
    /// <summary>Inserts or updates a measurement.</summary>
    public async Task SaveAsync(BloodPressureMeasurement measurement)
    {
        await using var context = await factory.CreateDbContextAsync();
        if (measurement.Id == 0)
        {
            context.Add(measurement);
        }
        else
        {
            context.Update(measurement);
        }
        await context.SaveChangesAsync();
    }
    /// <summary>Deletes a measurement by identifier.</summary>
    public async Task DeleteAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var measurement = await context.BloodPressureMeasurements.FindAsync(id);
        if (measurement is null)
        {
            return;
        }
        context.Remove(measurement);
        await context.SaveChangesAsync();
    }
}
