
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.EntityFrameworkCore;

namespace BloodPressureDiary.UI.Tests.Fixtures;

public abstract class TestBase
{
    protected IFixture Fixture { get; }

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

    protected T Create<T>()
    {
        return Fixture.Create<T>();
    }

    protected T Freeze<T>()
        where T : class
    {
        return Fixture.Freeze<T>();
    }

    protected DbContextOptions<TContext> CreateInMemoryOptions<TContext>()
        where TContext : DbContext
    {
        return new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .EnableDetailedErrors()
            .Options;
    }
}
