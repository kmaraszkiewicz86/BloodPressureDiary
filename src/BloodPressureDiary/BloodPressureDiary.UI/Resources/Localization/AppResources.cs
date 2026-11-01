using System.Globalization;
using System.Resources;
namespace BloodPressureDiary.UI.Resources.Localization;
/// <summary>Provides localized application strings.</summary>
public static class AppResources
{
    private static readonly ResourceManager Manager = new("BloodPressureDiary.UI.Resources.Localization.AppResources", typeof(AppResources).Assembly);
    /// <summary>Gets the localized AppTitle text.</summary>
    public static string AppTitle => Manager.GetString(nameof(AppTitle), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Measurements text.</summary>
    public static string Measurements => Manager.GetString(nameof(Measurements), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Settings text.</summary>
    public static string Settings => Manager.GetString(nameof(Settings), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Systolic text.</summary>
    public static string Systolic => Manager.GetString(nameof(Systolic), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Diastolic text.</summary>
    public static string Diastolic => Manager.GetString(nameof(Diastolic), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Pulse text.</summary>
    public static string Pulse => Manager.GetString(nameof(Pulse), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Note text.</summary>
    public static string Note => Manager.GetString(nameof(Note), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Date text.</summary>
    public static string Date => Manager.GetString(nameof(Date), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Time text.</summary>
    public static string Time => Manager.GetString(nameof(Time), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Save text.</summary>
    public static string Save => Manager.GetString(nameof(Save), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Edit text.</summary>
    public static string Edit => Manager.GetString(nameof(Edit), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Delete text.</summary>
    public static string Delete => Manager.GetString(nameof(Delete), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Cancel text.</summary>
    public static string Cancel => Manager.GetString(nameof(Cancel), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Actions text.</summary>
    public static string Actions => Manager.GetString(nameof(Actions), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized DailyReminder text.</summary>
    public static string DailyReminder => Manager.GetString(nameof(DailyReminder), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized ReminderTime text.</summary>
    public static string ReminderTime => Manager.GetString(nameof(ReminderTime), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized ConfirmDelete text.</summary>
    public static string ConfirmDelete => Manager.GetString(nameof(ConfirmDelete), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Error text.</summary>
    public static string Error => Manager.GetString(nameof(Error), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Ok text.</summary>
    public static string Ok => Manager.GetString(nameof(Ok), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized StorageError text.</summary>
    public static string StorageError => Manager.GetString(nameof(StorageError), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized InvalidMeasurement text.</summary>
    public static string InvalidMeasurement => Manager.GetString(nameof(InvalidMeasurement), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized NotificationDenied text.</summary>
    public static string NotificationDenied => Manager.GetString(nameof(NotificationDenied), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized ReminderError text.</summary>
    public static string ReminderError => Manager.GetString(nameof(ReminderError), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized ReminderTitle text.</summary>
    public static string ReminderTitle => Manager.GetString(nameof(ReminderTitle), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized ReminderBody text.</summary>
    public static string ReminderBody => Manager.GetString(nameof(ReminderBody), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized ReminderChannel text.</summary>
    public static string ReminderChannel => Manager.GetString(nameof(ReminderChannel), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized Empty text.</summary>
    public static string Empty => Manager.GetString(nameof(Empty), CultureInfo.CurrentUICulture)!;
    /// <summary>Gets the localized ReminderHint text.</summary>
    public static string ReminderHint => Manager.GetString(nameof(ReminderHint), CultureInfo.CurrentUICulture)!;
}
