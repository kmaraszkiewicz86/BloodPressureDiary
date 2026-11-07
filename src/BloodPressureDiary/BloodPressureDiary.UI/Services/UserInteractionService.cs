using BloodPressureDiary.UI.Interfaces;

namespace BloodPressureDiary.UI.Services;

/// <summary>Uses the current Shell to display user prompts.</summary>
public sealed class UserInteractionService : IUserInteractionService
{
    /// <summary>Asks the user to confirm a destructive action.</summary>
    public Task<bool> ConfirmAsync(string title, string message, string accept, string cancel) =>
        Shell.Current.DisplayAlertAsync(title, message, accept, cancel);

    /// <summary>Displays an informational message.</summary>
    public Task ShowMessageAsync(string title, string message, string close) =>
        Shell.Current.DisplayAlertAsync(title, message, close);
}
