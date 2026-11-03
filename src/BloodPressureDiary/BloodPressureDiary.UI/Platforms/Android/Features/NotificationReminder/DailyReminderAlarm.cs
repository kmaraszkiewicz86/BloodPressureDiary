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
    private const int _requestCode = 7421;

    /// <summary>Schedules the next daily reminder alarm.</summary>
    internal static void Schedule(NativeContext context, TimeSpan time)
    {
        var alarmManager = context.GetSystemService(NativeContext.AlarmService) as NativeAlarmManager;
        if (alarmManager is null)
        {
            return;
        }

        var next = DateTime.Today.Add(time);
        if (next <= DateTime.Now)
        {
            next = next.AddDays(1);
        }

        alarmManager.SetInexactRepeating(
            NativeAlarmType.RtcWakeup,
            new DateTimeOffset(next).ToUnixTimeMilliseconds(),
            NativeAlarmManager.IntervalDay,
            GetPendingIntent(context));
    }

    /// <summary>Cancels the daily reminder alarm.</summary>
    internal static void Cancel(NativeContext context)
    {
        var alarmManager = context.GetSystemService(NativeContext.AlarmService) as NativeAlarmManager;
        alarmManager?.Cancel(GetPendingIntent(context));
    }

    /// <summary>Creates the pending intent for the daily reminder receiver.</summary>
    private static NativePendingIntent GetPendingIntent(NativeContext context)
    {
        var intent = new NativeIntent(context, typeof(DailyReminderReceiver));
        intent.SetAction("BloodPressureDiary.DAILY_REMINDER");
        var flags = NativePendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            flags |= NativePendingIntentFlags.Immutable;
        }

        return NativePendingIntent.GetBroadcast(context, _requestCode, intent, flags)!;
    }
}
