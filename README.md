# BloodPressureDiary

BloodPressureDiary is a .NET 10 MAUI application for recording blood pressure measurements on Android. It uses CommunityToolkit.Mvvm for MVVM, dependency injection for application services, and Entity Framework Core with SQLite for local persistence. Android notifications are scheduled locally and remain disabled until the user enables the reminder in Settings.

## Technical reference

### Application startup and composition

- `MauiProgram.CreateMauiApp()` builds the MAUI host, calls `AddBloodPressureDiaryServices()`, applies EF Core migrations before measurement access, and restores a reminder only when an enabled state and valid saved time exist.
- `ServiceCollectionExtensions.AddBloodPressureDiaryServices()` registers `DiaryDbContext`, repositories, service implementations, reminder schedulers, ViewModels, pages, and `AppShell` in dependency injection.
- `App.CreateWindow(IActivationState?)` initializes application resources once and creates a window containing the injected `AppShell`.
- `AppShell.InitializeComponent()` loads the Shell hierarchy from `AppShell.xaml`. Its `ShellContent` entries define the `MainPage` and `SettingsPage` routes used by `NavigationService`.
- `MainPage` and `SettingsPage` initialize their XAML and assign their injected ViewModels to `BindingContext`.

### Database

- `BloodPressureMeasurement` is the EF Core entity. It stores `Id`, `Systolic`, `Diastolic`, optional `Pulse` and `Notes`, and `MeasuredAt`.
- `DiaryDbContext` exposes `BloodPressureMeasurements` and applies `BloodPressureMeasurementConfiguration` in `OnModelCreating`.
- `BloodPressureMeasurementConfiguration.Configure()` maps the entity to the `Measurements` table, marks required values, and indexes `MeasuredAt`.
- `IBloodPressureMeasurementRepository` defines asynchronous `GetAllAsync`, `AddAsync`, `UpdateAsync`, and `DeleteAsync` operations.
- `BloodPressureMeasurementRepository` implements those operations. `GetAllAsync` orders by `MeasuredAt` descending, and write operations save changes and clear the EF change tracker.

### Measurement ViewModels

- `MainPageViewModel` exposes entry fields, `BloodPressureMeasurements`, `IsEmpty`, and commands for saving, loading, editing, deleting, exporting, and opening Settings.
- `SaveAsync()` validates positive systolic and diastolic values plus an optional positive pulse, persists the entry, clears the input fields, and reloads the list.
- `LoadAsync()` reads records through `IBloodPressureMeasurementRepository` and replaces the observable row collection.
- `EditAsync()`, `SaveEditAsync()`, and `CancelEdit()` start, validate and persist, or discard inline edits. `DeleteAsync()` asks for confirmation before deleting the selected record.
- `ExportBackupAsync()` and `ExportReportAsync()` create CSV or HTML files through `IFileExportService`, then pass them to `IFileSharingService`.
- `TryReadValues()` parses and validates the pressure and pulse input. `SortBloodPressureMeasurements()` restores descending date order after an edit.
- `BloodPressureMeasurementRowViewModel` wraps one entity and its temporary edit values. `BeginEdit()` copies the saved values into the editor, `CancelEdit()` discards the edit state, and `ApplyEdit()` updates the entity and display properties.

### Settings and reminder scheduling

- `SettingsPageViewModel` exposes `ReminderEnabled`, `ReminderTime`, `SaveCommand`, and the command for returning to the measurements tab. `SaveAsync()` delegates persistence and scheduling to `IReminderService` and displays the result through `IUserInteractionService`.
- `IReminderService` exposes `IsEnabled`, `ReminderTime`, `SetReminderAsync()`, and `RestoreAsync()`. `ReminderService` stores the preference and time, schedules only when enabled, cancels when disabled, and restores only an enabled reminder with a valid saved time.
- `IReminderScheduler` abstracts platform scheduling. `AndroidReminderScheduler.ScheduleDailyAsync()` requests notification permission when required and schedules an Android alarm; `CancelAsync()` cancels it. `UnsupportedReminderScheduler` reports scheduling as unavailable on other platforms.
- `DailyReminderBootReceiver.OnReceive()` restores an alarm after device startup only when the saved preference is enabled and the saved time is valid. `DailyReminderReceiver.OnReceive()` checks the enabled preference again before posting a local notification.

### Export, sharing, navigation, and interaction services

- `IFileExportService` defines `CreateBackupAsync()` and `CreateReportAsync()`. `FileExportService` reads measurements from the repository and writes CSV and localized HTML files to the application cache.
- `IFileSharingService.ShareAsync()` accepts a file path and title. `FileSharingService` opens the system share sheet for that file.
- `INavigationService` defines `OpenSettingsAsync()` and `OpenBloodPressureMeasurementsAsync()`. `NavigationService` navigates to the Shell routes declared by `ShellContent`.
- `IUserInteractionService` defines `ConfirmAsync()` and `ShowMessageAsync()`. `UserInteractionService` implements them with MAUI alerts.
- `AppResources` resolves application strings from `AppResources.resx` and `AppResources.pl.resx` using the current UI culture.

### Automated tests

`BloodPressureDiary.UI.Tests` contains xUnit tests for `BloodPressureMeasurementRepository.GetAllAsync()` ordering and `MainPageViewModel.SaveCommand` valid and invalid input behavior. Repository tests use an isolated EF Core InMemory database; ViewModel dependencies are substituted through interfaces.
