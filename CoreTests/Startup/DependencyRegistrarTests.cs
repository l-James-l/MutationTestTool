using Core;
using Core.Startup;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CoreTests.Startup;

internal class DependencyRegistrarTests
{
    private IServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = Substitute.For<IServiceCollection>();
    }

    [Test]
    public void GivenConstructed_ThenAllDependenciesRegistered()
    {
        //Arrange
        //Act
        DependencyRegistrar registrar = new DependencyRegistrar(_services);

        //Assert
        AssertRegistration<ISolutionPathProvidedAwaiter, SolutionPathProvidedAwaiter>();
        AssertRegistration<IEventAggregator, EventAggregator>();
    }

    private void AssertRegistration<T1, T2>(bool isSingleton = true)
    {
        _services.Received(1).Add(Arg.Is<ServiceDescriptor>(x => 
        x.Lifetime == (isSingleton ? ServiceLifetime.Singleton : ServiceLifetime.Transient)
        && x.ImplementationType == typeof(SolutionPathProvidedAwaiter)
        && x.ServiceType == typeof(ISolutionPathProvidedAwaiter)));
    }
}
