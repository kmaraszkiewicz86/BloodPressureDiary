using BloodPressureDiary.UI.Interfaces;
using Microsoft.Maui.ApplicationModel;

namespace BloodPressureDiary.UI.Platforms.Android;

/// <summary>Schedules the daily measurement notification with Android AlarmManager.</summary>
public sealed class AndroidReminderScheduler : IReminderScheduler
{
    /// <inheritdoc />
    public async Task<bool> ScheduleDailyAsync(TimeSpan time, bool requestPermission)
    {
        if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Tiramisu && requestPermission)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await MainThread.InvokeOnMainThreadAsync(Permissions.RequestAsync<Permissions.PostNotifications>);
            }

            if (status != PermissionStatus.Granted)
            {
                return false;
            }
        }

        var context = global::Android.App.Application.Context;
        DailyReminderAlarm.Schedule(context, time);
        return true;
    }

    /// <inheritdoc />
    public Task CancelAsync()
    {
        DailyReminderAlarm.Cancel(global::Android.App.Application.Context);
        return Task.CompletedTask;
    }
}
