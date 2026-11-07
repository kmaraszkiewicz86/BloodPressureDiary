using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Models;
using Microsoft.Maui.ApplicationModel;

namespace BloodPressureDiary.UI.Platforms.Android;

/// <summary>Schedules measurement notifications with Android AlarmManager.</summary>
public sealed class AndroidReminderScheduler(IReminderScheduleCalculator _calculator) : IReminderScheduler
{
    /// <summary>Checks whether notifications can be shown, requesting permission when required.</summary>
    public async Task<bool> CheckPermissionAsync(bool requestPermission)
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

        return true;
    }

    /// <summary>Schedules the next occurrence of a reminder.</summary>
    public void Schedule(ReminderSchedule reminder)
    {
        var next = _calculator.GetNextOccurrence(reminder, DateTime.Now);
        DailyReminderAlarm.Schedule(global::Android.App.Application.Context, reminder.Id, next);
    }

    /// <summary>Cancels a reminder by identifier.</summary>
    public void Cancel(int reminderId) => DailyReminderAlarm.Cancel(global::Android.App.Application.Context, reminderId);
}
