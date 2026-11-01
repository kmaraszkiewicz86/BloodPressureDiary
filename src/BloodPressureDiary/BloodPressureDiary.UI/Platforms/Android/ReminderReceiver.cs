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
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                manager.CreateNotificationChannel(new NotificationChannel(ReminderScheduler.Channel, AppResources.ReminderChannel, NotificationImportance.Default));
            if (NotificationManagerCompat.From(context).AreNotificationsEnabled())
            {
                var launch = context.PackageManager!.GetLaunchIntentForPackage(context.PackageName!)!;
                var pending = PendingIntent.GetActivity(context, 101, launch, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
                var notification = new NotificationCompat.Builder(context, ReminderScheduler.Channel)
                    .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                    .SetContentTitle(AppResources.ReminderTitle).SetContentText(AppResources.ReminderBody)
                    .SetAutoCancel(true).SetContentIntent(pending).Build();
                manager.Notify(100, notification);
            }
        }
        ReminderScheduler.Configure(context, true, settings.Time);
    }
}
