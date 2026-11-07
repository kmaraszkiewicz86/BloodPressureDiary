using System.Collections.ObjectModel;
using System.Globalization;
using BloodPressureDiary.UI.Database.Entities;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BloodPressureDiary.UI.ViewModels;

/// <summary>Manages measurements, history filters, pagination, and file operations.</summary>
public sealed class MainPageViewModel(
    IBloodPressureMeasurementRepository _repository,
    IUserInteractionService _interaction,
    IFileExportService _exportService,
    IFileSharingService _sharingService,
    INavigationService _navigationService) : ObservableObject
{
    private const int PageSize = 10;
    private IReadOnlyList<BloodPressureMeasurement> _filteredBloodPressureMeasurements = [];
    private int _currentPage = 1;
    private bool _exportReport;

    /// <summary>Gets the current page of measurements in descending date order.</summary>
    public ObservableCollection<BloodPressureMeasurementRowViewModel> BloodPressureMeasurements { get; } = [];
    /// <summary>Gets whether no measurements are currently available.</summary>
    public bool IsEmpty => BloodPressureMeasurements.Count == 0;
    /// <summary>Gets the available history filters.</summary>
    public IReadOnlyList<string> HistoryFilters => [AppResources.AllMeasurements, AppResources.Today, AppResources.LastWeek, AppResources.LastMonth];
    /// <summary>Gets or sets the selected history filter.</summary>
    public int SelectedHistoryFilter
    {
        get;
        set => SetProperty(ref field, value);
    }
    /// <summary>Gets today's date for date selection.</summary>
    public DateTime Today => DateTime.Today;
    /// <summary>Gets or sets whether the history uses a custom date range.</summary>
    public bool UseHistoryDateRange { get; set => SetProperty(ref field, value); }
    /// <summary>Gets or sets the first date in the history range.</summary>
    public DateTime HistoryStartDate { get; set => SetProperty(ref field, value); } = DateTime.Today.AddMonths(-1);
    /// <summary>Gets or sets the last date in the history range.</summary>
    public DateTime HistoryEndDate { get; set => SetProperty(ref field, value); } = DateTime.Today;
    /// <summary>Gets whether the export options are visible.</summary>
    public bool IsExporting { get; private set => SetProperty(ref field, value); }
    /// <summary>Gets the export options title.</summary>
    public string ExportWindowTitle => _exportReport ? AppResources.ExportReport : AppResources.ExportBackup;
    /// <summary>Gets or sets the selected export period.</summary>
    public int SelectedExportFilter { get; set => SetProperty(ref field, value); }
    /// <summary>Gets or sets whether export uses a custom date range.</summary>
    public bool UseExportDateRange { get; set => SetProperty(ref field, value); }
    /// <summary>Gets or sets the first date in the export range.</summary>
    public DateTime ExportStartDate { get; set => SetProperty(ref field, value); } = DateTime.Today.AddMonths(-1);
    /// <summary>Gets or sets the last date in the export range.</summary>
    public DateTime ExportEndDate { get; set => SetProperty(ref field, value); } = DateTime.Today;
    /// <summary>Gets the current page and total page count.</summary>
    public string PageText { get; private set => SetProperty(ref field, value); } = string.Empty;
    /// <summary>Gets whether the previous page is available.</summary>
    public bool HasPreviousPage { get; private set => SetProperty(ref field, value); }
    /// <summary>Gets whether the next page is available.</summary>
    public bool HasNextPage { get; private set => SetProperty(ref field, value); }
    /// <summary>Gets whether the new measurement form is visible.</summary>
    public bool IsAddingBloodPressureMeasurement { get; private set => SetProperty(ref field, value); }
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
    /// <summary>Gets the open new measurement form command.</summary>
    public IRelayCommand AddBloodPressureMeasurementCommand => new RelayCommand(OpenNewBloodPressureMeasurement);
    /// <summary>Gets the cancel new measurement command.</summary>
    public IRelayCommand CancelNewItemCommand => new RelayCommand(CancelNewBloodPressureMeasurement);
    /// <summary>Gets the refresh measurements command.</summary>
    public IAsyncRelayCommand LoadCommand => new AsyncRelayCommand(LoadAsync);
    /// <summary>Gets the history filter change command.</summary>
    public IAsyncRelayCommand<int> FilterHistoryCommand => new AsyncRelayCommand<int>(FilterHistoryAsync);
    /// <summary>Gets the command that applies the history date range.</summary>
    public IAsyncRelayCommand ApplyHistoryDateRangeCommand => new AsyncRelayCommand(ApplyHistoryDateRangeAsync);
    /// <summary>Gets the inline edit command.</summary>
    public IAsyncRelayCommand<BloodPressureMeasurementRowViewModel?> EditCommand => new AsyncRelayCommand<BloodPressureMeasurementRowViewModel?>(EditAsync);
    /// <summary>Gets the inline edit save command.</summary>
    public IAsyncRelayCommand<BloodPressureMeasurementRowViewModel?> SaveEditCommand => new AsyncRelayCommand<BloodPressureMeasurementRowViewModel?>(SaveEditAsync);
    /// <summary>Gets the inline edit cancel command.</summary>
    public IRelayCommand<BloodPressureMeasurementRowViewModel?> CancelEditCommand => new RelayCommand<BloodPressureMeasurementRowViewModel?>(CancelEdit);
    /// <summary>Gets the delete command.</summary>
    public IAsyncRelayCommand<BloodPressureMeasurementRowViewModel?> DeleteCommand => new AsyncRelayCommand<BloodPressureMeasurementRowViewModel?>(DeleteAsync);
    /// <summary>Gets the command that opens CSV backup options.</summary>
    public IRelayCommand ExportBackupCommand => new RelayCommand(() => OpenExport(false));
    /// <summary>Gets the CSV backup import command.</summary>
    public IAsyncRelayCommand ImportBackupCommand => new AsyncRelayCommand(ImportBackupAsync);
    /// <summary>Gets the command that opens HTML report options.</summary>
    public IRelayCommand ExportReportCommand => new RelayCommand(() => OpenExport(true));
    /// <summary>Gets the command that creates and shares the selected export.</summary>
    public IAsyncRelayCommand ConfirmExportCommand => new AsyncRelayCommand(ExportAsync);
    /// <summary>Gets the command that closes export options.</summary>
    public IRelayCommand CancelExportCommand => new RelayCommand(() => IsExporting = false);
    /// <summary>Gets the settings navigation command.</summary>
    public IAsyncRelayCommand OpenSettingsCommand => new AsyncRelayCommand(_navigationService.OpenSettingsAsync);
    /// <summary>Gets the previous history page command.</summary>
    public IRelayCommand PreviousPageCommand => new RelayCommand(() =>
    {
        if (HasPreviousPage)
        {
            _currentPage--;
            UpdatePage();
        }
    });
    /// <summary>Gets the next history page command.</summary>
    public IRelayCommand NextPageCommand => new RelayCommand(() =>
    {
        if (HasNextPage)
        {
            _currentPage++;
            UpdatePage();
        }
    });

    /// <summary>Loads saved measurements into the row collection.</summary>
    private async Task LoadAsync()
    {
        if (!TryGetDateRange(SelectedHistoryFilter, UseHistoryDateRange, HistoryStartDate, HistoryEndDate, out var startDate, out var endDate))
        {
            await _interaction.ShowMessageAsync(AppResources.ValidationTitle, AppResources.InvalidDateRange, AppResources.Cancel);
            return;
        }

        try
        {
            _filteredBloodPressureMeasurements = await _repository.GetAllAsync(startDate, endDate);
            UpdatePage();
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.LoadFailed, AppResources.Cancel);
        }
    }

    /// <summary>Resets pagination and reloads measurements for the selected history filter.</summary>
    private async Task FilterHistoryAsync(int filter)
    {
        if (filter < 0)
        {
            return;
        }

        SelectedHistoryFilter = filter;
        _currentPage = 1;
        await LoadAsync();
    }

    /// <summary>Resets pagination and applies the history date range.</summary>
    private async Task ApplyHistoryDateRangeAsync()
    {
        _currentPage = 1;
        await LoadAsync();
    }

    /// <summary>Opens the form for a new measurement.</summary>
    private void OpenNewBloodPressureMeasurement()
    {
        ResetNewBloodPressureMeasurement();
        IsAddingBloodPressureMeasurement = true;
    }

    /// <summary>Closes the new measurement form and clears its inputs.</summary>
    private void CancelNewBloodPressureMeasurement()
    {
        IsAddingBloodPressureMeasurement = false;
        ResetNewBloodPressureMeasurement();
    }

    /// <summary>Validates and saves a new measurement, then reloads the rows.</summary>
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
            IsAddingBloodPressureMeasurement = false;
            await LoadAsync();
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.SaveFailed, AppResources.Cancel);
        }
    }

    /// <summary>Starts editing the selected measurement row.</summary>
    private static Task EditAsync(BloodPressureMeasurementRowViewModel? row)
    {
        row?.BeginEdit();
        return Task.CompletedTask;
    }

    /// <summary>Validates and saves changes to the selected measurement row.</summary>
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
            await LoadAsync();
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.SaveFailed, AppResources.Cancel);
        }
    }

    /// <summary>Cancels editing the selected measurement row.</summary>
    private static void CancelEdit(BloodPressureMeasurementRowViewModel? row) => row?.CancelEdit();

    /// <summary>Requests confirmation and deletes the selected measurement.</summary>
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
            await LoadAsync();
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.DeleteFailed, AppResources.Cancel);
        }
    }

    /// <summary>Opens the export options for the selected file type.</summary>
    private void OpenExport(bool report)
    {
        _exportReport = report;
        SelectedExportFilter = 0;
        UseExportDateRange = false;
        ExportStartDate = DateTime.Today.AddMonths(-1);
        ExportEndDate = DateTime.Today;
        OnPropertyChanged(nameof(ExportWindowTitle));
        IsExporting = true;
    }

    /// <summary>Imports a selected CSV backup and refreshes the history.</summary>
    private async Task ImportBackupAsync()
    {
        try
        {
            if (await _exportService.ImportBackupAsync())
            {
                _currentPage = 1;
                await LoadAsync();
                await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.ImportSucceeded, AppResources.Cancel);
            }
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.ImportFailed, AppResources.Cancel);
        }
    }

    /// <summary>Creates and shares the selected file for the chosen date range.</summary>
    private async Task ExportAsync()
    {
        if (!TryGetDateRange(SelectedExportFilter, UseExportDateRange, ExportStartDate, ExportEndDate, out var startDate, out var endDate))
        {
            await _interaction.ShowMessageAsync(AppResources.ValidationTitle, AppResources.InvalidDateRange, AppResources.Cancel);
            return;
        }

        try
        {
            var path = _exportReport
                ? await _exportService.CreateReportAsync(startDate, endDate)
                : await _exportService.CreateBackupAsync(startDate, endDate);
            IsExporting = false;
            await _sharingService.ShareAsync(path, _exportReport ? AppResources.ShareReport : AppResources.ShareBackup);
        }
        catch
        {
            await _interaction.ShowMessageAsync(AppResources.AppTitle, AppResources.ExportFailed, AppResources.Cancel);
        }
    }

    /// <summary>Combines the selected period with a valid custom date range.</summary>
    private static bool TryGetDateRange(int filter, bool useCustomRange, DateTime from, DateTime to, out DateTime? startDate, out DateTime? endDate)
    {
        var today = DateTime.Today;
        startDate = filter switch
        {
            1 => today,
            2 => today.AddDays(-6),
            3 => today.AddMonths(-1),
            _ => null
        };
        endDate = startDate.HasValue ? today.AddDays(1) : null;

        if (!useCustomRange)
        {
            return true;
        }

        if (from.Date > today || to.Date > today || from.Date > to.Date)
        {
            return false;
        }

        var customEndDate = to.Date.AddDays(1);
        startDate = !startDate.HasValue || from.Date > startDate.Value ? from.Date : startDate;
        endDate = !endDate.HasValue || customEndDate < endDate.Value ? customEndDate : endDate;
        return true;
    }

    /// <summary>Parses and validates pressure and pulse values.</summary>
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

    /// <summary>Clears the new measurement inputs and resets the date and time.</summary>
    private void ResetNewBloodPressureMeasurement()
    {
        SystolicInput = string.Empty;
        DiastolicInput = string.Empty;
        PulseInput = string.Empty;
        NotesInput = string.Empty;
        MeasuredDate = DateTime.Today;
        MeasuredTime = DateTime.Now.TimeOfDay;
    }

    /// <summary>Displays the current page of filtered measurements.</summary>
    private void UpdatePage()
    {
        var totalPages = Math.Max(1, (_filteredBloodPressureMeasurements.Count + PageSize - 1) / PageSize);
        _currentPage = Math.Clamp(_currentPage, 1, totalPages);
        BloodPressureMeasurements.Clear();
        var startIndex = (_currentPage - 1) * PageSize;
        var endIndex = Math.Min(startIndex + PageSize, _filteredBloodPressureMeasurements.Count);
        for (var index = startIndex; index < endIndex; index++)
        {
            BloodPressureMeasurements.Add(new BloodPressureMeasurementRowViewModel(_filteredBloodPressureMeasurements[index]));
        }
        HasPreviousPage = _currentPage > 1;
        HasNextPage = _currentPage < totalPages;
        PageText = string.Format(CultureInfo.CurrentCulture, AppResources.PageNumber, _currentPage, totalPages);
        OnPropertyChanged(nameof(IsEmpty));
    }
}
