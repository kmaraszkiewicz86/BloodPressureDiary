using BloodPressureDiary.UI.Resources.Localization;
using Microsoft.Maui.Storage;
using NativeNotificationManager = global::Android.App.NotificationManager;
using NativeNotification = global::Android.App.Notification;
using NativeNotificationChannel = global::Android.App.NotificationChannel;
using NativeNotificationImportance = global::Android.App.NotificationImportance;
using NativeContext = global::Android.Content.Context;
using NativeIntent = global::Android.Content.Intent;
using NativeReceiver = global::Android.Content.BroadcastReceiver;

namespace BloodPressureDiary.UI.Platforms.Android;

/// <summary>Displays the daily blood pressure reminder notification.</summary>
[global::Android.Content.BroadcastReceiver(Enabled = true, Exported = false)]
public sealed class DailyReminderReceiver : NativeReceiver
{
    private const string _channelId = "blood-pressure-reminders";

    /// <inheritdoc />
    public override void OnReceive(NativeContext? context, NativeIntent? intent)
    {
        if (context is null || !Preferences.Default.Get("daily-reminder-enabled", false))
        {
            return;
        }

        var manager = (NativeNotificationManager?)context.GetSystemService(NativeContext.NotificationService);
        if (manager is null)
        {
            return;
        }

        NativeNotification.Builder builder;
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            manager.CreateNotificationChannel(new NativeNotificationChannel(
                _channelId, AppResources.ReminderTitle, NativeNotificationImportance.Default));
            builder = new NativeNotification.Builder(context, _channelId);
        }
        else
        {
            builder = new NativeNotification.Builder(context);
        }

        var notification = builder
            .SetContentTitle(AppResources.ReminderTitle)
            .SetContentText(AppResources.ReminderNotification)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetAutoCancel(true)
            .Build();
        manager.Notify(7421, notification);
    }
}
