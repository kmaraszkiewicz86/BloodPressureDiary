using System.Collections.ObjectModel;
using System.Globalization;
using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BloodPressureDiary.UI.ViewModels;

/// <summary>Manages measurement entry, editing, deletion, and file exports.</summary>
public sealed class MainPageViewModel(
    IBloodPressureMeasurementRepository _repository,
    IUserInteractionService _interaction,
    IFileExportService _exportService,
    IFileSharingService _sharingService,
    INavigationService _navigationService) : ObservableObject
{
    /// <summary>Gets saved measurements in descending date order.</summary>
    public ObservableCollection<BloodPressureMeasurementRowViewModel> BloodPressureMeasurements { get; } = [];
    /// <summary>Gets whether no measurements are currently available.</summary>
    public bool IsEmpty => BloodPressureMeasurements.Count == 0;
    /// <summary>Gets or sets the new systolic value.</summary>
    public string SystolicInput { get; set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets or sets the new diastolic value.</summary>
    public string DiastolicInput { get; set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets or sets the optional new pulse value.</summary>
    public string PulseInput { get; set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets or sets the optional new note.</summary>
    public string NotesInput { get; set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets or sets the new measurement date.</summary>
    public DateTime MeasuredDate { get; set => SetProperty(ref field, value); } = DateTime.Today;
    /// <summary>Gets or sets the new measurement time.</summary>
    public TimeSpan MeasuredTime { get; set => SetProperty(ref field, value); } = DateTime.Now.TimeOfDay;
    /// <summary>Gets the save measurement command.</summary>
    public IAsyncRelayCommand SaveCommand => new AsyncRelayCommand(SaveAsync);
    /// <summary>Gets the refresh measurements command.</summary>
    public IAsyncRelayCommand LoadCommand => new AsyncRelayCommand(LoadAsync);
    /// <summary>Gets the inline edit command.</summary>
    public IAsyncRelayCommand<BloodPressureMeasurementRowViewModel?> EditCommand => new AsyncRelayCommand<BloodPressureMeasurementRowViewModel?>(EditAsync);
    /// <summary>Gets the inline edit save command.</summary>
    public IAsyncRelayCommand<BloodPressureMeasurementRowViewModel?> SaveEditCommand => new AsyncRelayCommand<BloodPressureMeasurementRowViewModel?>(SaveEditAsync);
    /// <summary>Gets the inline edit cancel command.</summary>
    public IRelayCommand<BloodPressureMeasurementRowViewModel?> CancelEditCommand => new RelayCommand<BloodPressureMeasurementRowViewModel?>(CancelEdit);
    /// <summary>Gets the delete command.</summary>
    public IAsyncRelayCommand<BloodPressureMeasurementRowViewModel?> DeleteCommand => new AsyncRelayCommand<BloodPressureMeasurementRowViewModel?>(DeleteAsync);
    /// <summary>Gets the CSV backup command.</summary>
    public IAsyncRelayCommand ExportBackupCommand => new AsyncRelayCommand(ExportBackupAsync);
    /// <summary>Gets the HTML report command.</summary>
    public IAsyncRelayCommand ExportReportCommand => new AsyncRelayCommand(ExportReportAsync);
    /// <summary>Gets the settings navigation command.</summary>
    public IAsyncRelayCommand OpenSettingsCommand => new AsyncRelayCommand(_navigationService.OpenSettingsAsync);

    private async Task LoadAsync()
    {
        try
        {
            var records = await _repository.GetAllAsync();
            BloodPressureMeasurements.Clear();
            foreach (var record in records)
            {
                BloodPressureMeasurements.Add(new BloodPressureMeasurementRowViewModel(record));
            }
            OnPropertyChanged(nameof(IsEmpty));
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.LoadFailed, AppResources.Cancel);
        }
    }

    private async Task SaveAsync()
    {
        if (!TryReadValues(SystolicInput, DiastolicInput, PulseInput, out var systolic, out var diastolic, out var pulse, out var error))
        {
            await _interaction.ShowMessageAsync(AppResources.ValidationTitle, error, AppResources.Cancel);
            return;
        }

        try
        {
            await _repository.AddAsync(new BloodPressureMeasurement
            {
                Systolic = systolic,
                Diastolic = diastolic,
                Pulse = pulse,
                Notes = string.IsNullOrWhiteSpace(NotesInput) ? null : NotesInput.Trim(),
                MeasuredAt = MeasuredDate.Date.Add(MeasuredTime)
            });
            ResetNewBloodPressureMeasurement();
            await LoadAsync();
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.SaveFailed, AppResources.Cancel);
        }
    }

    private static Task EditAsync(BloodPressureMeasurementRowViewModel? row)
    {
        row?.BeginEdit();
        return Task.CompletedTask;
    }

    private async Task SaveEditAsync(BloodPressureMeasurementRowViewModel? row)
    {
        if (row is null)
        {
            return;
        }

        if (!TryReadValues(row.SystolicInput, row.DiastolicInput, row.PulseInput, out var systolic, out var diastolic, out var pulse, out var error))
        {
            await _interaction.ShowMessageAsync(AppResources.ValidationTitle, error, AppResources.Cancel);
            return;
        }

        try
        {
            await _repository.UpdateAsync(new BloodPressureMeasurement
            {
                Id = row.BloodPressureMeasurement.Id,
                Systolic = systolic,
                Diastolic = diastolic,
                Pulse = pulse,
                Notes = string.IsNullOrWhiteSpace(row.NotesInput) ? null : row.NotesInput.Trim(),
                MeasuredAt = row.MeasuredDate.Date.Add(row.MeasuredTime)
            });
            row.ApplyEdit(systolic, diastolic, pulse);
            SortBloodPressureMeasurements();
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.SaveFailed, AppResources.Cancel);
        }
    }

    private static void CancelEdit(BloodPressureMeasurementRowViewModel? row) => row?.CancelEdit();

    private async Task DeleteAsync(BloodPressureMeasurementRowViewModel? row)
    {
        if (row is null || !await _interaction.ConfirmAsync(
                AppResources.DeleteTitle, AppResources.DeleteConfirmation, AppResources.Delete, AppResources.Cancel))
        {
            return;
        }

        try
        {
            await _repository.DeleteAsync(row.BloodPressureMeasurement.Id);
            BloodPressureMeasurements.Remove(row);
            OnPropertyChanged(nameof(IsEmpty));
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.DeleteFailed, AppResources.Cancel);
        }
    }

    private async Task ExportBackupAsync()
    {
        try
        {
            var path = await _exportService.CreateBackupAsync();
            await _sharingService.ShareAsync(path, AppResources.ShareBackup);
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.ExportFailed, AppResources.Cancel);
        }
    }

    private async Task ExportReportAsync()
    {
        try
        {
            var path = await _exportService.CreateReportAsync();
            await _sharingService.ShareAsync(path, AppResources.ShareReport);
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.ExportFailed, AppResources.Cancel);
        }
    }

    private static bool TryReadValues(string systolicText, string diastolicText, string pulseText,
        out int systolic, out int diastolic, out int? pulse, out string error)
    {
        systolic = 0;
        diastolic = 0;
        pulse = null;
        if (!int.TryParse(systolicText, NumberStyles.Integer, CultureInfo.CurrentCulture, out systolic) || systolic <= 0
            || !int.TryParse(diastolicText, NumberStyles.Integer, CultureInfo.CurrentCulture, out diastolic) || diastolic <= 0)
        {
            error = AppResources.RequiredPressure;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(pulseText))
        {
            if (!int.TryParse(pulseText, NumberStyles.Integer, CultureInfo.CurrentCulture, out var parsedPulse) || parsedPulse <= 0)
            {
                error = AppResources.InvalidPulse;
                return false;
            }

            pulse = parsedPulse;
        }

        error = string.Empty;
        return true;
    }

    private void ResetNewBloodPressureMeasurement()
    {
        SystolicInput = string.Empty;
        DiastolicInput = string.Empty;
        PulseInput = string.Empty;
        NotesInput = string.Empty;
        MeasuredDate = DateTime.Today;
        MeasuredTime = DateTime.Now.TimeOfDay;
    }

    private void SortBloodPressureMeasurements()
    {
        var sorted = BloodPressureMeasurements.OrderByDescending(row => row.BloodPressureMeasurement.MeasuredAt)
            .ThenByDescending(row => row.BloodPressureMeasurement.Id).ToArray();
        BloodPressureMeasurements.Clear();
        foreach (var row in sorted)
        {
            BloodPressureMeasurements.Add(row);
        }
    }
}
