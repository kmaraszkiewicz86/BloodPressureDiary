# Project Rules

All paths are relative to the repository root.

- Work in the existing project at `src/BloodPressureDiary/BloodPressureDiary.UI`. Keep one production .NET MAUI project without DDD.
- When a block contains two or more statements, place each statement on a separate line and format the block across multiple lines.
- Keep the entire application UI in `src/BloodPressureDiary/BloodPressureDiary.UI/MainPage.xaml`. Do not add other pages or view XAML files; preserve MAUI infrastructure files such as `App.xaml`.
- Place ViewModels in `ViewModels`, services in `Services`, interfaces in `Interfaces`, and EF Core code in `Database`.
- Use constructor dependency injection and service interfaces. Register dependencies in `MauiProgram`.
- Use `CommunityToolkit.Mvvm` and `ObservableObject`. Write explicit `get`/`set` properties with `SetProperty` and instantiate commands explicitly, without MVVM source-generator attributes.
- Keep application logic in ViewModels and services. Limit code-behind to view handling; hide platform APIs behind interfaces.
- Use EF Core with SQLite, Fluent API configuration, and migrations in the application. Inject options into `DbContext` through its constructor.
- The application MUST provide complete Polish and English translations: English strings in `Resources/Localization/AppResources.resx` and Polish strings in `AppResources.pl.resx`. All UI text, validation messages, dialogs, and notifications must use these resources; both files must contain complete, actual translations.
- Write code and comments in English. Follow Microsoft naming conventions, enable nullable reference types, and use async/await.
- Add English XML documentation comments to handwritten public types and members.
- Do not use XML `<inheritdoc />` comments. Write explicit XML documentation in both interfaces and implementing classes, repeating the descriptions where appropriate, and document overrides directly.
- Place all ViewModel and service tests in `src/BloodPressureDiary/BloodPressureDiary.UI.Tests`.
- Use xUnit, AutoFixture, AutoFixture.AutoNSubstitute, and NSubstitute. Mock dependencies through interfaces.
- Classes in `Fixtures` must inherit from `TestBase` and contain configuration and creation of the SUT, test data, and mocks. Keep shared AutoFixture (`IFixture`) and `AutoNSubstituteCustomization` configuration in `TestBase`.
- Place `TestBase` and xUnit fixture classes in the `BloodPressureDiary.UI.Tests.Fixtures` namespace. Use the exact `TestBase` implementation supplied in the task prompt as the base class for fixtures.
- Test classes must implement `IClassFixture<TFixture>` and receive the fixture through their constructors. Fixtures must create fresh state and mocks for each test, even though xUnit shares the fixture instance.
- Follow the AAA structure: `Arrange`, `Act`, `Assert`. Each test must verify one behavior or responsibility; multiple assertions are allowed when they concern the same behavior.
- Use EF Core InMemory for database operation tests. Isolate the database for each test; do not mock `DbSet`.
- Keep the solution simple. Run available tests and builds, and report actual verification results.

## README.md

Keep the documentation in `README.md` up to date as the project changes. Follow these requirements without changing them:

1. A short description of the application and the technologies added.
2. Section 2: only state what the view class is and
3. For classes, services, and repositories, if present, add the method descriptions from the .NET XML comments. Transfer the XML comments without additional information or other content. This should be technical documentation only.
