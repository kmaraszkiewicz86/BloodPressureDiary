# BloodPressureDiary

BloodPressureDiary is a .NET MAUI application for recording blood pressure measurements on Android. It uses CommunityToolkit.Mvvm, Entity Framework Core, SQLite, and local Android notifications.

## Views

- `MainPage` — Displays the measurements sheet.
- `NewBloodPressureMeasurementView` — Displays the form for a new blood pressure measurement.
- `ExportOptionsView` — Displays the export options form.
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
- `Today` — Gets today's date for date selection.
- `UseHistoryDateRange` — Gets or sets whether the history uses a custom date range.
- `HistoryStartDate` — Gets or sets the first date in the history range.
- `HistoryEndDate` — Gets or sets the last date in the history range.
- `IsExporting` — Gets whether the export options are visible.
- `ExportWindowTitle` — Gets the export options title.
- `SelectedExportFilter` — Gets or sets the selected export period.
- `UseExportDateRange` — Gets or sets whether export uses a custom date range.
- `ExportStartDate` — Gets or sets the first date in the export range.
- `ExportEndDate` — Gets or sets the last date in the export range.
- `PageText` — Gets the current page and total page count.
- `HasPreviousPage` — Gets whether the previous page is available.
- `HasNextPage` — Gets whether the next page is available.
- `IsAddingBloodPressureMeasurement` — Gets whether the new measurement form is visible.
- `SystolicInput` — Gets or sets the new systolic value.
- `DiastolicInput` — Gets or sets the new diastolic value.
- `PulseInput` — Gets or sets the optional new pulse value.
- `NotesInput` — Gets or sets the optional new note.
- `MeasuredDate` — Gets or sets the new measurement date.
- `MeasuredTime` — Gets or sets the new measurement time.
- `SaveCommand` — Gets the save measurement command.
- `AddBloodPressureMeasurementCommand` — Gets the open new measurement form command.
- `CancelNewItemCommand` — Gets the cancel new measurement command.
- `LoadCommand` — Gets the refresh measurements command.
- `FilterHistoryCommand` — Gets the history filter change command.
- `ApplyHistoryDateRangeCommand` — Gets the command that applies the history date range.
- `EditCommand` — Gets the inline edit command.
- `SaveEditCommand` — Gets the inline edit save command.
- `CancelEditCommand` — Gets the inline edit cancel command.
- `DeleteCommand` — Gets the delete command.
- `ExportBackupCommand` — Gets the command that opens CSV backup options.
- `ImportBackupCommand` — Gets the CSV backup import command.
- `ExportReportCommand` — Gets the command that opens HTML report options.
- `ConfirmExportCommand` — Gets the command that creates and shares the selected export.
- `CancelExportCommand` — Gets the command that closes export options.
- `OpenSettingsCommand` — Gets the settings navigation command.
- `PreviousPageCommand` — Gets the previous history page command.
- `NextPageCommand` — Gets the next history page command.
- `LoadAsync()` (private) — Loads saved measurements into the row collection.
- `FilterHistoryAsync()` (private) — Resets pagination and reloads measurements for the selected history filter.
- `ApplyHistoryDateRangeAsync()` (private) — Resets pagination and applies the history date range.
- `OpenNewBloodPressureMeasurement()` (private) — Opens the form for a new measurement.
- `CancelNewBloodPressureMeasurement()` (private) — Closes the new measurement form and clears its inputs.
- `SaveAsync()` (private) — Validates and saves a new measurement, then reloads the rows.
- `EditAsync()` (private) — Starts editing the selected measurement row.
- `SaveEditAsync()` (private) — Validates and saves changes to the selected measurement row.
- `CancelEdit()` (private) — Cancels editing the selected measurement row.
- `DeleteAsync()` (private) — Requests confirmation and deletes the selected measurement.
- `OpenExport()` (private) — Opens the export options for the selected file type.
- `ImportBackupAsync()` (private) — Imports a selected CSV backup and refreshes the history.
- `ExportAsync()` (private) — Creates and shares the selected file for the chosen date range.
- `TryGetDateRange()` (private) — Combines the selected period with a valid custom date range.
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
- `ExistsAsync()` — Checks whether a measurement exists within the specified date range.
- `AddAsync()` — Adds a measurement.
- `AddRangeAsync()` — Adds a collection of measurements.
- `UpdateAsync()` — Updates a measurement.
- `DeleteAsync()` — Removes a measurement by identifier.

### `BloodPressureMeasurementReminderService`

Checks whether a blood pressure measurement reminder is needed today.

- `ShouldShowReminderAsync()` — Checks whether a reminder should be shown when no measurement exists today.

### `FileExportService`

Exports measurements to CSV and HTML files and imports CSV backups.

- `CreateBackupAsync()` — Creates a CSV backup for the specified date range and returns its path.
- `ImportBackupAsync()` — Imports measurements from a selected CSV backup and returns whether a file was selected.
- `CreateReportAsync()` — Creates an HTML report for the specified date range and returns its path.
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

- `Schedule()` (internal) — Schedules the next daily reminder alarm.
- `Cancel()` (internal) — Cancels the daily reminder alarm.
- `GetPendingIntent()` (private) — Creates the pending intent for the daily reminder receiver.

### `UnsupportedReminderScheduler`

Provides a no-op reminder implementation on platforms without local reminder support.

- `ScheduleDailyAsync()` — Schedules a daily notification and reports whether notifications can be shown.
- `CancelAsync()` — Cancels the daily notification.

### `DailyReminderBootReceiver`

Restores the daily alarm after an Android device restart.

### `DailyReminderReceiver`

Displays the daily blood pressure reminder notification.

- `ShowNotification()` (private) — Displays the measurement reminder notification.

### `AppResources`

Provides localized application strings.

- `AppTitle` — Gets the application title.
- `BloodPressureMeasurementsTab` — Gets the measurements tab title.
- `HomeTab` — Gets the home tab title.
- `SettingsTab` — Gets the settings tab title.
- `NewBloodPressureMeasurement` — Gets the new measurement heading.
- `AddBloodPressureMeasurement` — Gets the add measurement action label.
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
- `UseDateRange` — Gets the custom date range label.
- `StartDate` — Gets the start date label.
- `EndDate` — Gets the end date label.
- `ApplyFilter` — Gets the apply filter action label.
- `ExportPeriod` — Gets the export period label.
- `CreateFile` — Gets the create file action label.
- `InvalidDateRange` — Gets the invalid date range message.
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

## Polski

BloodPressureDiary to aplikacja .NET MAUI do zapisywania pomiarów ciśnienia na Androidzie. Wykorzystuje CommunityToolkit.Mvvm, Entity Framework Core, SQLite oraz lokalne powiadomienia Androida.

### Widoki

- `MainPage` — Wyświetla zestawienie pomiarów.
- `NewBloodPressureMeasurementView` — Wyświetla formularz nowego pomiaru ciśnienia.
- `ExportOptionsView` — Wyświetla formularz opcji eksportu.
- `SettingsPage` — Wyświetla ustawienia przypomnienia.
- `AppShell` — Definiuje zakładki pomiarów i ustawień aplikacji.

### Klasy

#### `App`

Udostępnia okno aplikacji.

#### `MauiProgram`

Konfiguruje i buduje aplikację MAUI.

- `CreateMauiApp()` — Tworzy aplikację, rejestruje zależności i stosuje migracje bazy danych.

#### `ServiceCollectionExtensions`

Rejestruje bazę danych aplikacji, serwisy, ViewModele i widoki.

- `AddBloodPressureDiaryServices()` — Rejestruje zależności używane przez dzienniczek ciśnienia.

#### `BloodPressureMeasurement`

Reprezentuje ręcznie zapisany pomiar ciśnienia.

- `Id` — Pobiera lub ustawia identyfikator pomiaru.
- `Systolic` — Pobiera lub ustawia ciśnienie skurczowe w mmHg.
- `Diastolic` — Pobiera lub ustawia ciśnienie rozkurczowe w mmHg.
- `Pulse` — Pobiera lub ustawia opcjonalny puls w uderzeniach na minutę.
- `Notes` — Pobiera lub ustawia opcjonalną notatkę.
- `MeasuredAt` — Pobiera lub ustawia lokalną datę i godzinę pomiaru.

#### `DiaryDbContext`

Udostępnia dostęp do lokalnej bazy danych dzienniczka ciśnienia.

- `BloodPressureMeasurements` — Pobiera zapisane pomiary ciśnienia.

#### `BloodPressureMeasurementConfiguration`

Konfiguruje mapowanie SQLite dla pomiarów ciśnienia.

#### `MainPageViewModel`

Zarządza pomiarami, filtrami historii, paginacją i operacjami na plikach.

- `BloodPressureMeasurements` — Pobiera bieżącą stronę pomiarów w kolejności od najnowszego.
- `IsEmpty` — Pobiera informację, czy obecnie nie ma dostępnych pomiarów.
- `HistoryFilters` — Pobiera dostępne filtry historii.
- `SelectedHistoryFilter` — Pobiera lub ustawia wybrany filtr historii.
- `Today` — Pobiera dzisiejszą datę do wyboru daty.
- `UseHistoryDateRange` — Pobiera lub ustawia informację, czy historia używa własnego zakresu dat.
- `HistoryStartDate` — Pobiera lub ustawia pierwszą datę zakresu historii.
- `HistoryEndDate` — Pobiera lub ustawia ostatnią datę zakresu historii.
- `IsExporting` — Pobiera informację, czy widoczne są opcje eksportu.
- `ExportWindowTitle` — Pobiera tytuł opcji eksportu.
- `SelectedExportFilter` — Pobiera lub ustawia wybrany okres eksportu.
- `UseExportDateRange` — Pobiera lub ustawia informację, czy eksport używa własnego zakresu dat.
- `ExportStartDate` — Pobiera lub ustawia pierwszą datę zakresu eksportu.
- `ExportEndDate` — Pobiera lub ustawia ostatnią datę zakresu eksportu.
- `PageText` — Pobiera bieżący numer strony i liczbę stron.
- `HasPreviousPage` — Pobiera informację, czy dostępna jest poprzednia strona.
- `HasNextPage` — Pobiera informację, czy dostępna jest następna strona.
- `IsAddingBloodPressureMeasurement` — Pobiera informację, czy formularz nowego pomiaru jest widoczny.
- `SystolicInput` — Pobiera lub ustawia nową wartość ciśnienia skurczowego.
- `DiastolicInput` — Pobiera lub ustawia nową wartość ciśnienia rozkurczowego.
- `PulseInput` — Pobiera lub ustawia nową opcjonalną wartość pulsu.
- `NotesInput` — Pobiera lub ustawia nową opcjonalną notatkę.
- `MeasuredDate` — Pobiera lub ustawia datę nowego pomiaru.
- `MeasuredTime` — Pobiera lub ustawia godzinę nowego pomiaru.
- `SaveCommand` — Pobiera polecenie zapisu pomiaru.
- `AddBloodPressureMeasurementCommand` — Pobiera polecenie otwarcia formularza nowego pomiaru.
- `CancelNewItemCommand` — Pobiera polecenie anulowania nowego pomiaru.
- `LoadCommand` — Pobiera polecenie odświeżenia pomiarów.
- `FilterHistoryCommand` — Pobiera polecenie zmiany filtra historii.
- `ApplyHistoryDateRangeCommand` — Pobiera polecenie zastosowania zakresu dat historii.
- `EditCommand` — Pobiera polecenie edycji w wierszu.
- `SaveEditCommand` — Pobiera polecenie zapisu edycji w wierszu.
- `CancelEditCommand` — Pobiera polecenie anulowania edycji w wierszu.
- `DeleteCommand` — Pobiera polecenie usunięcia.
- `ExportBackupCommand` — Pobiera polecenie otwarcia opcji kopii CSV.
- `ImportBackupCommand` — Pobiera polecenie importu kopii CSV.
- `ExportReportCommand` — Pobiera polecenie otwarcia opcji raportu HTML.
- `ConfirmExportCommand` — Pobiera polecenie utworzenia i udostępnienia wybranego pliku.
- `CancelExportCommand` — Pobiera polecenie zamknięcia opcji eksportu.
- `OpenSettingsCommand` — Pobiera polecenie przejścia do ustawień.
- `PreviousPageCommand` — Pobiera polecenie przejścia do poprzedniej strony historii.
- `NextPageCommand` — Pobiera polecenie przejścia do następnej strony historii.
- `LoadAsync()` (prywatna) — Wczytuje zapisane pomiary do kolekcji wierszy.
- `FilterHistoryAsync()` (prywatna) — Resetuje paginację i ponownie wczytuje pomiary dla wybranego filtra historii.
- `ApplyHistoryDateRangeAsync()` (prywatna) — Resetuje paginację i stosuje zakres dat historii.
- `OpenNewBloodPressureMeasurement()` (prywatna) — Otwiera formularz nowego pomiaru.
- `CancelNewBloodPressureMeasurement()` (prywatna) — Zamyka formularz nowego pomiaru i czyści jego pola.
- `SaveAsync()` (prywatna) — Sprawdza i zapisuje nowy pomiar, a następnie ponownie wczytuje wiersze.
- `EditAsync()` (prywatna) — Rozpoczyna edycję wybranego wiersza pomiaru.
- `SaveEditAsync()` (prywatna) — Sprawdza i zapisuje zmiany w wybranym wierszu pomiaru.
- `CancelEdit()` (prywatna) — Anuluje edycję wybranego wiersza pomiaru.
- `DeleteAsync()` (prywatna) — Prosi o potwierdzenie i usuwa wybrany pomiar.
- `OpenExport()` (prywatna) — Otwiera opcje eksportu dla wybranego rodzaju pliku.
- `ImportBackupAsync()` (prywatna) — Importuje wybraną kopię CSV i odświeża historię.
- `ExportAsync()` (prywatna) — Tworzy i udostępnia wybrany plik dla wybranego zakresu dat.
- `TryGetDateRange()` (prywatna) — Łączy wybrany okres z poprawnym własnym zakresem dat.
- `TryReadValues()` (prywatna) — Odczytuje i sprawdza wartości ciśnienia oraz pulsu.
- `ResetNewBloodPressureMeasurement()` (prywatna) — Czyści pola nowego pomiaru i resetuje datę oraz godzinę.
- `UpdatePage()` (prywatna) — Wyświetla bieżącą stronę przefiltrowanych pomiarów.

#### `BloodPressureMeasurementRowViewModel`

Przechowuje pomiar i jego tymczasowe wartości edycji w wierszu.

- `BloodPressureMeasurementRowViewModel()` — Tworzy wiersz dla zapisanego pomiaru.
- `BloodPressureMeasurement` — Pobiera zapisany pomiar.
- `IsEditing` — Pobiera lub ustawia informację, czy wiersz jest edytowany.
- `IsNotEditing` — Pobiera informację, czy wiersz jest obecnie wyświetlany poza trybem edycji.
- `SystolicInput` — Pobiera lub ustawia edytowaną wartość ciśnienia skurczowego.
- `DiastolicInput` — Pobiera lub ustawia edytowaną wartość ciśnienia rozkurczowego.
- `PulseInput` — Pobiera lub ustawia edytowaną wartość pulsu.
- `NotesInput` — Pobiera lub ustawia edytowaną notatkę.
- `MeasuredDate` — Pobiera lub ustawia edytowaną datę.
- `MeasuredTime` — Pobiera lub ustawia edytowaną godzinę.
- `DisplayDateTime` — Pobiera zlokalizowaną datę i godzinę do wyświetlenia.
- `PulseDisplay` — Pobiera wartość wyświetlaną dla opcjonalnego pulsu.
- `NotesDisplay` — Pobiera wartość wyświetlaną dla notatki.
- `BeginEdit()` — Kopiuje bieżące wartości do edytora w wierszu.
- `CancelEdit()` — Odrzuca zmiany wprowadzane w wierszu.
- `ApplyEdit()` — Aktualizuje zapisane wartości i kończy edycję w wierszu.

#### `SettingsPageViewModel`

Zarządza ustawieniami codziennego przypomnienia o pomiarze.

- `ReminderEnabled` — Pobiera lub ustawia informację, czy codzienne przypomnienie jest włączone.
- `ReminderTime` — Pobiera lub ustawia godzinę codziennego przypomnienia.
- `SaveCommand` — Pobiera polecenie zapisu ustawień.
- `OpenBloodPressureMeasurementsCommand` — Pobiera polecenie przejścia do pomiarów.
- `SaveAsync()` (prywatna) — Zapisuje ustawienia przypomnienia i wyświetla wynik.

#### `BloodPressureMeasurementRepository`

Implementuje przechowywanie pomiarów za pomocą Entity Framework Core.

- `GetAllAsync()` — Zwraca pomiary w kolejności od najnowszego.
- `GetAllAsync(startDate, endDate)` — Zwraca pomiary z określonego zakresu dat w kolejności od najnowszego.
- `ExistsAsync()` — Sprawdza, czy istnieje pomiar w określonym zakresie dat.
- `AddAsync()` — Dodaje pomiar.
- `AddRangeAsync()` — Dodaje kolekcję pomiarów.
- `UpdateAsync()` — Aktualizuje pomiar.
- `DeleteAsync()` — Usuwa pomiar na podstawie identyfikatora.

#### `BloodPressureMeasurementReminderService`

Sprawdza, czy dzisiaj potrzebne jest przypomnienie o pomiarze ciśnienia.

- `ShouldShowReminderAsync()` — Sprawdza, czy należy wyświetlić przypomnienie, gdy dzisiaj nie ma pomiaru.

#### `FileExportService`

Eksportuje pomiary do plików CSV i HTML oraz importuje kopie CSV.

- `CreateBackupAsync()` — Tworzy kopię CSV dla określonego zakresu dat i zwraca jej ścieżkę.
- `ImportBackupAsync()` — Importuje pomiary z wybranej kopii CSV i zwraca informację, czy wybrano plik.
- `CreateReportAsync()` — Tworzy raport HTML dla określonego zakresu dat i zwraca jego ścieżkę.
- `Csv()` (prywatna) — Zastępuje znaki specjalne i ujmuje wartość CSV w cudzysłów.
- `WriteAsync()` (prywatna) — Zapisuje podaną zawartość do pliku w pamięci podręcznej i zwraca jego ścieżkę.

#### `FileSharingService`

Udostępnia pliki przez system operacyjny.

- `ShareAsync()` — Otwiera systemowy arkusz udostępniania dla wskazanego pliku.

#### `NavigationService`

Nawiguje między zakładkami pomiarów i ustawień.

- `OpenSettingsAsync()` — Otwiera zakładkę ustawień.
- `OpenBloodPressureMeasurementsAsync()` — Otwiera zakładkę pomiarów.

#### `UserInteractionService`

Używa bieżącego Shell do wyświetlania komunikatów użytkownika.

- `ConfirmAsync()` — Prosi użytkownika o potwierdzenie operacji usuwającej dane.
- `ShowMessageAsync()` — Wyświetla komunikat informacyjny.

#### `ReminderService`

Zapisuje ustawienia przypomnienia i przekazuje planowanie do platformy.

- `IsEnabled` — Pobiera informację, czy codzienne przypomnienia są włączone.
- `ReminderTime` — Pobiera skonfigurowaną godzinę przypomnienia.
- `SetReminderAsync()` — Aktualizuje ustawienie i harmonogram przypomnienia.
- `RestoreAsync()` — Przywraca wcześniej włączone przypomnienie bez pytania o uprawnienie.

#### `AndroidReminderScheduler`

Planuje codzienne powiadomienie o pomiarze za pomocą Android AlarmManager.

- `ScheduleDailyAsync()` — Planuje codzienne powiadomienie i zwraca informację, czy można wyświetlać powiadomienia.
- `CancelAsync()` — Anuluje codzienne powiadomienie.

#### `DailyReminderAlarm` (wewnętrzna)

Planuje i anuluje alarmy przypomnień Androida.

- `Schedule()` (wewnętrzna) — Planuje następny alarm codziennego przypomnienia.
- `Cancel()` (wewnętrzna) — Anuluje alarm codziennego przypomnienia.
- `GetPendingIntent()` (prywatna) — Tworzy oczekujący zamiar dla odbiornika codziennego przypomnienia.

#### `UnsupportedReminderScheduler`

Udostępnia pustą implementację przypomnień na platformach bez obsługi lokalnych przypomnień.

- `ScheduleDailyAsync()` — Planuje codzienne powiadomienie i zwraca informację, czy można wyświetlać powiadomienia.
- `CancelAsync()` — Anuluje codzienne powiadomienie.

#### `DailyReminderBootReceiver`

Przywraca codzienny alarm po ponownym uruchomieniu urządzenia z Androidem.

#### `DailyReminderReceiver`

Wyświetla powiadomienie z codziennym przypomnieniem o pomiarze ciśnienia.

- `ShowNotification()` (prywatna) — Wyświetla powiadomienie przypominające o pomiarze.

#### `AppResources`

Udostępnia zlokalizowane teksty aplikacji.

- `AppTitle` — Pobiera tytuł aplikacji.
- `BloodPressureMeasurementsTab` — Pobiera tytuł zakładki pomiarów.
- `HomeTab` — Pobiera tytuł zakładki głównej.
- `SettingsTab` — Pobiera tytuł zakładki ustawień.
- `NewBloodPressureMeasurement` — Pobiera nagłówek nowego pomiaru.
- `AddBloodPressureMeasurement` — Pobiera etykietę dodawania pomiaru.
- `Systolic` — Pobiera etykietę ciśnienia skurczowego.
- `Diastolic` — Pobiera etykietę ciśnienia rozkurczowego.
- `Pulse` — Pobiera etykietę pulsu.
- `Notes` — Pobiera etykietę notatek.
- `Date` — Pobiera etykietę daty.
- `Time` — Pobiera etykietę godziny.
- `DateAndTime` — Pobiera etykietę daty i godziny.
- `Save` — Pobiera etykietę zapisu.
- `Edit` — Pobiera etykietę edycji.
- `Delete` — Pobiera etykietę usuwania.
- `Cancel` — Pobiera etykietę anulowania.
- `BloodPressureMeasurements` — Pobiera nagłówek zapisanych pomiarów.
- `NoBloodPressureMeasurements` — Pobiera komunikat o braku pomiarów.
- `ExportBackup` — Pobiera etykietę eksportu CSV.
- `ImportBackup` — Pobiera etykietę importu CSV.
- `ImportSucceeded` — Pobiera komunikat o powodzeniu importu.
- `ImportFailed` — Pobiera komunikat o niepowodzeniu importu.
- `HistoryFilter` — Pobiera etykietę filtra historii.
- `UseDateRange` — Pobiera etykietę własnego zakresu dat.
- `StartDate` — Pobiera etykietę daty początkowej.
- `EndDate` — Pobiera etykietę daty końcowej.
- `ApplyFilter` — Pobiera etykietę zastosowania filtra.
- `ExportPeriod` — Pobiera etykietę okresu eksportu.
- `CreateFile` — Pobiera etykietę tworzenia pliku.
- `InvalidDateRange` — Pobiera komunikat o nieprawidłowym zakresie dat.
- `AllMeasurements` — Pobiera etykietę filtra wszystkich pomiarów.
- `Today` — Pobiera etykietę filtra dzisiejszych pomiarów.
- `LastWeek` — Pobiera etykietę filtra ostatniego tygodnia.
- `LastMonth` — Pobiera etykietę filtra ostatniego miesiąca.
- `PreviousPage` — Pobiera etykietę poprzedniej strony.
- `NextPage` — Pobiera etykietę następnej strony.
- `PageNumber` — Pobiera format numeru strony.
- `ExportReport` — Pobiera etykietę raportu HTML.
- `ShareBackup` — Pobiera tytuł udostępniania kopii.
- `ShareReport` — Pobiera tytuł udostępniania raportu.
- `DeleteTitle` — Pobiera tytuł potwierdzenia usunięcia.
- `DeleteConfirmation` — Pobiera komunikat potwierdzenia usunięcia.
- `ValidationTitle` — Pobiera tytuł komunikatu walidacji.
- `RequiredPressure` — Pobiera komunikat walidacji wymaganych wartości ciśnienia.
- `InvalidPulse` — Pobiera komunikat walidacji opcjonalnego pulsu.
- `SaveFailed` — Pobiera komunikat o niepowodzeniu zapisu.
- `DeleteFailed` — Pobiera komunikat o niepowodzeniu usuwania.
- `LoadFailed` — Pobiera komunikat o niepowodzeniu wczytywania.
- `ExportFailed` — Pobiera komunikat o niepowodzeniu eksportu.
- `ReminderTitle` — Pobiera tytuł przypomnienia.
- `DailyReminder` — Pobiera etykietę codziennego przypomnienia.
- `ReminderTime` — Pobiera etykietę godziny przypomnienia.
- `ReminderPermissionDenied` — Pobiera komunikat dotyczący uprawnienia do powiadomień.
- `ReminderSaved` — Pobiera komunikat o zapisaniu przypomnienia.
- `ReportTitle` — Pobiera tytuł raportu.
- `ReminderNotification` — Pobiera tekst powiadomienia przypominającego.
- `Get()` (prywatna) — Zwraca zlokalizowany tekst lub nazwę zasobu, gdy nie znaleziono tłumaczenia.
