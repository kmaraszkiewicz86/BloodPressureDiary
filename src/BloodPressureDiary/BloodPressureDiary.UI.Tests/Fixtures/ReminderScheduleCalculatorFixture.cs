using BloodPressureDiary.UI.Models;
using BloodPressureDiary.UI.Services;

namespace BloodPressureDiary.UI.Tests.Fixtures;

/// <summary>Creates a reminder schedule calculator and test schedules.</summary>
public sealed class ReminderScheduleCalculatorFixture : TestBase
{
    /// <summary>Gets the schedule calculator under test.</summary>
    public ReminderScheduleCalculator Sut { get; private set; } = null!;

    /// <summary>Creates a fresh calculator.</summary>
    public void Initialize() => Sut = Create<ReminderScheduleCalculator>();

    /// <summary>Creates a reminder schedule for a fixed start date.</summary>
    public ReminderSchedule CreateReminder(int intervalDays) => new()
    {
        Id = 10000, Time = new TimeSpan(8, 0, 0), IntervalDays = intervalDays, StartDate = new DateTime(2026, 1, 1)
    };
}
