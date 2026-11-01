using System.Collections.ObjectModel;
using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace BloodPressureDiary.UI.ViewModels;
/// <summary>Manages measurement entry and inline editing.</summary>
public sealed class MainViewModel : ObservableObject
{
    private readonly IBloodPressureMeasurementService measurements;
    private readonly IDialogService dialogs;
    private BloodPressureMeasurement? original;
    private BloodPressureMeasurementRowViewModel? editingRow;
    private bool busy;
    /// <summary>Initializes diary commands and dependencies.</summary>
    public MainViewModel(IBloodPressureMeasurementService measurements, IDialogService dialogs, INavigationService navigation)
    {
        this.measurements = measurements;
        this.dialogs = dialogs;
        LoadCommand = new AsyncRelayCommand(() => RunAsync(RefreshAsync));
        SaveCommand = new AsyncRelayCommand(() => RunAsync(SaveNewAsync));
        EditCommand = new RelayCommand<BloodPressureMeasurementRowViewModel>(Edit);
        CancelCommand = new RelayCommand<BloodPressureMeasurementRowViewModel>(Cancel);
        SaveRowCommand = new AsyncRelayCommand<BloodPressureMeasurementRowViewModel>(row => RunAsync(() => SaveRowAsync(row)));
        DeleteCommand = new AsyncRelayCommand<BloodPressureMeasurementRowViewModel>(row => RunAsync(() => DeleteAsync(row)));
        SettingsCommand = new AsyncRelayCommand(() => RunAsync(navigation.ShowSettingsAsync));
    }
    /// <summary>Gets the new measurement form.</summary>
    public BloodPressureMeasurementRowViewModel NewMeasurement { get; } = new() { IsEditing = true };
    /// <summary>Gets measurements in descending date order.</summary>
    public ObservableCollection<BloodPressureMeasurementRowViewModel> Rows { get; } = new();
    /// <summary>Gets whether a storage operation is running.</summary>
    public bool IsBusy
    {
        get => busy;
        private set
        {
            if (SetProperty(ref busy, value))
            {
                OnPropertyChanged(nameof(IsAvailable));
            }
        }
    }
    /// <summary>Gets whether controls may be used.</summary>
    public bool IsAvailable => !IsBusy;
    /// <summary>Gets the load command.</summary>
    public IAsyncRelayCommand LoadCommand { get; }
    /// <summary>Gets the insertion command.</summary>
    public IAsyncRelayCommand SaveCommand { get; }
    /// <summary>Gets the edit command.</summary>
    public IRelayCommand<BloodPressureMeasurementRowViewModel> EditCommand { get; }
    /// <summary>Gets the cancel command.</summary>
    public IRelayCommand<BloodPressureMeasurementRowViewModel> CancelCommand { get; }
    /// <summary>Gets the inline save command.</summary>
    public IAsyncRelayCommand<BloodPressureMeasurementRowViewModel> SaveRowCommand { get; }
    /// <summary>Gets the delete command.</summary>
    public IAsyncRelayCommand<BloodPressureMeasurementRowViewModel> DeleteCommand { get; }
    /// <summary>Gets the settings navigation command.</summary>
    public IAsyncRelayCommand SettingsCommand { get; }
    private async Task RunAsync(Func<Task> operation)
    {
        if (IsBusy)
        {
            return;
        }
        IsBusy = true;
        try
        {
            await operation();
        }
        catch (Exception)
        {
            await dialogs.ShowErrorAsync(AppResources.StorageError);
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async Task RefreshAsync()
    {
        var loaded = await measurements.LoadAsync();
        Rows.Clear();
        editingRow = null;
        original = null;
        foreach (var measurement in loaded)
        {
            var row = new BloodPressureMeasurementRowViewModel();
            row.Load(measurement);
            Rows.Add(row);
        }
    }
    private async Task SaveNewAsync()
    {
        var measurement = NewMeasurement.ToMeasurement();
        if (measurement is null)
        {
            await dialogs.ShowErrorAsync(AppResources.InvalidMeasurement);
            return;
        }
        await measurements.SaveAsync(measurement);
        NewMeasurement.Load(new BloodPressureMeasurement { CreatedDate = DateTime.Now });
        NewMeasurement.Systolic = "";
        NewMeasurement.Diastolic = "";
        await RefreshAsync();
    }
    private void Edit(BloodPressureMeasurementRowViewModel? row)
    {
        if (row is null || IsBusy)
        {
            return;
        }
        if (editingRow is not null)
        {
            Cancel(editingRow);
        }
        original = row.ToMeasurement();
        editingRow = row;
        row.IsEditing = true;
    }
    private void Cancel(BloodPressureMeasurementRowViewModel? row)
    {
        if (row is null || row != editingRow || IsBusy)
        {
            return;
        }
        if (original is not null)
        {
            row.Load(original);
        }
        row.IsEditing = false;
        editingRow = null;
        original = null;
    }
    private async Task SaveRowAsync(BloodPressureMeasurementRowViewModel? row)
    {
        if (row is null || row != editingRow)
        {
            return;
        }
        var measurement = row.ToMeasurement();
        if (measurement is null)
        {
            await dialogs.ShowErrorAsync(AppResources.InvalidMeasurement);
            return;
        }
        await measurements.SaveAsync(measurement);
        await RefreshAsync();
    }
    private async Task DeleteAsync(BloodPressureMeasurementRowViewModel? row)
    {
        if (row is null || !await dialogs.ConfirmDeleteAsync())
        {
            return;
        }
        await measurements.DeleteAsync(row.Id);
        await RefreshAsync();
    }
}
