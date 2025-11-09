using CoreTests.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Startup;

public class DependencyRegistrar
{
    private readonly IServiceCollection _services;
    private readonly ServiceProvider _serviceProvider;

    public DependencyRegistrar(IServiceCollection serviceCollection)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection);

        _services = serviceCollection;

        RegisterDependencies();

        _serviceProvider = _services.BuildServiceProvider();

        _serviceProvider.GetService<EstablishLoggerConfiguration>();
    }

    private void RegisterDependencies()
    {
        _services.AddSingleton<IEventAggregator, EventAggregator>();
        _services.AddSingleton<EstablishLoggerConfiguration>();
        _services.AddSingleton<ISolutionPathProvidedAwaiter, SolutionPathProvidedAwaiter>();
    }
}
