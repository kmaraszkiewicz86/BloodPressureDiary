using BloodPressureDiary.UI.Tests.Fixtures;

namespace BloodPressureDiary.UI.Tests.Services;

/// <summary>Tests recurring reminder date calculation.</summary>
public sealed class ReminderScheduleCalculatorTests(ReminderScheduleCalculatorFixture fixture) : IClassFixture<ReminderScheduleCalculatorFixture>
{
    /// <summary>Calculates the next occurrence using the saved start date and interval.</summary>
    [Theory]
    [InlineData(1, 1, 7, 1)]
    [InlineData(1, 1, 8, 2)]
    [InlineData(2, 1, 9, 3)]
    [InlineData(2, 2, 7, 3)]
    [InlineData(2, 3, 7, 3)]
    [InlineData(2, 3, 9, 5)]
    [InlineData(3, 2, 9, 4)]
    public void GetNextOccurrence_UsesSavedStartDate(int intervalDays, int day, int hour, int expectedDay)
    {
        // Arrange
        fixture.Initialize();
        var reminder = fixture.CreateReminder(intervalDays);
        var now = new DateTime(2026, 1, day, hour, 0, 0);

        // Act
        var next = fixture.Sut.GetNextOccurrence(reminder, now);

        // Assert
        Assert.Equal(new DateTime(2026, 1, expectedDay, 8, 0, 0), next);
    }
}
