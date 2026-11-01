using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace BloodPressureDiary.UI.ViewModels;
/// <summary>Manages daily reminder preferences.</summary>
public sealed class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService settings;
    private readonly IReminderService reminders;
    private readonly IDialogService dialogs;
    private bool enabled, busy;
    private TimeSpan time;
    /// <summary>Initializes settings and navigation commands.</summary>
    public SettingsViewModel(ISettingsService settings, IReminderService reminders, IDialogService dialogs, INavigationService navigation)
    {
        this.settings = settings; this.reminders = reminders; this.dialogs = dialogs;
        enabled = settings.Enabled; time = settings.Time;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        MeasurementsCommand = new AsyncRelayCommand(navigation.ShowMeasurementsAsync);
    }
    /// <summary>Gets or sets whether reminders are enabled.</summary>
    public bool Enabled { get => enabled; set => SetProperty(ref enabled, value); }
    /// <summary>Gets or sets the local reminder time.</summary>
    public TimeSpan Time { get => time; set => SetProperty(ref time, value); }
    /// <summary>Gets whether settings can be changed.</summary>
    public bool IsAvailable => !busy;
    /// <summary>Gets the preference save command.</summary>
    public IAsyncRelayCommand SaveCommand { get; }
    /// <summary>Gets the measurements navigation command.</summary>
    public IAsyncRelayCommand MeasurementsCommand { get; }
    private async Task SaveAsync()
    {
        busy = true; OnPropertyChanged(nameof(IsAvailable));
        try
        {
            if (!await reminders.ConfigureAsync(Enabled, Time))
            { await dialogs.ShowErrorAsync(AppResources.NotificationDenied); return; }
            settings.Save(Enabled, Time);
        }
        catch (Exception) { await dialogs.ShowErrorAsync(AppResources.ReminderError); }
        finally { busy = false; OnPropertyChanged(nameof(IsAvailable)); }
    }
}
