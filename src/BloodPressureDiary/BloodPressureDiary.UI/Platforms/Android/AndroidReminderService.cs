using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using BloodPressureDiary.UI.Interfaces;
namespace BloodPressureDiary.UI.Services;
/// <summary>Schedules local Android measurement notifications.</summary>
public sealed class AndroidReminderService : IReminderService
{
    /// <summary>Requests notification access and schedules or cancels the daily reminder.</summary>
    public async Task<bool> ConfigureAsync(bool enabled, TimeSpan time)
    {
        if (enabled)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33) &&
                await Permissions.RequestAsync<NotificationPermission>() != PermissionStatus.Granted)
            {
                return false;
            }
            if (NotificationManagerCompat.From(Android.App.Application.Context)?.AreNotificationsEnabled() != true)
            {
                return false;
            }
        }
        ReminderScheduler.Configure(Android.App.Application.Context, enabled, time);
        return true;
    }
}
