# Android notification reminder

This feature schedules a local Android reminder for a blood pressure measurement. It shows the notification only when reminders are enabled and no measurement is recorded for the current local day.

## English

### Classes and related files

| File | Responsibility |
| --- | --- |
| [AndroidReminderScheduler.cs](AndroidReminderScheduler.cs) | Checks notification permission when requested and delegates alarm scheduling and cancellation. |
| [DailyReminderAlarm.cs](DailyReminderAlarm.cs) | Creates, schedules, and cancels the Android alarm and its broadcast `PendingIntent`. |
| [DailyReminderBootReceiver.cs](DailyReminderBootReceiver.cs) | Restores an enabled reminder after the device finishes restarting. |
| [DailyReminderReceiver.cs](DailyReminderReceiver.cs) | Receives the alarm, checks whether a measurement exists today, and displays the notification when needed. |
| [ReminderService.cs](../../../../Services/ReminderService.cs) | Stores the enabled state and time in MAUI preferences and calls the platform scheduler. |
| [BloodPressureMeasurementReminderService.cs](../../../../Services/BloodPressureMeasurementReminderService.cs) | Checks whether today's measurement exists through the repository. |
| [BloodPressureMeasurementRepository.cs](../../../../Services/BloodPressureMeasurementRepository.cs) | Checks the measurement date range in the database. |
| [SettingsPageViewModel.cs](../../../../ViewModels/SettingsPageViewModel.cs) | Saves the reminder settings selected by the user. |

### 1. Enable or change the reminder

1. In settings, the user enables the reminder, chooses a time, and saves the settings.
2. `SettingsPageViewModel.SaveAsync()` calls `ReminderService.SetReminderAsync()`.
3. On Android 13 and later, `AndroidReminderScheduler.ScheduleDailyAsync()` checks and, when needed, requests notification permission. If permission is denied, it reports failure and the new settings are not saved.
4. `DailyReminderAlarm.Schedule()` chooses the next occurrence: today at the selected time, or tomorrow if that time has already passed. It registers a repeating alarm with `AlarmManager`.
5. After scheduling, `ReminderService` saves the time under `daily-reminder-time` and the enabled state under `daily-reminder-enabled` in MAUI preferences.

The alarm uses `SetInexactRepeating`, so Android can deliver it later than the selected time. See the [Android AlarmManager reference](https://developer.android.com/reference/android/app/AlarmManager#setInexactRepeating(int,%20long,%20long,%20android.app.PendingIntent)).

### 2. Check and display the notification

1. When Android delivers the alarm, its broadcast `PendingIntent` starts `DailyReminderReceiver.OnReceive()`.
2. The receiver exits if the reminder is disabled or the Android context is unavailable. Otherwise, it calls `GoAsync()` and creates a dependency-injection scope.
3. `BloodPressureMeasurementReminderService.ShouldShowReminderAsync()` asks the repository whether a measurement's `MeasuredAt` is at or after local midnight today and before local midnight tomorrow.
4. If a measurement exists, the receiver ends without displaying a notification. It also checks again whether the reminder is still enabled after the database query.
5. If no measurement exists and the reminder is enabled, `ShowNotification()` creates the channel when needed and displays the notification. Tapping it opens `MainActivity`; the notification is dismissed automatically after the tap.
6. The receiver calls `Finish()` on the pending broadcast in a `finally` block. Exceptions are written to the Android log and no notification is displayed for that invocation.

The check uses the measurement date and time stored in `MeasuredAt`, not a separate date of data entry. A measurement saved for a different day does not satisfy today's check.

### 3. Restart, startup, and cancellation

- `DailyReminderBootReceiver` receives `BOOT_COMPLETED`. It schedules the saved time only if reminders are enabled and the stored time can be parsed.
- During application startup, `MauiProgram` applies database migrations and calls `ReminderService.RestoreAsync()`. Restoration schedules an enabled reminder without requesting notification permission again; otherwise it cancels the alarm.
- When the user disables the reminder and saves, `ReminderService.SetReminderAsync()` stores the disabled state and calls the scheduler's `CancelAsync()`.
- `AndroidManifest.xml` declares `POST_NOTIFICATIONS` and `RECEIVE_BOOT_COMPLETED`.

### 4. Notes for modifications

- `DailyReminderAlarm` uses request code `7421` for the alarm broadcast. `DailyReminderReceiver` separately uses notification ID `7421` for `NotificationManager.Notify`; these are different identifiers.
- Scheduling the same broadcast `PendingIntent` again updates the existing alarm. Cancellation must use the matching `PendingIntent`.
- Android creates broadcast receivers; `DailyReminderReceiver` obtains the registered reminder service from `IPlatformApplication.Current.Services` and creates a scope for the database-backed service.
- The receiver classes retain the `BloodPressureDiary.UI.Platforms.Android` namespace. Changing their Android component identity can affect alarms already scheduled on a device.

## Polski

Ta funkcjonalność planuje lokalne przypomnienie Androida o pomiarze ciśnienia. Wyświetla powiadomienie tylko wtedy, gdy przypomnienia są włączone i nie ma pomiaru z bieżącego dnia według lokalnej daty urządzenia.

### Klasy i powiązane pliki

| Plik | Odpowiedzialność |
| --- | --- |
| [AndroidReminderScheduler.cs](AndroidReminderScheduler.cs) | Sprawdza uprawnienie do powiadomień, gdy jest to wymagane, oraz przekazuje planowanie i anulowanie alarmu. |
| [DailyReminderAlarm.cs](DailyReminderAlarm.cs) | Tworzy, planuje i anuluje alarm Androida oraz jego `PendingIntent` wysyłający broadcast. |
| [DailyReminderBootReceiver.cs](DailyReminderBootReceiver.cs) | Przywraca włączone przypomnienie po zakończeniu ponownego uruchamiania urządzenia. |
| [DailyReminderReceiver.cs](DailyReminderReceiver.cs) | Odbiera alarm, sprawdza, czy istnieje dzisiejszy pomiar, i w razie potrzeby wyświetla powiadomienie. |
| [ReminderService.cs](../../../../Services/ReminderService.cs) | Zapisuje stan włączenia i godzinę w preferencjach MAUI oraz wywołuje platformowy mechanizm planowania. |
| [BloodPressureMeasurementReminderService.cs](../../../../Services/BloodPressureMeasurementReminderService.cs) | Sprawdza przez repozytorium, czy istnieje dzisiejszy pomiar. |
| [BloodPressureMeasurementRepository.cs](../../../../Services/BloodPressureMeasurementRepository.cs) | Sprawdza zakres dat pomiarów w bazie danych. |
| [SettingsPageViewModel.cs](../../../../ViewModels/SettingsPageViewModel.cs) | Zapisuje ustawienia przypomnienia wybrane przez użytkownika. |

### 1. Włączenie lub zmiana przypomnienia

1. W ustawieniach użytkownik włącza przypomnienie, wybiera godzinę i zapisuje ustawienia.
2. `SettingsPageViewModel.SaveAsync()` wywołuje `ReminderService.SetReminderAsync()`.
3. Na Androidzie 13 i nowszym `AndroidReminderScheduler.ScheduleDailyAsync()` sprawdza uprawnienie do powiadomień i w razie potrzeby o nie prosi. Jeśli uprawnienie zostanie odrzucone, zwraca informację o niepowodzeniu, a nowe ustawienia nie są zapisywane.
4. `DailyReminderAlarm.Schedule()` wybiera następne wystąpienie: dzisiaj o wybranej godzinie albo jutro, jeśli ta godzina już minęła. Następnie rejestruje powtarzający się alarm w `AlarmManager`.
5. Po zaplanowaniu `ReminderService` zapisuje w preferencjach MAUI godzinę pod kluczem `daily-reminder-time`, a stan włączenia pod kluczem `daily-reminder-enabled`.

Alarm używa `SetInexactRepeating`, więc Android może dostarczyć go później niż o wybranej godzinie. Szczegóły znajdują się w [dokumentacji Android AlarmManager](https://developer.android.com/reference/android/app/AlarmManager#setInexactRepeating(int,%20long,%20long,%20android.app.PendingIntent)).

### 2. Sprawdzenie i wyświetlenie powiadomienia

1. Gdy Android dostarczy alarm, jego `PendingIntent` uruchamia `DailyReminderReceiver.OnReceive()`.
2. Odbiornik kończy pracę, jeśli przypomnienie jest wyłączone lub kontekst Androida jest niedostępny. W przeciwnym razie wywołuje `GoAsync()` i tworzy zakres dependency injection.
3. `BloodPressureMeasurementReminderService.ShouldShowReminderAsync()` sprawdza przez repozytorium, czy wartość `MeasuredAt` któregoś pomiaru przypada od dzisiejszej lokalnej północy do jutrzejszej północy, bez niej.
4. Jeśli pomiar istnieje, odbiornik kończy pracę bez wyświetlenia powiadomienia. Po zapytaniu do bazy ponownie sprawdza również, czy przypomnienie nadal jest włączone.
5. Jeśli pomiaru nie ma i przypomnienie jest włączone, `ShowNotification()` w razie potrzeby tworzy kanał i wyświetla powiadomienie. Jego naciśnięcie otwiera `MainActivity` i automatycznie usuwa powiadomienie.
6. W bloku `finally` odbiornik wywołuje `Finish()` dla oczekującego broadcastu. Wyjątki są zapisywane w dzienniku Androida, a dla danego wywołania powiadomienie nie jest wyświetlane.

Sprawdzana jest data i godzina pomiaru zapisana w `MeasuredAt`, a nie osobna data wprowadzenia danych. Pomiar zapisany dla innego dnia nie spełnia warunku dzisiejszego pomiaru.

### 3. Restart, uruchomienie aplikacji i anulowanie

- `DailyReminderBootReceiver` odbiera `BOOT_COMPLETED`. Planuje zapisaną godzinę tylko wtedy, gdy przypomnienia są włączone i można odczytać zapisaną godzinę.
- Podczas uruchamiania aplikacji `MauiProgram` stosuje migracje bazy danych i wywołuje `ReminderService.RestoreAsync()`. Przywracanie planuje włączone przypomnienie bez ponownego pytania o uprawnienie; w przeciwnym razie anuluje alarm.
- Gdy użytkownik wyłączy przypomnienie i zapisze ustawienia, `ReminderService.SetReminderAsync()` zapisuje stan wyłączenia i wywołuje `CancelAsync()` mechanizmu planującego.
- `AndroidManifest.xml` deklaruje uprawnienia `POST_NOTIFICATIONS` i `RECEIVE_BOOT_COMPLETED`.

### 4. Wskazówki do modyfikacji

- `DailyReminderAlarm` używa kodu żądania `7421` dla broadcastu alarmu. `DailyReminderReceiver` osobno używa identyfikatora powiadomienia `7421` w `NotificationManager.Notify`; są to dwa różne identyfikatory.
- Ponowne zaplanowanie tego samego broadcastu `PendingIntent` aktualizuje istniejący alarm. Anulowanie musi użyć pasującego `PendingIntent`.
- Odbiorniki broadcastów są tworzone przez Androida; `DailyReminderReceiver` pobiera zarejestrowany serwis przypomnień z `IPlatformApplication.Current.Services` i tworzy zakres dla serwisu korzystającego z bazy danych.
- Klasy odbiorników zachowują przestrzeń nazw `BloodPressureDiary.UI.Platforms.Android`. Zmiana tożsamości ich komponentów Androida może wpłynąć na alarmy zaplanowane już na urządzeniu.
