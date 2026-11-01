using BloodPressureDiary.UI.Database;
using BloodPressureDiary.UI.Tests.Fixtures;
using NSubstitute;
namespace BloodPressureDiary.UI.Tests;
/// <summary>Verifies diary interactions.</summary>
public sealed class MainViewModelTests : IClassFixture<DiaryFixture>
{
    private readonly DiaryFixture fixture;
    /// <summary>Initializes the test fixture.</summary>
    public MainViewModelTests(DiaryFixture fixture) => this.fixture = fixture;
    [Fact]
    /// <summary>Verifies SavePersistsOptionalFieldsAndDate.</summary>
    public async Task SavePersistsOptionalFieldsAndDate()
    {
        // Arrange
        var state = fixture.CreateMain();
        var input = state.Sut.NewMeasurement;
        input.Systolic = "125";
        input.Diastolic = "85";
        input.Pulse = "65";
        input.Note = "note";
        input.Date = new DateTime(2026, 10, 1);
        input.Time = TimeSpan.FromHours(10);
        // Act
        await state.Sut.SaveCommand.ExecuteAsync(null);
        // Assert
        await state.Store.Received().SaveAsync(Arg.Is<BloodPressureMeasurement>(measurement =>
            measurement.Systolic == 125 &&
            measurement.Diastolic == 85 &&
            measurement.Pulse == 65 &&
            measurement.Note == "note" &&
            measurement.CreatedDate == new DateTime(2026, 10, 1, 10, 0, 0)));
    }
    [Fact]
    /// <summary>Verifies InvalidInputDoesNotSave.</summary>
    public async Task InvalidInputDoesNotSave()
    {
        // Arrange
        var state = fixture.CreateMain();
        // Act
        await state.Sut.SaveCommand.ExecuteAsync(null);
        // Assert
        await state.Store.DidNotReceive().SaveAsync(Arg.Any<BloodPressureMeasurement>());
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    /// <summary>Verifies DeleteRequiresConfirmation.</summary>
    public async Task DeleteRequiresConfirmation(bool confirmed)
    {
        // Arrange
        var state = fixture.CreateMain();
        await state.Sut.LoadCommand.ExecuteAsync(null);
        state.Dialogs.ConfirmDeleteAsync().Returns(confirmed);
        // Act
        await state.Sut.DeleteCommand.ExecuteAsync(state.Sut.Rows[0]);
        // Assert
        await state.Store.Received(confirmed ? 1 : 0).DeleteAsync(1);
    }
    [Fact]
    /// <summary>Verifies CancelRestoresOriginalValues.</summary>
    public async Task CancelRestoresOriginalValues()
    {
        // Arrange
        var state = fixture.CreateMain();
        await state.Sut.LoadCommand.ExecuteAsync(null);
        var row = state.Sut.Rows[0];
        state.Sut.EditCommand.Execute(row);
        row.Note = "changed";
        row.Systolic = "invalid";
        // Act
        state.Sut.CancelCommand.Execute(row);
        // Assert
        Assert.Equal("original", row.Note);
        Assert.Equal("120", row.Systolic);
        Assert.False(row.IsEditing);
    }
    [Fact]
    /// <summary>Verifies InlineSaveUpdatesExistingIdentifier.</summary>
    public async Task InlineSaveUpdatesExistingIdentifier()
    {
        // Arrange
        var state = fixture.CreateMain();
        await state.Sut.LoadCommand.ExecuteAsync(null);
        var row = state.Sut.Rows[0];
        state.Sut.EditCommand.Execute(row);
        row.Systolic = "130";
        // Act
        await state.Sut.SaveRowCommand.ExecuteAsync(row);
        // Assert
        await state.Store.Received().SaveAsync(Arg.Is<BloodPressureMeasurement>(m => m.Id == 1 && m.Systolic == 130));
    }
    [Fact]
    /// <summary>Verifies StorageFailureIsReported.</summary>
    public async Task StorageFailureIsReported()
    {
        // Arrange
        var state = fixture.CreateMain();
        state.Store.LoadAsync().Returns(Task.FromException<IReadOnlyList<BloodPressureMeasurement>>(new IOException()));
        // Act
        await state.Sut.LoadCommand.ExecuteAsync(null);
        // Assert
        await state.Dialogs.Received().ShowErrorAsync(Arg.Any<string>());
        Assert.False(state.Sut.IsBusy);
    }
    [Fact]
    /// <summary>Verifies SettingsUsesNavigationInterface.</summary>
    public async Task SettingsUsesNavigationInterface()
    {
        // Arrange
        var state = fixture.CreateMain();
        // Act
        await state.Sut.SettingsCommand.ExecuteAsync(null);
        // Assert
        await state.Navigation.Received().ShowSettingsAsync();
    }
}
