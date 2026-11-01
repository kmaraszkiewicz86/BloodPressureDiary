namespace BloodPressureDiary.UI.Interfaces;

/// <summary>Shares an exported file using the platform share sheet.</summary>
public interface IFileSharingService
{
    /// <summary>Opens the system share sheet for the specified file.</summary>
    Task ShareAsync(string path, string title);
}
