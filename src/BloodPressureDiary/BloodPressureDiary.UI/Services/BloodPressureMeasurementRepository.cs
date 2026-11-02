using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodPressureDiary.UI.Services;

/// <summary>Implements measurement storage with Entity Framework Core.</summary>
public sealed class BloodPressureMeasurementRepository(DiaryDbContext _context) : IBloodPressureMeasurementRepository
{
    /// <inheritdoc />
    public Task<IReadOnlyList<BloodPressureMeasurement>> GetAllAsync(CancellationToken cancellationToken = default) =>
        GetAllAsync(null, null, cancellationToken);

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task AddAsync(BloodPressureMeasurement bloodPressureMeasurement, CancellationToken cancellationToken = default)
    {
        await _context.BloodPressureMeasurements.AddAsync(bloodPressureMeasurement, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(IReadOnlyList<BloodPressureMeasurement> bloodPressureMeasurements, CancellationToken cancellationToken = default)
    {
        await _context.BloodPressureMeasurements.AddRangeAsync(bloodPressureMeasurements, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(BloodPressureMeasurement bloodPressureMeasurement, CancellationToken cancellationToken = default)
    {
        _context.BloodPressureMeasurements.Update(bloodPressureMeasurement);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }

    /// <inheritdoc />
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
