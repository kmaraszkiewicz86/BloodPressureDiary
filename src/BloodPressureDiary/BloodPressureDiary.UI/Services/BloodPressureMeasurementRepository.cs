using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Database.Entities;
using BloodPressureDiary.UI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodPressureDiary.UI.Services;

/// <summary>Implements measurement storage with Entity Framework Core.</summary>
public sealed class BloodPressureMeasurementRepository(DiaryDbContext _context) : IBloodPressureMeasurementRepository
{
    /// <summary>Returns measurements ordered from newest to oldest.</summary>
    public Task<IReadOnlyList<BloodPressureMeasurement>> GetAllAsync(CancellationToken cancellationToken = default) =>
        GetAllAsync(null, null, cancellationToken);

    /// <summary>Returns measurements within the specified date range ordered from newest to oldest.</summary>
    public async Task<IReadOnlyList<BloodPressureMeasurement>> GetAllAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var query = _context.BloodPressureMeasurements.AsNoTracking();
        if (startDate.HasValue)
        {
            query = query.Where(measurement => measurement.MeasuredAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(measurement => measurement.MeasuredAt < endDate.Value);
        }

        return await query
            .OrderByDescending(bloodPressureMeasurement => bloodPressureMeasurement.MeasuredAt)
            .ThenByDescending(bloodPressureMeasurement => bloodPressureMeasurement.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>Checks whether a measurement exists within the specified date range.</summary>
    public Task<bool> ExistsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default) =>
        _context.BloodPressureMeasurements.AsNoTracking()
            .AnyAsync(measurement => measurement.MeasuredAt >= startDate && measurement.MeasuredAt < endDate, cancellationToken);

    /// <summary>Adds a measurement.</summary>
    public async Task AddAsync(BloodPressureMeasurement bloodPressureMeasurement, CancellationToken cancellationToken = default)
    {
        await _context.BloodPressureMeasurements.AddAsync(bloodPressureMeasurement, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }

    /// <summary>Adds a collection of measurements.</summary>
    public async Task AddRangeAsync(IReadOnlyList<BloodPressureMeasurement> bloodPressureMeasurements, CancellationToken cancellationToken = default)
    {
        await _context.BloodPressureMeasurements.AddRangeAsync(bloodPressureMeasurements, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }

    /// <summary>Updates a measurement.</summary>
    public async Task UpdateAsync(BloodPressureMeasurement bloodPressureMeasurement, CancellationToken cancellationToken = default)
    {
        _context.BloodPressureMeasurements.Update(bloodPressureMeasurement);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }

    /// <summary>Removes a measurement by identifier.</summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var bloodPressureMeasurement = await _context.BloodPressureMeasurements.FindAsync([id], cancellationToken);
        if (bloodPressureMeasurement is null)
        {
            return;
        }

        _context.BloodPressureMeasurements.Remove(bloodPressureMeasurement);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }
}
