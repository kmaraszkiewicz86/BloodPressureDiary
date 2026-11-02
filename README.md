# BloodPressureDiary

BloodPressureDiary is a .NET MAUI application for recording blood pressure measurements on Android. It uses CommunityToolkit.Mvvm, Entity Framework Core, SQLite, and local Android notifications.

## Views

- `MainPage` — Displays the measurements sheet.
- `SettingsPage` — Displays reminder settings.
- `AppShell` — Defines the application's measurements and settings tabs.

## Classes

### `App`

Provides the application window.

### `MauiProgram`

Configures and builds the MAUI application.

- `CreateMauiApp()` — Creates the application, registers dependencies, and applies database migrations.

### `ServiceCollectionExtensions`

Registers the application database, services, view models, and views.

- `AddBloodPressureDiaryServices()` — Registers the dependencies used by the blood pressure diary.

### `BloodPressureMeasurement`

Represents a manually recorded blood pressure measurement.

- `Id` — Gets or sets the measurement identifier.
- `Systolic` — Gets or sets systolic pressure in mmHg.
- `Diastolic` — Gets or sets diastolic pressure in mmHg.
- `Pulse` — Gets or sets the optional pulse in beats per minute.
- `Notes` — Gets or sets the optional note.
- `MeasuredAt` — Gets or sets the local date and time of the measurement.

### `DiaryDbContext`

Provides access to the local blood pressure diary database.

- `BloodPressureMeasurements` — Gets the saved blood pressure measurements.

### `BloodPressureMeasurementConfiguration`

Configures the SQLite mapping for blood pressure measurements.

### `MainPageViewModel`

Manages measurements, history filters, pagination, and file operations.

- `BloodPressureMeasurements` — Gets the current page of measurements in descending date order.
- `IsEmpty` — Gets whether no measurements are currently available.
- `HistoryFilters` — Gets the available history filters.
- `SelectedHistoryFilter` — Gets or sets the selected history filter.
- `PageText` — Gets the current page and total page count.
- `HasPreviousPage` — Gets whether the previous page is available.
- `HasNextPage` — Gets whether the next page is available.
- `SystolicInput` — Gets or sets the new systolic value.
- `DiastolicInput` — Gets or sets the new diastolic value.
- `PulseInput` — Gets or sets the optional new pulse value.
- `NotesInput` — Gets or sets the optional new note.
- `MeasuredDate` — Gets or sets the new measurement date.
- `MeasuredTime` — Gets or sets the new measurement time.
- `SaveCommand` — Gets the save measurement command.
- `LoadCommand` — Gets the refresh measurements command.
- `FilterHistoryCommand` — Gets the history filter change command.
- `EditCommand` — Gets the inline edit command.
- `SaveEditCommand` — Gets the inline edit save command.
- `CancelEditCommand` — Gets the inline edit cancel command.
- `DeleteCommand` — Gets the delete command.
- `ExportBackupCommand` — Gets the CSV backup command.
- `ImportBackupCommand` — Gets the CSV backup import command.
- `ExportReportCommand` — Gets the HTML report command.
- `OpenSettingsCommand` — Gets the settings navigation command.
- `PreviousPageCommand` — Gets the previous history page command.
- `NextPageCommand` — Gets the next history page command.
- `LoadAsync()` (private) — Loads saved measurements into the row collection.
- `FilterHistoryAsync()` (private) — Resets pagination and reloads measurements for the selected history filter.
- `SaveAsync()` (private) — Validates and saves a new measurement, then reloads the rows.
- `EditAsync()` (private) — Starts editing the selected measurement row.
- `SaveEditAsync()` (private) — Validates and saves changes to the selected measurement row.
- `CancelEdit()` (private) — Cancels editing the selected measurement row.
- `DeleteAsync()` (private) — Requests confirmation and deletes the selected measurement.
- `ExportBackupAsync()` (private) — Creates and shares a CSV backup.
- `ImportBackupAsync()` (private) — Imports a selected CSV backup and refreshes the history.
- `ExportReportAsync()` (private) — Creates and shares an HTML report.
- `TryReadValues()` (private) — Parses and validates pressure and pulse values.
- `ResetNewBloodPressureMeasurement()` (private) — Clears the new measurement inputs and resets the date and time.
- `UpdatePage()` (private) — Displays the current page of filtered measurements.

### `BloodPressureMeasurementRowViewModel`

Holds a measurement and its temporary inline edit values.

- `BloodPressureMeasurementRowViewModel()` — Creates a row for a saved measurement.
- `BloodPressureMeasurement` — Gets the saved measurement.
- `IsEditing` — Gets or sets whether the row is being edited.
- `IsNotEditing` — Gets whether the row is currently displayed outside edit mode.
- `SystolicInput` — Gets or sets the editable systolic value.
- `DiastolicInput` — Gets or sets the editable diastolic value.
- `PulseInput` — Gets or sets the editable pulse value.
- `NotesInput` — Gets or sets the editable note.
- `MeasuredDate` — Gets or sets the editable date.
- `MeasuredTime` — Gets or sets the editable time.
- `DisplayDateTime` — Gets a localized date and time for display.
- `PulseDisplay` — Gets the optional pulse display value.
- `NotesDisplay` — Gets the note display value.
- `BeginEdit()` — Copies current values into the inline editor.
- `CancelEdit()` — Discards inline edits.
- `ApplyEdit()` — Updates the saved values and ends inline editing.

### `SettingsPageViewModel`

Manages the daily measurement reminder settings.

- `ReminderEnabled` — Gets or sets whether the daily reminder is enabled.
- `ReminderTime` — Gets or sets the daily reminder time.
- `SaveCommand` — Gets the save settings command.
- `OpenBloodPressureMeasurementsCommand` — Gets the measurements navigation command.
- `SaveAsync()` (private) — Saves the reminder settings and displays the result.

### `BloodPressureMeasurementRepository`

Implements measurement storage with Entity Framework Core.

- `GetAllAsync()` — Returns measurements ordered from newest to oldest.
- `GetAllAsync(startDate, endDate)` — Returns measurements within the specified date range ordered from newest to oldest.
- `AddAsync()` — Adds a measurement.
- `AddRangeAsync()` — Adds a collection of measurements.
- `UpdateAsync()` — Updates a measurement.
- `DeleteAsync()` — Removes a measurement by identifier.

### `FileExportService`

Exports measurements to CSV and HTML files and imports CSV backups.

- `CreateBackupAsync()` — Creates a CSV backup and returns its path.
- `ImportBackupAsync()` — Imports measurements from a selected CSV backup and returns whether a file was selected.
- `CreateReportAsync()` — Creates an HTML report and returns its path.
- `Csv()` (private) — Escapes and quotes a CSV value.
- `WriteAsync()` (private) — Writes the supplied contents to a cache file and returns its path.

### `FileSharingService`

Shares files through the operating system.

- `ShareAsync()` — Opens the system share sheet for the specified file.

### `NavigationService`

Navigates between the measurements and settings tabs.

- `OpenSettingsAsync()` — Opens the settings tab.
- `OpenBloodPressureMeasurementsAsync()` — Opens the measurements tab.

### `UserInteractionService`

Uses the current Shell to display user prompts.

- `ConfirmAsync()` — Asks the user to confirm a destructive action.
- `ShowMessageAsync()` — Displays an informational message.

### `ReminderService`

Persists reminder settings and delegates scheduling to the platform.

- `IsEnabled` — Gets whether daily reminders are enabled.
- `ReminderTime` — Gets the configured reminder time.
- `SetReminderAsync()` — Updates the reminder preference and schedule.
- `RestoreAsync()` — Restores a previously enabled reminder without prompting for permission.

### `AndroidReminderScheduler`

Schedules the daily measurement notification with Android AlarmManager.

- `ScheduleDailyAsync()` — Schedules a daily notification and reports whether notifications can be shown.
- `CancelAsync()` — Cancels the daily notification.

### `DailyReminderAlarm` (internal)

Schedules and cancels Android reminder alarms.

- `GetPendingIntent()` (private) — Creates the pending intent for the daily reminder receiver.

### `UnsupportedReminderScheduler`

Provides a no-op reminder implementation on platforms without local reminder support.

- `ScheduleDailyAsync()` — Schedules a daily notification and reports whether notifications can be shown.
- `CancelAsync()` — Cancels the daily notification.

### `DailyReminderBootReceiver`

Restores the daily alarm after an Android device restart.

### `DailyReminderReceiver`

Displays the daily blood pressure reminder notification.

### `AppResources`

Provides localized application strings.

- `AppTitle` — Gets the application title.
- `BloodPressureMeasurementsTab` — Gets the measurements tab title.
- `HomeTab` — Gets the home tab title.
- `SettingsTab` — Gets the settings tab title.
- `NewBloodPressureMeasurement` — Gets the new measurement heading.
- `Systolic` — Gets the systolic pressure label.
- `Diastolic` — Gets the diastolic pressure label.
- `Pulse` — Gets the pulse label.
- `Notes` — Gets the notes label.
- `Date` — Gets the date label.
- `Time` — Gets the time label.
- `DateAndTime` — Gets the date and time label.
- `Save` — Gets the save action label.
- `Edit` — Gets the edit action label.
- `Delete` — Gets the delete action label.
- `Cancel` — Gets the cancel action label.
- `BloodPressureMeasurements` — Gets the saved measurements heading.
- `NoBloodPressureMeasurements` — Gets the empty measurements message.
- `ExportBackup` — Gets the CSV export label.
- `ImportBackup` — Gets the CSV import label.
- `ImportSucceeded` — Gets the import success message.
- `ImportFailed` — Gets the import failure message.
- `HistoryFilter` — Gets the history filter label.
- `AllMeasurements` — Gets the all measurements filter label.
- `Today` — Gets the today filter label.
- `LastWeek` — Gets the last week filter label.
- `LastMonth` — Gets the last month filter label.
- `PreviousPage` — Gets the previous page action label.
- `NextPage` — Gets the next page action label.
- `PageNumber` — Gets the page number format.
- `ExportReport` — Gets the HTML report label.
- `ShareBackup` — Gets the backup share title.
- `ShareReport` — Gets the report share title.
- `DeleteTitle` — Gets the delete confirmation title.
- `DeleteConfirmation` — Gets the delete confirmation message.
- `ValidationTitle` — Gets the validation message title.
- `RequiredPressure` — Gets the required pressure validation message.
- `InvalidPulse` — Gets the optional pulse validation message.
- `SaveFailed` — Gets the save failure message.
- `DeleteFailed` — Gets the delete failure message.
- `LoadFailed` — Gets the load failure message.
- `ExportFailed` — Gets the export failure message.
- `ReminderTitle` — Gets the reminder title.
- `DailyReminder` — Gets the daily reminder label.
- `ReminderTime` — Gets the reminder time label.
- `ReminderPermissionDenied` — Gets the notification permission message.
- `ReminderSaved` — Gets the reminder saved message.
- `ReportTitle` — Gets the report title.
- `ReminderNotification` — Gets the reminder notification text.
- `Get()` (private) — Returns the localized string or the resource name when no translation is found.
