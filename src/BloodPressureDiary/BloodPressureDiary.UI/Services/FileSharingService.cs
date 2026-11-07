using BloodPressureDiary.UI.Interfaces;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace BloodPressureDiary.UI.Services;

/// <summary>Shares files through the operating system.</summary>
public sealed class FileSharingService : IFileSharingService
{
    /// <summary>Opens the system share sheet for the specified file.</summary>
    public Task ShareAsync(string path, string title) =>
        Share.Default.RequestAsync(new ShareFileRequest(title, new ShareFile(path)));
}
