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
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu &&
                await Permissions.RequestAsync<NotificationPermission>() != PermissionStatus.Granted) return false;
            if (!NotificationManagerCompat.From(Android.App.Application.Context).AreNotificationsEnabled()) return false;
        }
        ReminderScheduler.Configure(Android.App.Application.Context, enabled, time);
        return true;
    }
}
internal static class ReminderScheduler
{
    internal const string Channel = "measurements";
    internal static void Configure(Context context, bool enabled, TimeSpan time)
    {
        var alarm = (AlarmManager)context.GetSystemService(Context.AlarmService)!;
        var intent = new Intent(context, typeof(ReminderReceiver)).SetAction("BloodPressureDiary.REMIND");
        var pending = PendingIntent.GetBroadcast(context, 100, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
        alarm.Cancel(pending);
        if (!enabled) return;
        var next = DateTime.Today.Add(time);
        if (next <= DateTime.Now) next = next.AddDays(1);
        // Inexact alarms avoid requiring special exact-alarm access.
        var trigger = new DateTimeOffset(next).ToUnixTimeMilliseconds();
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M) alarm.SetAndAllowWhileIdle(AlarmType.RtcWakeup, trigger, pending);
        else alarm.Set(AlarmType.RtcWakeup, trigger, pending);
    }
}

internal sealed class NotificationPermission : Permissions.BasePlatformPermission
{
    /// <summary>Initializes the Android notification permission request.</summary>
    public NotificationPermission() { }
    /// <summary>Gets the Android runtime permission.</summary>
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new[] { ("android.permission.POST_NOTIFICATIONS", true) };
}
