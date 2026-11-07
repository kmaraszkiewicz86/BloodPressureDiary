using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Models;

namespace BloodPressureDiary.UI.Services;

/// <summary>Persists reminder settings and delegates scheduling to the platform.</summary>
public sealed class ReminderService(IReminderScheduler _scheduler, IReminderSettingsStorage _storage) : IReminderService
{
    private const int _legacyReminderId = 7421;

    /// <summary>Gets whether reminders are enabled.</summary>
    public async Task<bool> IsEnabledAsync() => (await _storage.GetAsync()).IsEnabled;

    /// <summary>Gets the configured reminder schedules.</summary>
    public async Task<IReadOnlyList<ReminderSchedule>> GetRemindersAsync() => (await _storage.GetAsync()).Reminders;

    /// <summary>Updates the reminder preference and schedules.</summary>
    public async Task<bool> SetRemindersAsync(bool enabled, IReadOnlyList<ReminderSchedule> reminders, bool requestPermission = true)
    {
        var maximumInterval = (DateTime.MaxValue.Date - DateTime.Today).Days;
        if (reminders.Any(reminder => reminder.IntervalDays <= 0 || reminder.IntervalDays > maximumInterval))
        {
            throw new ArgumentException("Each reminder must have a valid positive interval.", nameof(reminders));
        }

        if (enabled && reminders.Count == 0)
        {
            throw new ArgumentException("At least one reminder is required.", nameof(reminders));
        }

        if (reminders.Any(reminder => reminder.Time < TimeSpan.Zero || reminder.Time >= TimeSpan.FromDays(1))
            || reminders.Select(reminder => reminder.Id).Distinct().Count() != reminders.Count)
        {
            throw new ArgumentException("Reminder times and identifiers must be valid and unique.", nameof(reminders));
        }

        if (enabled && !await _scheduler.CheckPermissionAsync(requestPermission))
        {
            return false;
        }

        var previousSettings = await _storage.GetAsync();
        var updatedReminders = reminders.Select(reminder =>
        {
            var previous = previousSettings.Reminders.FirstOrDefault(item => item.Id == reminder.Id);
            return new ReminderSchedule
            {
                Id = reminder.Id,
                Time = reminder.Time,
                IntervalDays = reminder.IntervalDays,
                StartDate = previousSettings.IsEnabled && previous is not null && previous.Time == reminder.Time && previous.IntervalDays == reminder.IntervalDays
                    ? previous.StartDate.Date
                    : DateTime.Today
            };
        }).OrderBy(reminder => reminder.Time).ToList();

        await _storage.SaveAsync(new ReminderSettings { IsEnabled = enabled, Reminders = updatedReminders });
        foreach (var reminder in previousSettings.Reminders)
        {
            _scheduler.Cancel(reminder.Id);
        }

        if (enabled)
        {
            foreach (var reminder in updatedReminders)
            {
                _scheduler.Schedule(reminder);
            }
        }

        return true;
    }

    /// <summary>Restores previously enabled reminders without prompting for permission.</summary>
    public async Task RestoreAsync()
    {
        var settings = await _storage.GetAsync();
        if (!settings.IsEnabled)
        {
            foreach (var reminder in settings.Reminders)
            {
                _scheduler.Cancel(reminder.Id);
            }

            _scheduler.Cancel(_legacyReminderId);
            return;
        }

        foreach (var reminder in settings.Reminders)
        {
            _scheduler.Schedule(reminder);
        }
    }

    /// <summary>Schedules the next occurrence of a configured reminder when enabled.</summary>
    public async Task<bool> RescheduleAsync(int reminderId)
    {
        var settings = await _storage.GetAsync();
        if (!settings.IsEnabled)
        {
            return false;
        }

        var reminder = settings.Reminders.FirstOrDefault(item => item.Id == reminderId);
        if (reminder is null)
        {
            return false;
        }

        _scheduler.Schedule(reminder);
        return true;
    }
}
