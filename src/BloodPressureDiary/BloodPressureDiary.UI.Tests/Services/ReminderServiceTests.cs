using System.Text.Json;
using BloodPressureDiary.UI.Models;
using BloodPressureDiary.UI.Tests.Fixtures;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.Services;

/// <summary>Tests reminder persistence and scheduling.</summary>
public sealed class ReminderServiceTests(ReminderServiceFixture fixture) : IClassFixture<ReminderServiceFixture>
{
    /// <summary>Restores and converts an existing single reminder.</summary>
    [Fact]
    public async Task RestoreAsync_ConvertsEnabledLegacyReminder()
    {
        // Arrange
        fixture.Initialize();
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("daily-reminder-time", "08:00:00");

        // Act
        await fixture.Sut.RestoreAsync();

        // Assert
        fixture.Scheduler.Received(1).Schedule(Arg.Is<ReminderSchedule>(reminder =>
            reminder.Id == 7421 && reminder.Time == new TimeSpan(8, 0, 0) && reminder.IntervalDays == 1));
        Assert.Single((await fixture.Sut.GetRemindersAsync()));
        Assert.Single((await fixture.Storage.GetAsync()).Reminders);
    }

    /// <summary>Stores and schedules all selected times with their individual intervals.</summary>
    [Fact]
    public async Task SetRemindersAsync_StoresAndSchedulesAllTimes()
    {
        // Arrange
        fixture.Initialize();
        var reminders = new[] { fixture.CreateReminder(1, 8), fixture.CreateReminder(2, 10), fixture.CreateReminder(3, 15) };
        reminders[1].IntervalDays = 2;
        reminders[2].IntervalDays = 3;

        // Act
        var saved = await fixture.Sut.SetRemindersAsync(true, reminders);

        // Assert
        Assert.True(saved);
        Assert.True((await fixture.Sut.IsEnabledAsync()));
        Assert.Equal(3, (await fixture.Sut.GetRemindersAsync()).Count);
        Assert.Equal(new[] { 1, 2, 3 }, (await fixture.Sut.GetRemindersAsync()).Select(reminder => reminder.IntervalDays));
        fixture.Scheduler.Received(1).Schedule(Arg.Is<ReminderSchedule>(reminder => reminder.Id == 1 && reminder.IntervalDays == 1));
        fixture.Scheduler.Received(1).Schedule(Arg.Is<ReminderSchedule>(reminder => reminder.Id == 2 && reminder.IntervalDays == 2));
        fixture.Scheduler.Received(1).Schedule(Arg.Is<ReminderSchedule>(reminder => reminder.Id == 3 && reminder.IntervalDays == 3));
    }

    /// <summary>Keeps the existing settings when notification permission is denied.</summary>
    [Fact]
    public async Task SetRemindersAsync_PermissionDenied_PreservesExistingSettings()
    {
        // Arrange
        fixture.Initialize();
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("daily-reminder-time", "08:00:00");
        fixture.Scheduler.CheckPermissionAsync(true).Returns(false);

        // Act
        var saved = await fixture.Sut.SetRemindersAsync(true, new[] { fixture.CreateReminder(2, 10) });

        // Assert
        Assert.False(saved);
        Assert.True((await fixture.Sut.IsEnabledAsync()));
        Assert.Equal(new TimeSpan(8, 0, 0), Assert.Single((await fixture.Sut.GetRemindersAsync())).Time);
        fixture.Scheduler.DidNotReceive().Cancel(Arg.Any<int>());
        fixture.Scheduler.DidNotReceive().Schedule(Arg.Any<ReminderSchedule>());
    }

    /// <summary>Preserves an unchanged reminder's start date while starting a new reminder today.</summary>
    [Fact]
    public async Task SetRemindersAsync_UnchangedInterval_PreservesStartDate()
    {
        // Arrange
        fixture.Initialize();
        var startDate = DateTime.Today.AddDays(-5);
        var previous = fixture.CreateReminder(1, 8);
        previous.IntervalDays = 2;
        previous.StartDate = startDate;
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("reminder-schedules", JsonSerializer.Serialize(new[] { previous }));

        // Act
        await fixture.Sut.SetRemindersAsync(true, new[] { previous, fixture.CreateReminder(2, 10) });

        // Assert
        Assert.Equal(startDate, (await fixture.Sut.GetRemindersAsync())[0].StartDate);
        Assert.Equal(DateTime.Today, (await fixture.Sut.GetRemindersAsync())[1].StartDate);
    }

    /// <summary>Changing one interval does not reset another reminder's start date.</summary>
    [Fact]
    public async Task SetRemindersAsync_ChangedInterval_PreservesOtherReminderCycle()
    {
        // Arrange
        fixture.Initialize();
        var startDate = DateTime.Today.AddDays(-5);
        var first = fixture.CreateReminder(1, 8);
        var second = fixture.CreateReminder(2, 10);
        first.IntervalDays = 2;
        first.StartDate = startDate;
        second.IntervalDays = 3;
        second.StartDate = startDate;
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("reminder-schedules", JsonSerializer.Serialize(new[] { first, second }));
        first.IntervalDays = 4;

        // Act
        await fixture.Sut.SetRemindersAsync(true, new[] { first, second });

        // Assert
        Assert.Equal(DateTime.Today, (await fixture.Sut.GetRemindersAsync())[0].StartDate);
        Assert.Equal(startDate, (await fixture.Sut.GetRemindersAsync())[1].StartDate);
        Assert.Equal(3, (await fixture.Sut.GetRemindersAsync())[1].IntervalDays);
    }

    /// <summary>Restores all schedules without changing their start date.</summary>
    [Fact]
    public async Task RestoreAsync_RestoresAllSchedulesWithOriginalStartDate()
    {
        // Arrange
        fixture.Initialize();
        var startDate = DateTime.Today.AddDays(-5);
        var first = fixture.CreateReminder(1, 8);
        var second = fixture.CreateReminder(2, 15);
        first.IntervalDays = 2;
        first.StartDate = startDate;
        second.IntervalDays = 3;
        second.StartDate = startDate;
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("reminder-schedules", JsonSerializer.Serialize(new[] { first, second }));

        // Act
        await fixture.Sut.RestoreAsync();

        // Assert
        fixture.Scheduler.Received(1).Schedule(Arg.Is<ReminderSchedule>(reminder => reminder.Id == 1 && reminder.IntervalDays == 2 && reminder.StartDate == startDate));
        fixture.Scheduler.Received(1).Schedule(Arg.Is<ReminderSchedule>(reminder => reminder.Id == 2 && reminder.IntervalDays == 3 && reminder.StartDate == startDate));
    }

    /// <summary>Cancels removed reminder times while saving the remaining schedule.</summary>
    [Fact]
    public async Task SetRemindersAsync_RemovedTime_CancelsPreviousAlarm()
    {
        // Arrange
        fixture.Initialize();
        var first = fixture.CreateReminder(1, 8);
        var removed = fixture.CreateReminder(2, 10);
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("reminder-schedules", JsonSerializer.Serialize(new[] { first, removed }));

        // Act
        await fixture.Sut.SetRemindersAsync(true, new[] { first });

        // Assert
        fixture.Scheduler.Received(1).Cancel(2);
        Assert.Single((await fixture.Sut.GetRemindersAsync()));
        fixture.Scheduler.DidNotReceive().Schedule(Arg.Is<ReminderSchedule>(reminder => reminder.Id == 2));
    }

    /// <summary>Cancels saved alarms when reminders are disabled.</summary>
    [Fact]
    public async Task SetRemindersAsync_Disabled_CancelsSavedAlarms()
    {
        // Arrange
        fixture.Initialize();
        var reminders = new[] { fixture.CreateReminder(1, 8), fixture.CreateReminder(2, 10) };
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("reminder-schedules", JsonSerializer.Serialize(reminders));

        // Act
        await fixture.Sut.SetRemindersAsync(false, reminders);

        // Assert
        Assert.False((await fixture.Sut.IsEnabledAsync()));
        fixture.Scheduler.Received(1).Cancel(1);
        fixture.Scheduler.Received(1).Cancel(2);
        fixture.Scheduler.DidNotReceive().Schedule(Arg.Any<ReminderSchedule>());
    }

    /// <summary>Reschedules only the matching enabled reminder.</summary>
    [Fact]
    public async Task RescheduleAsync_EnabledReminder_SchedulesMatchingTime()
    {
        // Arrange
        fixture.Initialize();
        var reminders = new[] { fixture.CreateReminder(1, 8), fixture.CreateReminder(2, 10) };
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("reminder-schedules", JsonSerializer.Serialize(reminders));

        // Act
        await fixture.Sut.RescheduleAsync(2);

        // Assert
        fixture.Scheduler.Received(1).Schedule(Arg.Is<ReminderSchedule>(reminder => reminder.Id == 2));
    }
    /// <summary>Reads the migrated file after the legacy preferences change.</summary>
    [Fact]
    public async Task GetRemindersAsync_MigratedSettings_AreReadFromFile()
    {
        // Arrange
        fixture.Initialize();
        fixture.Preferences.Set("daily-reminder-enabled", true);
        fixture.Preferences.Set("daily-reminder-time", "08:00:00");
        await fixture.Sut.GetRemindersAsync();
        fixture.Preferences.Set("daily-reminder-time", "10:00:00");

        // Act
        var reminders = await fixture.Sut.GetRemindersAsync();

        // Assert
        Assert.Equal(new TimeSpan(8, 0, 0), Assert.Single(reminders).Time);
    }

    /// <summary>Does not reschedule when reminders are disabled.</summary>
    [Fact]
    public async Task RescheduleAsync_Disabled_DoesNotSchedule()
    {
        // Arrange
        fixture.Initialize();
        fixture.Preferences.Set("daily-reminder-time", "08:00:00");

        // Act
        var scheduled = await fixture.Sut.RescheduleAsync(7421);

        // Assert
        Assert.False(scheduled);
        fixture.Scheduler.DidNotReceive().Schedule(Arg.Any<ReminderSchedule>());
    }
    /// <summary>Waits for the saved settings before rescheduling the reminder.</summary>
    [Fact]
    public async Task RescheduleAsync_WaitsForSettingsBeforeScheduling()
    {
        // Arrange
        fixture.Initialize();
        var reminder = fixture.CreateReminder(1, 8);

        // Act
        var operation = fixture.PendingSut.RescheduleAsync(1);
        fixture.Scheduler.DidNotReceive().Schedule(Arg.Any<ReminderSchedule>());
        fixture.PendingRead.SetResult(new ReminderSettings { IsEnabled = true, Reminders = [reminder] });
        var scheduled = await operation;

        // Assert
        Assert.True(scheduled);
        fixture.Scheduler.Received(1).Schedule(reminder);
    }
}
