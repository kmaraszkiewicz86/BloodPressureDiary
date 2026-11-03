using Microsoft.Maui.Storage;
using System.Globalization;
using NativeContext = global::Android.Content.Context;
using NativeIntent = global::Android.Content.Intent;
using NativeReceiver = global::Android.Content.BroadcastReceiver;

namespace BloodPressureDiary.UI.Platforms.Android;

/// <summary>Restores the daily alarm after an Android device restart.</summary>
[global::Android.Content.BroadcastReceiver(Enabled = true, Exported = true)]
[global::Android.App.IntentFilter(new[] { NativeIntent.ActionBootCompleted })]
public sealed class DailyReminderBootReceiver : NativeReceiver
{
    /// <inheritdoc />
    public override void OnReceive(NativeContext? context, NativeIntent? intent)
    {
        if (context is null || !Preferences.Default.Get("daily-reminder-enabled", false))
        {
            return;
        }

        var savedTime = Preferences.Default.Get("daily-reminder-time", string.Empty);
        if (TimeSpan.TryParse(savedTime, CultureInfo.InvariantCulture, out var time))
        {
            DailyReminderAlarm.Schedule(context, time);
        }
    }
}
