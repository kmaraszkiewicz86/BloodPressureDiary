using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using BloodPressureDiary.UI.Resources.Localization;
using BloodPressureDiary.UI.Services;
namespace BloodPressureDiary.UI;
/// <summary>Delivers local reminders and restores their schedule after device changes.</summary>
[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionTimeChanged, Intent.ActionTimezoneChanged, Intent.ActionMyPackageReplaced })]
public sealed class ReminderReceiver : BroadcastReceiver
{
    /// <summary>Posts a reminder and schedules the next local occurrence.</summary>
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null) return;
        var settings = new SettingsService();
        if (!settings.Enabled) return;
        if (intent?.Action == "BloodPressureDiary.REMIND")
        {
            var manager = (NotificationManager)context.GetSystemService(Context.NotificationService)!;
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
                manager.CreateNotificationChannel(new NotificationChannel(ReminderScheduler.Channel, AppResources.ReminderChannel, NotificationImportance.Default));
            if (NotificationManagerCompat.From(context)?.AreNotificationsEnabled() == true)
            {
                var launch = context.PackageManager!.GetLaunchIntentForPackage(context.PackageName!)!;
                var pending = PendingIntent.GetActivity(context, 101, launch, PendingIntentFlags.UpdateCurrent | (OperatingSystem.IsAndroidVersionAtLeast(23) ? PendingIntentFlags.Immutable : 0));
                var builder = new NotificationCompat.Builder(context, ReminderScheduler.Channel);
                builder.SetSmallIcon(Android.Resource.Drawable.IcDialogInfo);
                builder.SetContentTitle(AppResources.ReminderTitle);
                builder.SetContentText(AppResources.ReminderBody);
                builder.SetAutoCancel(true);
                builder.SetContentIntent(pending);
                manager.Notify(100, builder.Build());
            }
        }
        ReminderScheduler.Configure(context, true, settings.Time);
    }
}
