namespace BloodPressureDiary.UI.Services;

internal sealed class NotificationPermission : Permissions.BasePlatformPermission
{
    /// <summary>Initializes the Android notification permission request.</summary>
    public NotificationPermission() { }
    /// <summary>Gets the Android runtime permission.</summary>
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new[] { ("android.permission.POST_NOTIFICATIONS", true) };
}
