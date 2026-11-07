using NativeAlarmManager = global::Android.App.AlarmManager;
using NativeContext = global::Android.Content.Context;
using NativeIntent = global::Android.Content.Intent;
using NativePendingIntent = global::Android.App.PendingIntent;
using NativePendingIntentFlags = global::Android.App.PendingIntentFlags;
using NativeAlarmType = global::Android.App.AlarmType;

namespace BloodPressureDiary.UI.Platforms.Android;

/// <summary>Schedules and cancels Android reminder alarms.</summary>
internal static class DailyReminderAlarm
{
    internal const string ReminderIdExtra = "reminder-id";
    internal const int LegacyReminderId = 7421;

    /// <summary>Schedules the next reminder alarm.</summary>
    internal static void Schedule(NativeContext context, int reminderId, DateTime next)
    {
        var alarmManager = context.GetSystemService(NativeContext.AlarmService) as NativeAlarmManager;
        if (alarmManager is null)
        {
            return;
        }

        var triggerAt = new DateTimeOffset(next).ToUnixTimeMilliseconds();
        var pendingIntent = GetPendingIntent(context, reminderId);
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            alarmManager.SetAndAllowWhileIdle(NativeAlarmType.RtcWakeup, triggerAt, pendingIntent);
        }
        else
        {
            alarmManager.Set(NativeAlarmType.RtcWakeup, triggerAt, pendingIntent);
        }
    }

    /// <summary>Cancels a reminder alarm by identifier.</summary>
    internal static void Cancel(NativeContext context, int reminderId)
    {
        var alarmManager = context.GetSystemService(NativeContext.AlarmService) as NativeAlarmManager;
        alarmManager?.Cancel(GetPendingIntent(context, reminderId));
    }

    /// <summary>Creates the pending intent for the reminder receiver.</summary>
    private static NativePendingIntent GetPendingIntent(NativeContext context, int reminderId)
    {
        var intent = new NativeIntent(context, typeof(DailyReminderReceiver));
        intent.SetAction("BloodPressureDiary.DAILY_REMINDER");
        intent.PutExtra(ReminderIdExtra, reminderId);
        var flags = NativePendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            flags |= NativePendingIntentFlags.Immutable;
        }

        return NativePendingIntent.GetBroadcast(context, reminderId, intent, flags)!;
    }
}
