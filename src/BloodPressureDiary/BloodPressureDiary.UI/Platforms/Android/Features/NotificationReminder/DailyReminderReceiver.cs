using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Resources.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using NativeNotificationManager = global::Android.App.NotificationManager;
using NativeNotification = global::Android.App.Notification;
using NativeNotificationChannel = global::Android.App.NotificationChannel;
using NativeNotificationImportance = global::Android.App.NotificationImportance;
using NativePendingIntent = global::Android.App.PendingIntent;
using NativePendingIntentFlags = global::Android.App.PendingIntentFlags;
using NativeContext = global::Android.Content.Context;
using NativeIntent = global::Android.Content.Intent;
using NativeActivityFlags = global::Android.Content.ActivityFlags;
using NativeReceiver = global::Android.Content.BroadcastReceiver;
using NativeLog = global::Android.Util.Log;

namespace BloodPressureDiary.UI.Platforms.Android;

/// <summary>Displays the blood pressure reminder notification.</summary>
[global::Android.Content.BroadcastReceiver(Enabled = true, Exported = false)]
public sealed class DailyReminderReceiver : NativeReceiver
{
    private const string _channelId = "blood-pressure-reminders";
    private const int _notificationId = 7421;

    /// <summary>Reschedules the reminder and shows a notification when no measurement exists today.</summary>
    public override async void OnReceive(NativeContext? context, NativeIntent? intent)
    {
        if (context is null)
        {
            return;
        }

        var pendingResult = GoAsync();
        try
        {
            var services = IPlatformApplication.Current?.Services;
            if (services is null)
            {
                return;
            }

            using var scope = services.CreateScope();
            var reminderId = intent?.GetIntExtra(DailyReminderAlarm.ReminderIdExtra, DailyReminderAlarm.LegacyReminderId) ?? DailyReminderAlarm.LegacyReminderId;
            var schedules = scope.ServiceProvider.GetRequiredService<IReminderService>();
            if (!await schedules.RescheduleAsync(reminderId))
            {
                return;
            }

            var reminderService = scope.ServiceProvider.GetRequiredService<IBloodPressureMeasurementReminderService>();
            if (!await reminderService.ShouldShowReminderAsync() || !await schedules.IsEnabledAsync())
            {
                return;
            }

            ShowNotification(context);
        }
        catch (Exception exception)
        {
            NativeLog.Error(nameof(DailyReminderReceiver), exception.ToString());
        }
        finally
        {
            pendingResult?.Finish();
        }
    }

    /// <summary>Displays the measurement reminder notification.</summary>
    private static void ShowNotification(NativeContext context)
    {
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

        var launchIntent = new NativeIntent(context, typeof(MainActivity));
        launchIntent.SetAction(NativeIntent.ActionMain);
        launchIntent.AddCategory(NativeIntent.CategoryLauncher);
        launchIntent.SetFlags(NativeActivityFlags.NewTask | NativeActivityFlags.SingleTop);
        var pendingIntentFlags = NativePendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            pendingIntentFlags |= NativePendingIntentFlags.Immutable;
        }

        var pendingIntent = NativePendingIntent.GetActivity(context, 0, launchIntent, pendingIntentFlags);
        var notification = builder
            .SetContentTitle(AppResources.ReminderTitle)
            .SetContentText(AppResources.ReminderNotification)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentIntent(pendingIntent)
            .SetAutoCancel(true)
            .Build();
        manager.Notify(_notificationId, notification);
    }
}
