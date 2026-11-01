using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;
namespace BloodPressureDiary.UI.Services;
/// <summary>Displays localized application dialogs.</summary>
public sealed class DialogService : IDialogService
{
    /// <summary>Asks the user to confirm deletion.</summary>
    public Task<bool> ConfirmDeleteAsync() => Shell.Current.DisplayAlertAsync(AppResources.Delete, AppResources.ConfirmDelete, AppResources.Delete, AppResources.Cancel);
    /// <summary>Displays a localized error message.</summary>
    public Task ShowErrorAsync(string message) => Shell.Current.DisplayAlertAsync(AppResources.Error, message, AppResources.Ok);
}
