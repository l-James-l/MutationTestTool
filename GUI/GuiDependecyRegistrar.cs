using Core.Startup;
using Microsoft.Extensions.DependencyInjection;
using GUI.ViewModels;

namespace GUI;

public class GuiDependecyRegistrar : DependencyRegistrar
{
    public GuiDependecyRegistrar(IServiceCollection serviceCollection) : base(serviceCollection) { }

    protected override void RegisterLocalDependencies()
    {
        Services.AddSingleton<MainWindow>();
        Services.AddSingleton<MainWindowViewModel>();
        Services.AddSingleton<DashBoardViewModel>();
    }
}