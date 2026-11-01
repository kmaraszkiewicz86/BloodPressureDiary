using Android.App;
using Android.Content;

namespace BloodPressureDiary.UI.Services;

internal static class ReminderScheduler
{
    internal const string Channel = "measurements";
    internal static void Configure(Context context, bool enabled, TimeSpan time)
    {
        var alarm = (AlarmManager)context.GetSystemService(Context.AlarmService)!;
        var intent = new Intent(context, typeof(ReminderReceiver)).SetAction("BloodPressureDiary.REMIND");
        var pending = PendingIntent.GetBroadcast(context, 100, intent, PendingIntentFlags.UpdateCurrent | (OperatingSystem.IsAndroidVersionAtLeast(23) ? PendingIntentFlags.Immutable : 0))!;
        alarm.Cancel(pending);
        if (!enabled)
        {
            return;
        }
        var next = DateTime.Today.Add(time);
        if (next <= DateTime.Now)
        {
            next = next.AddDays(1);
        }
        // Inexact alarms avoid requiring special exact-alarm access.
        var trigger = new DateTimeOffset(next).ToUnixTimeMilliseconds();
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            alarm.SetAndAllowWhileIdle(AlarmType.RtcWakeup, trigger, pending);
        }
        else
        {
            alarm.Set(AlarmType.RtcWakeup, trigger, pending);
        }
    }
}
