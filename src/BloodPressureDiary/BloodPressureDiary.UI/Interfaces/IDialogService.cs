namespace BloodPressureDiary.UI.Interfaces;
/// <summary>Defines DialogService operations.</summary>
public interface IDialogService
{
    /// <summary>Asks the user to confirm deletion.</summary>
    Task<bool> ConfirmDeleteAsync();
    /// <summary>Displays a localized error message.</summary>
    Task ShowErrorAsync(string message);
}
