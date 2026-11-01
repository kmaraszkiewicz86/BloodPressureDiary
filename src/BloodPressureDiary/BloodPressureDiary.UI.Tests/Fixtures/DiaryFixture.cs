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
    public (MainViewModel Sut, IMeasurementService Store, IDialogService Dialogs, INavigationService Navigation) CreateMain()
    {
        var store = Create<IMeasurementService>();
        var dialogs = Create<IDialogService>();
        var navigation = Create<INavigationService>();
        store.LoadAsync().Returns(Task.FromResult<IReadOnlyList<Measurement>>(new[] { new Measurement { Id = 1, RecordedAt = DateTime.Today, Systolic = 120, Diastolic = 80, Note = "original" } }));
        return (new MainViewModel(store, dialogs, navigation), store, dialogs, navigation);
    }
    /// <summary>Creates a fresh settings view model and mocks.</summary>
    public (SettingsViewModel Sut, ISettingsService Settings, IReminderService Reminders, IDialogService Dialogs, INavigationService Navigation) CreateSettings()
    {
        var settings = Create<ISettingsService>(); var reminders = Create<IReminderService>();
        var dialogs = Create<IDialogService>(); var navigation = Create<INavigationService>();
        return (new SettingsViewModel(settings, reminders, dialogs, navigation), settings, reminders, dialogs, navigation);
    }
    /// <summary>Creates an isolated in-memory measurement service.</summary>
    public MeasurementService CreateStore()
    {
        var options = CreateInMemoryOptions<DiaryContext>();
        var factory = Create<IDbContextFactory<DiaryContext>>();
        factory.CreateDbContextAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(new DiaryContext(options)));
        return new MeasurementService(factory);
    }
    /// <summary>Creates a valid new measurement row.</summary>
    public MeasurementRowViewModel CreateRow() => new() { Systolic = "120", Diastolic = "80", Date = DateTime.Today, Time = TimeSpan.FromHours(9) };


}
