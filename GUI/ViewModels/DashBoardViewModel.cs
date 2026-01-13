using GUI.ViewModels.ElementViewModels;

namespace GUI.ViewModels;

public class DashBoardViewModel
{
    public StatusBarViewModel StatusBarViewModel { get; set; }

    public DashBoardViewModel()
    {
        StatusBarViewModel = new StatusBarViewModel();
    }
}
