using System.Globalization;
using System.Resources;

namespace BloodPressureDiary.UI.Resources.Localization;

/// <summary>Provides localized application strings.</summary>
public static class AppResources
{
    private static readonly ResourceManager _manager = new("BloodPressureDiary.UI.Resources.Localization.AppResources", typeof(AppResources).Assembly);

    /// <summary>Gets the application title.</summary>
    public static string AppTitle => Get(nameof(AppTitle));
    /// <summary>Gets the measurements tab title.</summary>
    public static string BloodPressureMeasurementsTab => Get(nameof(BloodPressureMeasurementsTab));
    /// <summary>Gets the home tab title.</summary>
    public static string HomeTab => Get(nameof(HomeTab));
    /// <summary>Gets the settings tab title.</summary>
    public static string SettingsTab => Get(nameof(SettingsTab));
    /// <summary>Gets the new measurement heading.</summary>
    public static string NewBloodPressureMeasurement => Get(nameof(NewBloodPressureMeasurement));
    /// <summary>Gets the add measurement action label.</summary>
    public static string AddBloodPressureMeasurement => Get(nameof(AddBloodPressureMeasurement));
    /// <summary>Gets the systolic pressure label.</summary>
    public static string Systolic => Get(nameof(Systolic));
    /// <summary>Gets the diastolic pressure label.</summary>
    public static string Diastolic => Get(nameof(Diastolic));
    /// <summary>Gets the pulse label.</summary>
    public static string Pulse => Get(nameof(Pulse));
    /// <summary>Gets the notes label.</summary>
    public static string Notes => Get(nameof(Notes));
    /// <summary>Gets the date label.</summary>
    public static string Date => Get(nameof(Date));
    /// <summary>Gets the time label.</summary>
    public static string Time => Get(nameof(Time));
    /// <summary>Gets the date and time label.</summary>
    public static string DateAndTime => Get(nameof(DateAndTime));
    /// <summary>Gets the save action label.</summary>
    public static string Save => Get(nameof(Save));
    /// <summary>Gets the edit action label.</summary>
    public static string Edit => Get(nameof(Edit));
    /// <summary>Gets the delete action label.</summary>
    public static string Delete => Get(nameof(Delete));
    /// <summary>Gets the cancel action label.</summary>
    public static string Cancel => Get(nameof(Cancel));
    /// <summary>Gets the saved measurements heading.</summary>
    public static string BloodPressureMeasurements => Get(nameof(BloodPressureMeasurements));
    /// <summary>Gets the empty measurements message.</summary>
    public static string NoBloodPressureMeasurements => Get(nameof(NoBloodPressureMeasurements));
    /// <summary>Gets the CSV export label.</summary>
    public static string ExportBackup => Get(nameof(ExportBackup));
    /// <summary>Gets the CSV import label.</summary>
    public static string ImportBackup => Get(nameof(ImportBackup));
    /// <summary>Gets the import success message.</summary>
    public static string ImportSucceeded => Get(nameof(ImportSucceeded));
    /// <summary>Gets the import failure message.</summary>
    public static string ImportFailed => Get(nameof(ImportFailed));
    /// <summary>Gets the history filter label.</summary>
    public static string HistoryFilter => Get(nameof(HistoryFilter));
    /// <summary>Gets the custom date range label.</summary>
    public static string UseDateRange => Get(nameof(UseDateRange));
    /// <summary>Gets the start date label.</summary>
    public static string StartDate => Get(nameof(StartDate));
    /// <summary>Gets the end date label.</summary>
    public static string EndDate => Get(nameof(EndDate));
    /// <summary>Gets the apply filter action label.</summary>
    public static string ApplyFilter => Get(nameof(ApplyFilter));
    /// <summary>Gets the export period label.</summary>
    public static string ExportPeriod => Get(nameof(ExportPeriod));
    /// <summary>Gets the create file action label.</summary>
    public static string CreateFile => Get(nameof(CreateFile));
    /// <summary>Gets the invalid date range message.</summary>
    public static string InvalidDateRange => Get(nameof(InvalidDateRange));
    /// <summary>Gets the all measurements filter label.</summary>
    public static string AllMeasurements => Get(nameof(AllMeasurements));
    /// <summary>Gets the today filter label.</summary>
    public static string Today => Get(nameof(Today));
    /// <summary>Gets the last week filter label.</summary>
    public static string LastWeek => Get(nameof(LastWeek));
    /// <summary>Gets the last month filter label.</summary>
    public static string LastMonth => Get(nameof(LastMonth));
    /// <summary>Gets the previous page action label.</summary>
    public static string PreviousPage => Get(nameof(PreviousPage));
    /// <summary>Gets the next page action label.</summary>
    public static string NextPage => Get(nameof(NextPage));
    /// <summary>Gets the page number format.</summary>
    public static string PageNumber => Get(nameof(PageNumber));
    /// <summary>Gets the HTML report label.</summary>
    public static string ExportReport => Get(nameof(ExportReport));
    /// <summary>Gets the backup share title.</summary>
    public static string ShareBackup => Get(nameof(ShareBackup));
    /// <summary>Gets the report share title.</summary>
    public static string ShareReport => Get(nameof(ShareReport));
    /// <summary>Gets the delete confirmation title.</summary>
    public static string DeleteTitle => Get(nameof(DeleteTitle));
    /// <summary>Gets the delete confirmation message.</summary>
    public static string DeleteConfirmation => Get(nameof(DeleteConfirmation));
    /// <summary>Gets the validation message title.</summary>
    public static string ValidationTitle => Get(nameof(ValidationTitle));
    /// <summary>Gets the required pressure validation message.</summary>
    public static string RequiredPressure => Get(nameof(RequiredPressure));
    /// <summary>Gets the optional pulse validation message.</summary>
    public static string InvalidPulse => Get(nameof(InvalidPulse));
    /// <summary>Gets the save failure message.</summary>
    public static string SaveFailed => Get(nameof(SaveFailed));
    /// <summary>Gets the delete failure message.</summary>
    public static string DeleteFailed => Get(nameof(DeleteFailed));
    /// <summary>Gets the load failure message.</summary>
    public static string LoadFailed => Get(nameof(LoadFailed));
    /// <summary>Gets the export failure message.</summary>
    public static string ExportFailed => Get(nameof(ExportFailed));
    /// <summary>Gets the reminder title.</summary>
    public static string ReminderTitle => Get(nameof(ReminderTitle));
    /// <summary>Gets the enable reminders label.</summary>
    public static string EnableReminders => Get(nameof(EnableReminders));
    /// <summary>Gets the reminder time label.</summary>
    public static string ReminderTime => Get(nameof(ReminderTime));
    /// <summary>Gets the reminder interval label.</summary>
    public static string ReminderInterval => Get(nameof(ReminderInterval));
    /// <summary>Gets the reminder interval display format.</summary>
    public static string ReminderIntervalFormat => Get(nameof(ReminderIntervalFormat));
    /// <summary>Gets the add reminder time label.</summary>
    public static string AddReminderTime => Get(nameof(AddReminderTime));
    /// <summary>Gets the duplicate reminder time message.</summary>
    public static string DuplicateReminderTime => Get(nameof(DuplicateReminderTime));
    /// <summary>Gets the invalid reminder interval message.</summary>
    public static string InvalidReminderInterval => Get(nameof(InvalidReminderInterval));
    /// <summary>Gets the message requiring a reminder time.</summary>
    public static string ReminderTimeRequired => Get(nameof(ReminderTimeRequired));
    /// <summary>Gets the notification permission message.</summary>
    public static string ReminderPermissionDenied => Get(nameof(ReminderPermissionDenied));
    /// <summary>Gets the reminder saved message.</summary>
    public static string ReminderSaved => Get(nameof(ReminderSaved));
    /// <summary>Gets the report title.</summary>
    public static string ReportTitle => Get(nameof(ReportTitle));
    /// <summary>Gets the reminder notification text.</summary>
    public static string ReminderNotification => Get(nameof(ReminderNotification));

    /// <summary>Returns the localized string or the resource name when no translation is found.</summary>
    private static string Get(string name) => _manager.GetString(name, CultureInfo.CurrentUICulture) ?? name;
}
