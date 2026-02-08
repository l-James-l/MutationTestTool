using Core.Startup;
using Microsoft.Extensions.DependencyInjection;
using GUI.ViewModels;
using GUI.Services;
using GUI.ViewModels.DashBoardElements;
using GUI.ViewModels.SolutionExplorerElements;
using GUI.ViewModels.SettingsElements;

namespace GUI;

public class GuiDependencyRegistrar : DependencyRegistrar
{
    public GuiDependencyRegistrar(IServiceCollection serviceCollection) : base(serviceCollection) { }

    protected override void RegisterLocalDependencies()
    {
        Services.AddSingleton<MainWindow>();

        RegisterViewModels();
        RegisterServices();
    }

    private void RegisterServices()
    {
        Services.AddSingleton<IFileSelectorService, FileSelectorService>();
        Services.AddSingleton<IDarwingDialogService, DialogService>();
        Services.AddSingleton<IConsoleService, ConsoleService>();
    }

    private void RegisterViewModels()
    {
        Services.AddSingleton<MainWindowViewModel>();

        //Dashboard
        Services.AddSingleton<DashBoardViewModel>();
        Services.AddSingleton<StatusBarViewModel>();
        Services.AddSingleton<MutationScoreByProjectViewModel>();
        Services.AddSingleton<SummaryCountsViewModel>();

        //Solution Explorer 
        Services.AddSingleton<SolutionExplorerViewModel>();
        Services.AddSingleton<FileExplorerViewModel>();
        
        //Settings
        Services.AddSingleton<SettingsViewModel>();
        Services.AddSingleton<ProjectTypeCollectionSettings>();
        Services.AddSingleton<GeneralSettingsViewModel>();
        Services.AddSingleton<DisabledMutationTypesViewModel>();
    }
}