using BloodPressureDiary.UI.Interfaces;

namespace BloodPressureDiary.UI.Services;

/// <summary>Uses the current Shell to display user prompts.</summary>
public sealed class UserInteractionService : IUserInteractionService
{
    /// <inheritdoc />
    public Task<bool> ConfirmAsync(string title, string message, string accept, string cancel) =>
        Shell.Current.DisplayAlertAsync(title, message, accept, cancel);

    /// <inheritdoc />
    public Task ShowMessageAsync(string title, string message, string close) =>
        Shell.Current.DisplayAlertAsync(title, message, close);
}
