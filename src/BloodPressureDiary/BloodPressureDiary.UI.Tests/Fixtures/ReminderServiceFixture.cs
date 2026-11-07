using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Models;
using BloodPressureDiary.UI.Services;
using Microsoft.Maui.Storage;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.Fixtures;

/// <summary>Creates fresh preferences, storage, scheduler, and reminder service for each test.</summary>
public sealed class ReminderServiceFixture : TestBase, IDisposable
{
    private readonly List<string> _directories = [];
    private TestState _state;

    /// <summary>Creates the initial test state.</summary>
    public ReminderServiceFixture()
    {
        _state = CreateState();
    }

    /// <summary>Gets the preferences substitute.</summary>
    public IPreferences Preferences => _state.Preferences;
    /// <summary>Gets the scheduler substitute.</summary>
    public IReminderScheduler Scheduler => _state.Scheduler;
    /// <summary>Gets the storage used by the test.</summary>
    public IReminderSettingsStorage Storage => _state.Storage;
    /// <summary>Gets the pending settings read.</summary>
    public TaskCompletionSource<ReminderSettings> PendingRead => _state.PendingRead;
    /// <summary>Gets the reminder service with a pending settings read.</summary>
    public ReminderService PendingSut => _state.PendingSut;
    /// <summary>Gets the reminder service under test.</summary>
    public ReminderService Sut => _state.Sut;

    /// <summary>Creates fresh dependencies and a reminder service.</summary>
    public void Initialize() => _state = CreateState();

    /// <summary>Creates a reminder with the specified identifier and hour.</summary>
    public ReminderSchedule CreateReminder(int id, int hour) => new()
    {
        Id = id, Time = new TimeSpan(hour, 0, 0), IntervalDays = 1, StartDate = DateTime.Today
    };

    /// <summary>Deletes the temporary directories used by the tests.</summary>
    public void Dispose()
    {
        foreach (var directory in _directories)
        {
            Directory.Delete(directory, true);
        }
    }

    private TestState CreateState()
    {
        var path = Path.Combine(Path.GetTempPath(), "blood-pressure-reminders-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        _directories.Add(path);
        return new TestState(path);
    }

    private sealed class TestState : TestBase
    {
        private readonly Dictionary<string, object> _values = [];

        public IPreferences Preferences { get; }
        public IReminderScheduler Scheduler { get; }
        public IReminderSettingsStorage Storage { get; }
        public ReminderService Sut { get; }
        public TaskCompletionSource<ReminderSettings> PendingRead { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public ReminderService PendingSut { get; }

        public TestState(string directory)
        {
            Preferences = Freeze<IPreferences>();
            Scheduler = Freeze<IReminderScheduler>();
            var fileSystem = Freeze<IFileSystem>();
            fileSystem.AppDataDirectory.Returns(directory);
            Preferences.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>())
                .Returns(call => _values.TryGetValue(call.ArgAt<string>(0), out var value) ? (string)value : call.ArgAt<string>(1));
            Preferences.Get(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string?>())
                .Returns(call => _values.TryGetValue(call.ArgAt<string>(0), out var value) ? (bool)value : call.ArgAt<bool>(1));
            Preferences.When(preferences => preferences.Set(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>()))
                .Do(call => _values[call.ArgAt<string>(0)] = call.ArgAt<string>(1));
            Preferences.When(preferences => preferences.Set(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string?>()))
                .Do(call => _values[call.ArgAt<string>(0)] = call.ArgAt<bool>(1));
            Scheduler.CheckPermissionAsync(Arg.Any<bool>()).Returns(true);
            Storage = new ReminderSettingsStorage(fileSystem, Preferences);
            Sut = new ReminderService(Scheduler, Storage);
            var pendingStorage = Substitute.For<IReminderSettingsStorage>();
            pendingStorage.GetAsync().Returns(PendingRead.Task);
            PendingSut = new ReminderService(Scheduler, pendingStorage);
        }
    }
}
