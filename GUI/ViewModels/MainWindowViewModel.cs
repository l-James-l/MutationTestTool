namespace GUI.ViewModels;

public class MainWindowViewModel
{
    public DashBoardViewModel DashBoardViewModel { get; }
    public SolutionExplorerViewModel SolutionExplorerViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    public MainWindowViewModel()
    {
        DashBoardViewModel = new DashBoardViewModel();
        SolutionExplorerViewModel = new SolutionExplorerViewModel();
        SettingsViewModel = new SettingsViewModel();
    }
}
