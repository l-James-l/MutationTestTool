using GUI.ViewModels.DashBoardElements;

namespace GUI.ViewModels;

public class DashBoardViewModel : ViewModelBase
{
    public StatusBarViewModel StatusBarViewModel { get; set; }
    public MutationScoreByProjectViewModel MutationScoreByProjectViewModel { get; }
    public SummaryCountsViewModel SummaryCountsViewModel { get; }

    public DashBoardViewModel(StatusBarViewModel statusBarViewModel, MutationScoreByProjectViewModel mutationScoreByProjectViewModel,
        SummaryCountsViewModel summaryCountsViewModel)
    {
        StatusBarViewModel = statusBarViewModel;
        MutationScoreByProjectViewModel = mutationScoreByProjectViewModel;
        SummaryCountsViewModel = summaryCountsViewModel;
    }
}
