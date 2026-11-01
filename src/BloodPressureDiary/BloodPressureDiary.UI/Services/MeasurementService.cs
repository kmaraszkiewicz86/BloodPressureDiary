using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BloodPressureDiary.UI.Services;
/// <summary>Persists measurements using EF Core.</summary>
public sealed class MeasurementService : IMeasurementService
{
    private readonly IDbContextFactory<DiaryContext> factory;
    /// <summary>Initializes the measurement service.</summary>
    public MeasurementService(IDbContextFactory<DiaryContext> factory) => this.factory = factory;
    private async Task<DiaryContext> OpenAsync()
    {
        var context = await factory.CreateDbContextAsync();
        try
        {
            if (context.Database.IsRelational()) await context.Database.MigrateAsync();
            return context;
        }
        catch { await context.DisposeAsync(); throw; }
    }
    /// <summary>Loads measurements from newest to oldest.</summary>
    public async Task<IReadOnlyList<Measurement>> LoadAsync()
    {
        await using var context = await OpenAsync();
        return await context.Measurements.AsNoTracking().OrderByDescending(x => x.RecordedAt).ThenByDescending(x => x.Id).ToListAsync();
    }
    /// <summary>Inserts or updates a measurement.</summary>
    public async Task SaveAsync(Measurement measurement)
    {
        await using var context = await OpenAsync();
        if (measurement.Id == 0) context.Add(measurement);
        else context.Update(measurement);
        await context.SaveChangesAsync();
    }
    /// <summary>Deletes a measurement by identifier.</summary>
    public async Task DeleteAsync(int id)
    {
        await using var context = await OpenAsync();
        var measurement = await context.Measurements.FindAsync(id);
        if (measurement is null) return;
        context.Remove(measurement);
        await context.SaveChangesAsync();
    }
}
