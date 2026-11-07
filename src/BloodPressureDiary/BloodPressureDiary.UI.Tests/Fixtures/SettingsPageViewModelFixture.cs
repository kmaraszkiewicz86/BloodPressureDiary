using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Models;
using BloodPressureDiary.UI.ViewModels;
using NSubstitute;

namespace BloodPressureDiary.UI.Tests.Fixtures;

/// <summary>Creates fresh reminder and interaction substitutes for each settings ViewModel test.</summary>
public sealed class SettingsPageViewModelFixture : TestBase
{
    private TestState _state = new(false, TimeSpan.Zero);

    /// <summary>Gets the reminder service substitute.</summary>
    public IReminderService ReminderService => _state.ReminderService;
    /// <summary>Gets the user interaction substitute.</summary>
    public IUserInteractionService Interaction => _state.Interaction;
    /// <summary>Gets the navigation substitute.</summary>
    public INavigationService NavigationService => _state.NavigationService;
    /// <summary>Gets the settings ViewModel system under test.</summary>
    public SettingsPageViewModel Sut => _state.Sut;

    /// <summary>Creates fresh substitutes and a settings ViewModel.</summary>
    public void Reset(bool reminderEnabled, TimeSpan reminderTime) => _state = new TestState(reminderEnabled, reminderTime);

    private sealed class TestState : TestBase
    {
        /// <summary>Gets the reminder service substitute.</summary>
        public IReminderService ReminderService { get; }
        /// <summary>Gets the user interaction substitute.</summary>
        public IUserInteractionService Interaction { get; }
        /// <summary>Gets the navigation substitute.</summary>
        public INavigationService NavigationService { get; }
        /// <summary>Gets the settings ViewModel system under test.</summary>
        public SettingsPageViewModel Sut { get; }

        /// <summary>Creates fresh substitutes and a settings ViewModel with the supplied reminder state.</summary>
        public TestState(bool reminderEnabled, TimeSpan reminderTime)
        {
            ReminderService = Freeze<IReminderService>();
            ReminderService.IsEnabledAsync().Returns(reminderEnabled);
            ReminderService.GetRemindersAsync().Returns(new[] { new ReminderSchedule { Id = 10001, Time = reminderTime, IntervalDays = 1 } });
            Interaction = Freeze<IUserInteractionService>();
            NavigationService = Freeze<INavigationService>();
            Sut = Create<SettingsPageViewModel>();
            Sut.ReminderEnabled = reminderEnabled;
            Sut.ReminderTime = reminderTime;
            Sut.IntervalDaysInput = "1";
        }
    }
}
