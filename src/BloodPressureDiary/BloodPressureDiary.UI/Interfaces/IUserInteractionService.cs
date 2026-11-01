namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Displays localized messages and confirmations.</summary>
public interface IUserInteractionService
{
    /// <summary>Asks the user to confirm a destructive action.</summary>
    Task<bool> ConfirmAsync(string title, string message, string accept, string cancel);

    /// <summary>Displays an informational message.</summary>
    Task ShowMessageAsync(string title, string message, string close);
}
