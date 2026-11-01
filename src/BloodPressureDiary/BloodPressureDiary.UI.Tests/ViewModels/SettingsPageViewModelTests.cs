using BloodPressureDiary.UI.Resources.Localization;
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
        fixture.Sut.ReminderEnabled = true;
        fixture.ReminderService.SetReminderAsync(true, fixture.Sut.ReminderTime, true).Returns(Task.FromResult(false));

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        Assert.False(fixture.Sut.ReminderEnabled);
        await fixture.ReminderService.Received(1).SetReminderAsync(true, new TimeSpan(7, 30, 0), true);
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
        fixture.Sut.ReminderEnabled = true;
        fixture.ReminderService.SetReminderAsync(true, fixture.Sut.ReminderTime, true).Returns(Task.FromResult(true));

        // Act
        await fixture.Sut.SaveCommand.ExecuteAsync(null);

        // Assert
        await fixture.ReminderService.Received(1).SetReminderAsync(true, new TimeSpan(8, 15, 0), true);
        await fixture.Interaction.Received(1).ShowMessageAsync(
            AppResources.ReminderTitle,
            AppResources.ReminderSaved,
            AppResources.Cancel);
    }
}
