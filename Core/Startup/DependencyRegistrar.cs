using Microsoft.Extensions.DependencyInjection;

namespace Core.Startup;

public class DependencyRegistrar: IDependencyRegistrar
{
    private IServiceCollection _services;

    public DependencyRegistrar(IServiceCollection services)
    {
        _services = services;

        RegisterDependencies();
    }

    private void RegisterDependencies()
    {
        _services.AddSingleton<IEventAggregator, EventAggregator>();
        _services.AddSingleton<ISolutionPathProvidedAwaiter, SolutionPathProvidedAwaiter>();
    }
}
