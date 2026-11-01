using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BloodPressureDiary.UI.ViewModels;

/// <summary>Manages the daily measurement reminder settings.</summary>
public sealed class SettingsPageViewModel(
    IReminderService _reminderService,
    IUserInteractionService _interaction,
    INavigationService _navigationService) : ObservableObject
{
    /// <summary>Gets or sets whether the daily reminder is enabled.</summary>
    public bool ReminderEnabled { get; set => SetProperty(ref field, value); } = _reminderService.IsEnabled;
    /// <summary>Gets or sets the daily reminder time.</summary>
    public TimeSpan ReminderTime { get; set => SetProperty(ref field, value); } = _reminderService.ReminderTime;
    /// <summary>Gets the save settings command.</summary>
    public IAsyncRelayCommand SaveCommand => new AsyncRelayCommand(SaveAsync);
    /// <summary>Gets the measurements navigation command.</summary>
    public IAsyncRelayCommand OpenBloodPressureMeasurementsCommand => new AsyncRelayCommand(_navigationService.OpenBloodPressureMeasurementsAsync);

    private async Task SaveAsync()
    {
        if (!await _reminderService.SetReminderAsync(ReminderEnabled, ReminderTime))
        {
            ReminderEnabled = _reminderService.IsEnabled;
            await _interaction.ShowMessageAsync(AppResources.ReminderTitle, AppResources.ReminderPermissionDenied, AppResources.Cancel);
            return;
        }

        await _interaction.ShowMessageAsync(AppResources.ReminderTitle, AppResources.ReminderSaved, AppResources.Cancel);
    }
}
