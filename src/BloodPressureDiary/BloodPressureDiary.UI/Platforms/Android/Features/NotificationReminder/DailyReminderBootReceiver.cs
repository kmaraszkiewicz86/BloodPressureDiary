using BloodPressureDiary.UI.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using NativeContext = global::Android.Content.Context;
using NativeIntent = global::Android.Content.Intent;
using NativeReceiver = global::Android.Content.BroadcastReceiver;
using NativeLog = global::Android.Util.Log;

namespace BloodPressureDiary.UI.Platforms.Android;

/// <summary>Restores reminder alarms after an Android device restart.</summary>
[global::Android.Content.BroadcastReceiver(Enabled = true, Exported = true)]
[global::Android.App.IntentFilter(new[] { NativeIntent.ActionBootCompleted })]
public sealed class DailyReminderBootReceiver : NativeReceiver
{
    /// <summary>Restores reminder alarms after the Android device finishes restarting.</summary>
    public override async void OnReceive(NativeContext? context, NativeIntent? intent)
    {
        if (context is null || intent?.Action != NativeIntent.ActionBootCompleted)
        {
            return;
        }

        var pendingResult = GoAsync();
        try
        {
            var service = IPlatformApplication.Current?.Services.GetRequiredService<IReminderService>();
            if (service is not null)
            {
                await service.RestoreAsync();
            }
        }
        catch (Exception exception)
        {
            NativeLog.Error(nameof(DailyReminderBootReceiver), exception.ToString());
        }
        finally
        {
            pendingResult?.Finish();
        }
    }
}
