using System.Collections.ObjectModel;
using System.Globalization;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Models;
using BloodPressureDiary.UI.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BloodPressureDiary.UI.ViewModels;

/// <summary>Manages blood pressure measurement reminder settings.</summary>
public sealed class SettingsPageViewModel(
    IReminderService _reminderService,
    IUserInteractionService _interaction,
    INavigationService _navigationService) : ObservableObject
{
    private const int _reminderIdOffset = 10000;

    /// <summary>Gets or sets whether reminders are enabled.</summary>
    public bool ReminderEnabled { get; set => SetProperty(ref field, value); }
    /// <summary>Gets or sets the time to add to the reminder schedule.</summary>
    public TimeSpan ReminderTime { get; set => SetProperty(ref field, value); } = new(DateTime.Now.Hour, DateTime.Now.Minute, 0);
    /// <summary>Gets or sets the interval in days for the reminder being added.</summary>
    public string IntervalDaysInput { get; set => SetProperty(ref field, value); } = "1";
    /// <summary>Gets the reminder times being edited.</summary>
    public ObservableCollection<ReminderSchedule> Reminders { get; } = new();
    /// <summary>Gets whether the saved settings have been loaded.</summary>
    public bool IsLoaded { get; set => SetProperty(ref field, value); }
    /// <summary>Gets the command to load saved settings.</summary>
    public IAsyncRelayCommand LoadCommand => new AsyncRelayCommand(LoadAsync);
    /// <summary>Gets the command to add a reminder time.</summary>
    public IAsyncRelayCommand AddReminderCommand => new AsyncRelayCommand(AddReminderAsync);
    /// <summary>Gets the command to remove a reminder time.</summary>
    public IRelayCommand<ReminderSchedule?> RemoveReminderCommand => new RelayCommand<ReminderSchedule?>(RemoveReminder);
    /// <summary>Gets the save settings command.</summary>
    public IAsyncRelayCommand SaveCommand => new AsyncRelayCommand(SaveAsync);
    /// <summary>Gets the measurements navigation command.</summary>
    public IAsyncRelayCommand OpenBloodPressureMeasurementsCommand => new AsyncRelayCommand(_navigationService.OpenBloodPressureMeasurementsAsync);

    /// <summary>Loads the saved reminder settings.</summary>
    private async Task LoadAsync()
    {
        IsLoaded = false;
        var enabled = await _reminderService.IsEnabledAsync();
        var reminders = await _reminderService.GetRemindersAsync();
        ReminderEnabled = enabled;
        Reminders.Clear();
        foreach (var reminder in reminders)
        {
            Reminders.Add(reminder);
        }

        IsLoaded = true;
    }

    /// <summary>Adds the selected reminder time if it is not already present.</summary>
    private async Task AddReminderAsync()
    {
        if (!int.TryParse(IntervalDaysInput, NumberStyles.Integer, CultureInfo.CurrentCulture, out var intervalDays)
            || intervalDays <= 0 || intervalDays > (DateTime.MaxValue.Date - DateTime.Today).Days)
        {
            await _interaction.ShowMessageAsync(AppResources.ReminderTitle, AppResources.InvalidReminderInterval, AppResources.Cancel);
            return;
        }

        var time = TimeSpan.FromMinutes((int)ReminderTime.TotalMinutes);
        foreach (var reminder in Reminders)
        {
            if (reminder.Time == time)
            {
                await _interaction.ShowMessageAsync(AppResources.ReminderTitle, AppResources.DuplicateReminderTime, AppResources.Cancel);
                return;
            }
        }

        Reminders.Add(new ReminderSchedule { Id = _reminderIdOffset + (int)time.TotalMinutes, Time = time, IntervalDays = intervalDays });
    }

    /// <summary>Removes the selected reminder time.</summary>
    private void RemoveReminder(ReminderSchedule? reminder)
    {
        if (reminder is not null)
        {
            Reminders.Remove(reminder);
        }
    }

    /// <summary>Validates and saves the reminder settings and displays the result.</summary>
    private async Task SaveAsync()
    {
        if (ReminderEnabled && Reminders.Count == 0)
        {
            await _interaction.ShowMessageAsync(AppResources.ReminderTitle, AppResources.ReminderTimeRequired, AppResources.Cancel);
            return;
        }

        if (!await _reminderService.SetRemindersAsync(ReminderEnabled, Reminders))
        {
            ReminderEnabled = await _reminderService.IsEnabledAsync();
            await _interaction.ShowMessageAsync(AppResources.ReminderTitle, AppResources.ReminderPermissionDenied, AppResources.Cancel);
            return;
        }

        await _interaction.ShowMessageAsync(AppResources.ReminderTitle, AppResources.ReminderSaved, AppResources.Cancel);
    }
}
