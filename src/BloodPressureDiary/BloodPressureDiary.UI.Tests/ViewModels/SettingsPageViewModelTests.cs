using BloodPressureDiary.UI.Resources.Localization;
using BloodPressureDiary.UI.Models;
using BloodPressureDiary.UI.Tests.Fixtures;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.ViewModels;

/// <summary>Tests reminder settings save behavior.</summary>
public sealed class SettingsPageViewModelTests(SettingsPageViewModelFixture fixture) : IClassFixture<SettingsPageViewModelFixture>
{
    /// <summary>Restores the saved disabled state and reports when reminder scheduling fails.</summary>
    [Fact]
    public async Task SaveCommand_WhenSchedulingFails_RestoresDisabledStateAndShowsPermissionMessage()
    {
        // Arrange
        fixture.Reset(reminderEnabled: false, reminderTime: new TimeSpan(7, 30, 0));
        await fixture.Sut.LoadCommand.ExecuteAsync(null);
        fixture.Sut.ReminderEnabled = true;
        fixture.ReminderService.SetRemindersAsync(true, Arg.Any<IReadOnlyList<ReminderSchedule>>(), true).Returns(false);

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        Assert.False(fixture.Sut.ReminderEnabled);
        await fixture.ReminderService.Received(1).SetRemindersAsync(true, Arg.Is<IReadOnlyList<ReminderSchedule>>(items => items.Count == 1 && items[0].Time == new TimeSpan(7, 30, 0)), true);
        await fixture.Interaction.Received(1).ShowMessageAsync(
            AppResources.ReminderTitle,
            AppResources.ReminderPermissionDenied,
            AppResources.Cancel);
    }

    /// <summary>Reports success after reminder settings are saved.</summary>
    [Fact]
    public async Task SaveCommand_WhenReminderIsSaved_ShowsSuccessMessage()
    {
        // Arrange
        fixture.Reset(reminderEnabled: false, reminderTime: new TimeSpan(8, 15, 0));
        await fixture.Sut.LoadCommand.ExecuteAsync(null);
        fixture.Sut.ReminderEnabled = true;
        fixture.ReminderService.SetRemindersAsync(true, Arg.Any<IReadOnlyList<ReminderSchedule>>(), true).Returns(true);

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        await fixture.ReminderService.Received(1).SetRemindersAsync(true, Arg.Is<IReadOnlyList<ReminderSchedule>>(items => items.Count == 1 && items[0].Time == new TimeSpan(8, 15, 0)), true);
        await fixture.Interaction.Received(1).ShowMessageAsync(
            AppResources.ReminderTitle,
            AppResources.ReminderSaved,
            AppResources.Cancel);
    }

    /// <summary>Adds and removes reminder times.</summary>
    [Fact]
    public async Task ReminderCommands_AddAndRemoveTimes()
    {
        // Arrange
        fixture.Reset(reminderEnabled: false, reminderTime: new TimeSpan(8, 0, 0));
        await fixture.Sut.LoadCommand.ExecuteAsync(null);
        fixture.Sut.ReminderTime = new TimeSpan(10, 0, 0);
        await fixture.Sut.AddReminderCommand.ExecuteAsync(null);
        fixture.Sut.ReminderTime = new TimeSpan(15, 0, 0);
        await fixture.Sut.AddReminderCommand.ExecuteAsync(null);
        var removed = fixture.Sut.Reminders[1];

        // Act
        fixture.Sut.RemoveReminderCommand.Execute(removed);

        // Assert
        Assert.Equal(2, fixture.Sut.Reminders.Count);
        Assert.DoesNotContain(removed, fixture.Sut.Reminders);
        Assert.Contains(fixture.Sut.Reminders, reminder => reminder.Time == new TimeSpan(15, 0, 0));
    }

    /// <summary>Rejects adding the same reminder time twice.</summary>
    [Fact]
    public async Task AddReminderCommand_Duplicate_ShowsValidation()
    {
        // Arrange
        fixture.Reset(reminderEnabled: false, reminderTime: new TimeSpan(8, 0, 0));
        await fixture.Sut.LoadCommand.ExecuteAsync(null);

        // Act
        await fixture.Sut.AddReminderCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(fixture.Sut.Reminders);
        await fixture.Interaction.Received(1).ShowMessageAsync(AppResources.ReminderTitle, AppResources.DuplicateReminderTime, AppResources.Cancel);
    }

    /// <summary>Rejects an invalid interval without adding a reminder.</summary>
    [Theory]
    [InlineData("0")]
    [InlineData("abc")]
    public async Task AddReminderCommand_InvalidInterval_ShowsValidation(string interval)
    {
        // Arrange
        fixture.Reset(reminderEnabled: false, reminderTime: new TimeSpan(8, 0, 0));
        await fixture.Sut.LoadCommand.ExecuteAsync(null);
        fixture.Sut.IntervalDaysInput = interval;
        fixture.Sut.ReminderTime = new TimeSpan(10, 0, 0);

        // Act
        await fixture.Sut.AddReminderCommand.ExecuteAsync(null);

        // Assert
        await fixture.Interaction.Received(1).ShowMessageAsync(AppResources.ReminderTitle, AppResources.InvalidReminderInterval, AppResources.Cancel);
        Assert.Single(fixture.Sut.Reminders);
    }

    /// <summary>Requires a reminder time when enabling reminders.</summary>
    [Fact]
    public async Task SaveCommand_EnabledWithoutTimes_ShowsValidation()
    {
        // Arrange
        fixture.Reset(reminderEnabled: false, reminderTime: new TimeSpan(8, 0, 0));
        await fixture.Sut.LoadCommand.ExecuteAsync(null);
        fixture.Sut.Reminders.Clear();
        fixture.Sut.ReminderEnabled = true;

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        await fixture.Interaction.Received(1).ShowMessageAsync(AppResources.ReminderTitle, AppResources.ReminderTimeRequired, AppResources.Cancel);
        await fixture.ReminderService.DidNotReceive().SetRemindersAsync(Arg.Any<bool>(), Arg.Any<IReadOnlyList<ReminderSchedule>>(), Arg.Any<bool>());
    }

    /// <summary>Adds reminder times with independently selected intervals.</summary>
    [Fact]
    public async Task AddReminderCommand_UsesIndividualIntervals()
    {
        // Arrange
        fixture.Reset(reminderEnabled: false, reminderTime: new TimeSpan(8, 0, 0));
        await fixture.Sut.LoadCommand.ExecuteAsync(null);
        fixture.Sut.ReminderTime = new TimeSpan(10, 0, 0);
        fixture.Sut.IntervalDaysInput = "2";
        await fixture.Sut.AddReminderCommand.ExecuteAsync(null);
        fixture.Sut.ReminderTime = new TimeSpan(15, 0, 0);
        fixture.Sut.IntervalDaysInput = "3";

        // Act
        await fixture.Sut.AddReminderCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(1, fixture.Sut.Reminders[0].IntervalDays);
        Assert.Equal(2, fixture.Sut.Reminders[1].IntervalDays);
        Assert.Equal(3, fixture.Sut.Reminders[2].IntervalDays);
    }
    /// <summary>Loads the saved enabled state and reminder times.</summary>
    [Fact]
    public async Task LoadCommand_LoadsSavedSettings()
    {
        // Arrange
        fixture.Reset(reminderEnabled: true, reminderTime: new TimeSpan(9, 0, 0));

        // Act
        await fixture.Sut.LoadCommand.ExecuteAsync(null);

        // Assert
        Assert.True(fixture.Sut.IsLoaded);
        Assert.True(fixture.Sut.ReminderEnabled);
        Assert.Equal(new TimeSpan(9, 0, 0), Assert.Single(fixture.Sut.Reminders).Time);
    }
}
