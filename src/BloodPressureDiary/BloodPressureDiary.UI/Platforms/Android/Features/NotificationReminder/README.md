# Android notification reminders

This feature schedules local Android reminders for blood pressure measurements. The user can select multiple times with an individual interval in days for each; a notification is displayed only when reminders are enabled and no measurement exists for the current local day.

## English

### Classes and related files

| File | Responsibility |
| --- | --- |
| [AndroidReminderScheduler.cs](AndroidReminderScheduler.cs) | Checks notification permission and schedules each reminder's next occurrence. |
| [DailyReminderAlarm.cs](DailyReminderAlarm.cs) | Registers and cancels individual Android alarms and their broadcast `PendingIntent` objects. |
| [DailyReminderBootReceiver.cs](DailyReminderBootReceiver.cs) | Restores enabled reminders after an Android device restart. |
| [DailyReminderReceiver.cs](DailyReminderReceiver.cs) | Reschedules the delivered reminder and displays a notification only when needed. |
| [ReminderSchedule.cs](../../../../Models/ReminderSchedule.cs) | Stores an identifier, time, interval, and starting date for a reminder. |
| [ReminderService.cs](../../../../Services/ReminderService.cs) | Manages settings and schedules. |
| [ReminderSettingsStorage.cs](../../../../Services/ReminderSettingsStorage.cs) | Asynchronously reads and writes settings and migrates previous preferences. |
| [ReminderScheduleCalculator.cs](../../../../Services/ReminderScheduleCalculator.cs) | Calculates the next occurrence from the saved starting date, time, and interval. |
| [BloodPressureMeasurementReminderService.cs](../../../../Services/BloodPressureMeasurementReminderService.cs) | Checks through the repository whether a measurement exists today. |
| [BloodPressureMeasurementRepository.cs](../../../../Services/BloodPressureMeasurementRepository.cs) | Checks the measurement date range in the database. |
| [SettingsPageViewModel.cs](../../../../ViewModels/SettingsPageViewModel.cs) | Manages and validates each reminder time and interval. |

### 1. Configure reminders

1. In settings, enable reminders. For each reminder, select a time, enter its positive interval in days, and select **Add reminder**. Save the settings after adding the reminders.
2. `SettingsPageViewModel.AddReminderAsync()` validates the new time and interval; `SaveAsync()` calls `ReminderService.SetRemindersAsync()`.
3. On Android 13 and later, `AndroidReminderScheduler.CheckPermissionAsync()` checks and, when needed, requests notification permission. If permission is denied, the previous settings and alarms are retained.
4. `ReminderService` asynchronously saves the enabled state and schedules in a local JSON file, cancels the previously saved alarms, and schedules each selected time when reminders are enabled.
5. Each schedule stores its own interval and starting date. Saving an unchanged reminder preserves its starting date while reminders remain enabled. Enabling reminders or changing a reminder's interval starts that reminder's cycle on the current local day.

To change an existing reminder's interval, remove that reminder, add the same time with the new interval, and save.

Existing schedules stored in MAUI preferences are migrated to the JSON file when first read. An existing single reminder stored under `daily-reminder-time` is converted to a schedule with the same time and a one-day interval. No reminder is enabled automatically.

### 2. Schedule and deliver alarms

1. `ReminderScheduleCalculator.GetNextOccurrence()` calculates the next local date and time using the schedule's starting date and interval. Past times on the current reminder day move to the next scheduled day.
2. `DailyReminderAlarm.Schedule()` registers a separate one-shot alarm for each time. On Android 6 and later it uses `SetAndAllowWhileIdle`; older versions use `Set`.
3. Android delivers the alarm to `DailyReminderReceiver`. The receiver checks the reminder identifier and schedules its next occurrence before checking the measurement. Therefore, skipping today's notification does not cancel later occurrences.
4. `BloodPressureMeasurementReminderService.ShouldShowReminderAsync()` checks the repository for a measurement whose `MeasuredAt` is at or after local midnight today and before local midnight tomorrow.
5. If a measurement exists, or reminders were disabled, no notification is displayed. Otherwise, the receiver displays the notification. Tapping it opens the application and dismisses the notification.
6. The receiver uses `GoAsync()` and calls `Finish()` in `finally`. Exceptions are written to the Android log.

These are inexact alarms: Android may deliver them later than the selected time, including because of battery restrictions. See the [Android alarm scheduling documentation](https://developer.android.com/develop/background-work/services/alarms).

### 3. Restart and cancellation

- During application startup, `MauiProgram` applies database migrations, and the application window asynchronously calls `ReminderService.RestoreAsync()` when created. The service restores all enabled schedules without requesting permission again.
- `DailyReminderBootReceiver` also calls `RestoreAsync()` after `BOOT_COMPLETED`. The saved starting date keeps the interval aligned across restarts.
- Saving changed times cancels the previous alarms before scheduling the new list. Disabling reminders cancels the saved alarms.
- `AndroidManifest.xml` declares `POST_NOTIFICATIONS` and `RECEIVE_BOOT_COMPLETED`.

### 4. Notes for modifications

- Every reminder has a distinct identifier used as the broadcast `PendingIntent` request code. The legacy single reminder retains identifier `7421`; newly added times use identifiers derived from their minute of the day.
- `DailyReminderReceiver` separately uses notification ID `7421` for `NotificationManager.Notify`. This is independent of the alarm identifiers.
- Cancellation must use the same receiver, action, and request code as the scheduled `PendingIntent`.
- Android creates broadcast receivers. The receiver obtains services from `IPlatformApplication.Current.Services` and creates a scope for the database-backed check.
- The receiver classes retain the `BloodPressureDiary.UI.Platforms.Android` namespace. Changing their Android component identity can affect alarms already scheduled on a device.

## Polski

Ta funkcjonalność planuje lokalne przypomnienia Androida o pomiarach ciśnienia. Użytkownik może wybrać wiele godzin i osobny odstęp w dniach dla każdej z nich; powiadomienie jest wyświetlane tylko wtedy, gdy przypomnienia są włączone i nie ma pomiaru z bieżącego dnia według lokalnej daty urządzenia.

### Klasy i powiązane pliki

| Plik | Odpowiedzialność |
| --- | --- |
| [AndroidReminderScheduler.cs](AndroidReminderScheduler.cs) | Sprawdza uprawnienie do powiadomień i planuje następne wystąpienie każdego przypomnienia. |
| [DailyReminderAlarm.cs](DailyReminderAlarm.cs) | Rejestruje i anuluje poszczególne alarmy Androida oraz ich obiekty `PendingIntent` wysyłające broadcast. |
| [DailyReminderBootReceiver.cs](DailyReminderBootReceiver.cs) | Przywraca włączone przypomnienia po restarcie urządzenia z Androidem. |
| [DailyReminderReceiver.cs](DailyReminderReceiver.cs) | Ponownie planuje dostarczone przypomnienie i wyświetla powiadomienie tylko wtedy, gdy jest potrzebne. |
| [ReminderSchedule.cs](../../../../Models/ReminderSchedule.cs) | Przechowuje identyfikator, godzinę, odstęp i datę rozpoczęcia przypomnienia. |
| [ReminderService.cs](../../../../Services/ReminderService.cs) | Zarządza ustawieniami i harmonogramami. |
| [ReminderSettingsStorage.cs](../../../../Services/ReminderSettingsStorage.cs) | Asynchronicznie odczytuje i zapisuje ustawienia oraz przenosi poprzednie preferencje. |
| [ReminderScheduleCalculator.cs](../../../../Services/ReminderScheduleCalculator.cs) | Oblicza następne wystąpienie na podstawie zapisanej daty rozpoczęcia, godziny i odstępu. |
| [BloodPressureMeasurementReminderService.cs](../../../../Services/BloodPressureMeasurementReminderService.cs) | Sprawdza przez repozytorium, czy istnieje dzisiejszy pomiar. |
| [BloodPressureMeasurementRepository.cs](../../../../Services/BloodPressureMeasurementRepository.cs) | Sprawdza zakres dat pomiarów w bazie danych. |
| [SettingsPageViewModel.cs](../../../../ViewModels/SettingsPageViewModel.cs) | Zarządza godziną i odstępem każdego przypomnienia oraz sprawdza ich poprawność. |

### 1. Konfiguracja przypomnień

1. W ustawieniach włącz przypomnienia. Dla każdego przypomnienia wybierz godzinę, wpisz jego dodatni odstęp w dniach i naciśnij **Dodaj przypomnienie**. Po dodaniu przypomnień zapisz ustawienia.
2. `SettingsPageViewModel.AddReminderAsync()` sprawdza godzinę i odstęp nowego przypomnienia; `SaveAsync()` wywołuje `ReminderService.SetRemindersAsync()`.
3. Na Androidzie 13 i nowszym `AndroidReminderScheduler.CheckPermissionAsync()` sprawdza uprawnienie do powiadomień i w razie potrzeby o nie prosi. Jeśli uprawnienie zostanie odrzucone, poprzednie ustawienia i alarmy pozostają zachowane.
4. `ReminderService` asynchronicznie zapisuje stan włączenia i harmonogramy w lokalnym pliku JSON, anuluje poprzednio zapisane alarmy oraz planuje każdą wybraną godzinę, jeśli przypomnienia są włączone.
5. Każdy harmonogram zawiera własny odstęp i datę rozpoczęcia. Zapis niezmienionego przypomnienia zachowuje jego datę rozpoczęcia, dopóki przypomnienia są włączone. Włączenie przypomnień lub zmiana odstępu danego przypomnienia rozpoczyna jego cykl od bieżącego dnia lokalnego.

Aby zmienić odstęp istniejącego przypomnienia, usuń je, dodaj tę samą godzinę z nowym odstępem i zapisz ustawienia.

Wcześniejsze pojedyncze przypomnienie zapisane pod kluczem `daily-reminder-time` jest przekształcane w harmonogram z tą samą godziną i odstępem jednego dnia. Żadne przypomnienie nie jest włączane automatycznie.

### 2. Planowanie i dostarczenie alarmów

1. `ReminderScheduleCalculator.GetNextOccurrence()` oblicza następną lokalną datę i godzinę na podstawie daty rozpoczęcia, godziny i odstępu. Godzina, która minęła w bieżącym dniu przypomnienia, zostaje przeniesiona na następny dzień wynikający z harmonogramu.
2. `DailyReminderAlarm.Schedule()` rejestruje osobny jednorazowy alarm dla każdej godziny. Na Androidzie 6 i nowszym używa `SetAndAllowWhileIdle`, a na starszych wersjach `Set`.
3. Android dostarcza alarm do `DailyReminderReceiver`. Odbiornik sprawdza identyfikator i planuje następne wystąpienie przed sprawdzeniem pomiaru. Pominięcie dzisiejszego powiadomienia nie anuluje więc kolejnych wystąpień.
4. `BloodPressureMeasurementReminderService.ShouldShowReminderAsync()` sprawdza w repozytorium, czy wartość `MeasuredAt` któregoś pomiaru przypada od dzisiejszej lokalnej północy do jutrzejszej północy, bez niej.
5. Jeśli pomiar istnieje lub przypomnienia zostały wyłączone, powiadomienie nie jest wyświetlane. W przeciwnym razie odbiornik je wyświetla. Naciśnięcie otwiera aplikację i usuwa powiadomienie.
6. Odbiornik używa `GoAsync()` i wywołuje `Finish()` w bloku `finally`. Wyjątki są zapisywane w dzienniku Androida.

Są to alarmy niedokładne: Android może dostarczyć je później niż o wybranej godzinie, także z powodu ograniczeń baterii. Szczegóły znajdują się w [dokumentacji planowania alarmów Androida](https://developer.android.com/develop/background-work/services/alarms).

### 3. Restart i anulowanie

- Podczas uruchamiania aplikacji `MauiProgram` stosuje migracje bazy danych, a podczas tworzenia okna aplikacja asynchronicznie wywołuje `ReminderService.RestoreAsync()`. Serwis przywraca wszystkie włączone harmonogramy bez ponownego pytania o uprawnienie.
- `DailyReminderBootReceiver` również wywołuje `RestoreAsync()` po `BOOT_COMPLETED`. Zapisana data rozpoczęcia zachowuje rytm odstępu po restarcie.
- Zapis zmienionych godzin anuluje poprzednie alarmy przed zaplanowaniem nowej listy. Wyłączenie przypomnień anuluje zapisane alarmy.
- `AndroidManifest.xml` deklaruje uprawnienia `POST_NOTIFICATIONS` i `RECEIVE_BOOT_COMPLETED`.

### 4. Wskazówki do modyfikacji

- Każde przypomnienie ma osobny identyfikator używany jako kod żądania broadcastu `PendingIntent`. Poprzednie pojedyncze przypomnienie zachowuje identyfikator `7421`; nowe godziny otrzymują identyfikatory wynikające z minuty dnia.
- `DailyReminderReceiver` osobno używa identyfikatora powiadomienia `7421` w `NotificationManager.Notify`. Jest on niezależny od identyfikatorów alarmów.
- Anulowanie musi użyć tego samego odbiornika, akcji i kodu żądania co zaplanowany `PendingIntent`.
- Odbiorniki broadcastów są tworzone przez Androida. Odbiornik pobiera serwisy z `IPlatformApplication.Current.Services` i tworzy zakres dla sprawdzenia w bazie danych.
- Klasy odbiorników zachowują przestrzeń nazw `BloodPressureDiary.UI.Platforms.Android`. Zmiana tożsamości ich komponentów Androida może wpłynąć na alarmy zaplanowane już na urządzeniu.
