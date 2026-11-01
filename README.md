# BloodPressureDiary

## 1. Application

An Android blood pressure diary with inline measurement editing, daily local reminders, and Polish and English translations. Built with .NET MAUI, CommunityToolkit.Mvvm, EF Core, and SQLite. Tests use xUnit, AutoFixture, NSubstitute, and EF Core InMemory.

## 2. Views

`MainPage` and `SettingsPage` are .NET MAUI `ContentPage` view classes.

## 3. Methods

### App

- `App`: Initializes application resources and shell.
- `CreateWindow`: Creates the main application window.

### AppShell

- `AppShell`: Initializes tabs with injected views.

### DiaryContext

- `DiaryContext`: Initializes the context with injected options.
- `OnModelCreating`: Configures the measurement table.

### InitialDiary

- `Up`: Creates diary storage.
- `Down`: Removes diary storage.
- `BuildTargetModel`: Describes the initial database model.

### DiaryContextModelSnapshot

- `BuildModel`: Builds the current database model.

### MainPage

- `MainPage`: Initializes the measurement view.
- `OnAppearing`: Loads measurements when the tab appears.

### MauiProgram

- `CreateMauiApp`: Builds the diary application.

### AndroidReminderService

- `ConfigureAsync`: Requests notification access and schedules or cancels the daily reminder.

### NotificationPermission

- `NotificationPermission`: Initializes the Android notification permission request.

### ReminderReceiver

- `OnReceive`: Posts a reminder and schedules the next local occurrence.

### DialogService

- `ConfirmDeleteAsync`: Asks the user to confirm deletion.
- `ShowErrorAsync`: Displays a localized error message.

### MeasurementService

- `MeasurementService`: Initializes the measurement service.
- `LoadAsync`: Loads measurements from newest to oldest.
- `SaveAsync`: Inserts or updates a measurement.
- `DeleteAsync`: Deletes a measurement by identifier.

### NavigationService

- `ShowMeasurementsAsync`: Opens the measurements tab.
- `ShowSettingsAsync`: Opens the settings tab.

### SettingsService

- `Save`: Stores reminder preferences.

### SettingsPage

- `SettingsPage`: Initializes the settings view.

### MainViewModel

- `MainViewModel`: Initializes diary commands and dependencies.

### MeasurementRowViewModel

- `Load`: Copies persisted values into the row.
- `ToMeasurement`: Validates input and creates a measurement.

### SettingsViewModel

- `SettingsViewModel`: Initializes settings and navigation commands.
