using AutoFixture;
using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Interfaces;
using BloodPressureDiary.UI.Services;
using BloodPressureDiary.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
namespace BloodPressureDiary.UI.Tests.Fixtures;
/// <summary>Creates isolated diary test state.</summary>
public sealed class DiaryFixture : TestBase
{
    /// <summary>Creates a fresh view model and mocks for one test.</summary>
    public (MainViewModel Sut, IBloodPressureMeasurementService Store, IDialogService Dialogs, INavigationService Navigation) CreateMain()
    {
        var store = Create<IBloodPressureMeasurementService>();
        var dialogs = Create<IDialogService>();
        var navigation = Create<INavigationService>();
        var measurement = new BloodPressureMeasurement
        {
            Id = 1,
            CreatedDate = DateTime.Today,
            Systolic = 120,
            Diastolic = 80,
            Note = "original"
        };
        store.LoadAsync().Returns(Task.FromResult<IReadOnlyList<BloodPressureMeasurement>>(
            new[] { measurement }));
        return (new MainViewModel(store, dialogs, navigation), store, dialogs, navigation);
    }
    /// <summary>Creates a fresh settings view model and mocks.</summary>
    public (SettingsViewModel Sut, ISettingsService Settings, IReminderService Reminders, IDialogService Dialogs, INavigationService Navigation) CreateSettings()
    {
        var settings = Create<ISettingsService>();
        var reminders = Create<IReminderService>();
        var dialogs = Create<IDialogService>();
        var navigation = Create<INavigationService>();
        return (new SettingsViewModel(settings, reminders, dialogs, navigation), settings, reminders, dialogs, navigation);
    }
    /// <summary>Creates an isolated in-memory measurement service.</summary>
    public BloodPressureMeasurementService CreateStore()
    {
        var options = CreateInMemoryOptions<DiaryContext>();
        var factory = Create<IDbContextFactory<DiaryContext>>();
        factory.CreateDbContextAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(new DiaryContext(options)));
        return new BloodPressureMeasurementService(factory);
    }
    /// <summary>Creates a valid new measurement row.</summary>
    public BloodPressureMeasurementRowViewModel CreateRow() => new()
    {
        Systolic = "120",
        Diastolic = "80",
        Date = DateTime.Today,
        Time = TimeSpan.FromHours(9)
    };
}
