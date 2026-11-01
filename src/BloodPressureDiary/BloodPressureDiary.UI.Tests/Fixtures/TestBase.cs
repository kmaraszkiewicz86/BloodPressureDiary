
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.EntityFrameworkCore;

namespace BloodPressureDiary.UI.Tests.Fixtures;

/// <summary>Provides shared AutoFixture and isolated database configuration.</summary>
public abstract class TestBase
{
    /// <summary>Gets the shared object creation fixture.</summary>
    protected IFixture Fixture { get; }

    /// <summary>Configures AutoFixture and interface substitutes.</summary>
    protected TestBase()
    {
        Fixture = new Fixture();

        Fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });

        Fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(behavior => Fixture.Behaviors.Remove(behavior));

        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    /// <summary>Creates test data or a substitute.</summary>
    protected T Create<T>()
    {
        return Fixture.Create<T>();
    }

    /// <summary>Freezes a substitute in the fixture.</summary>
    protected T Freeze<T>()
        where T : class
    {
        return Fixture.Freeze<T>();
    }

    /// <summary>Creates database options isolated to one test.</summary>
    protected DbContextOptions<TContext> CreateInMemoryOptions<TContext>()
        where TContext : DbContext
    {
        return new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .EnableDetailedErrors()
            .Options;
    }
}

