using GUI.Services;
using Models.Enums;
using Models.Events;
using Models.SharedInterfaces;

namespace GUI.ViewModels.DashBoardElements;

public class StatusBarViewModel : ViewModelBase
{
    private readonly IStatusTracker _statusTracker;
    private readonly IDarwingDialogService _dialogService;

    public StatusBarViewModel(IStatusTracker statusTracker, IEventAggregator eventAggregator, IDarwingDialogService dialogService)
    {
        _statusTracker = statusTracker;
        _dialogService = dialogService;
        eventAggregator.GetEvent<DarwingOperationStatesChangedEvent>().Subscribe(_ => OnOperationStatesChanged(), ThreadOption.UIThread);
    }

    private void OnOperationStatesChanged()
    {
        LoadSolutionState = CheckStatus(DarwingOperation.LoadSolution);
        BuildOperationState = CheckStatus(DarwingOperation.BuildSolution);
        InitialTestRunState = CheckStatus(DarwingOperation.TestUnmutatedSolution);
        MutantDiscoveryState = CheckStatus(DarwingOperation.DiscoveringMutants);
        BuildingMutatedSolutionState = CheckStatus(DarwingOperation.BuildingMutatedSolution);
        TestingMutantsState = CheckStatus(DarwingOperation.TestMutants);
        UpdateCompletionPercentage();

        if (TestingMutantsState is OperationStates.Succeeded)
        {
            _dialogService.InfoDialog("Testing Completed!", "Testing Complete");
        }
    }

    private OperationStates CheckStatus(DarwingOperation operation)
    {
        OperationStates state = _statusTracker.CheckStatus(operation);
        if (state is OperationStates.Failed)
        {
            _dialogService.ErrorDialog("Error occurred",
                $"While performing stage: {operation.ToReadableString()}, an error occurred and testing cannot continue. Check the console for details");
        }
        return state;
    }

    public float ProgressBarPercentage
    {
        get; 
        set => SetProperty(ref field, value);
    }

    private void UpdateCompletionPercentage()
    {
        const float eachOperationPercentage = 0.2f; // Since there are 5 operations contributing to the progress bar
        float percentage = 0.0f;
        if (LoadSolutionState is OperationStates.Succeeded)
        {
            percentage += eachOperationPercentage;
        }
        if (BuildOperationState is OperationStates.Succeeded)
        {
            // We give half weight to the build operation to make it clear that continuing is a manual operation
            percentage += eachOperationPercentage/2;
        }
        if (InitialTestRunState is not OperationStates.NotStarted)
        {
            // User has continued so now complete the building section.
            percentage += eachOperationPercentage / 2;
        }
        if (InitialTestRunState is OperationStates.Succeeded)
        {
            percentage += eachOperationPercentage;
        }
        if (MutantDiscoveryState is OperationStates.Succeeded)
        {
            percentage += eachOperationPercentage;
        }
        if (BuildingMutatedSolutionState is OperationStates.Succeeded)
        {
            percentage += eachOperationPercentage;
        }

        // We ignore the last operation (TestingMutantsState) for the progress bar percentage
        // because the bar doesn't extend past that operation, so once were at that point, the bar is full.
        ProgressBarPercentage = percentage;
    }

    public OperationStates LoadSolutionState
    {
        get; 
        set => SetProperty(ref field, value);
    }

    public OperationStates BuildOperationState
    {
        get; 
        set => SetProperty(ref field, value);
    }

    public OperationStates InitialTestRunState
    {
        get; 
        set => SetProperty(ref field, value);
    }

    public OperationStates MutantDiscoveryState
    {
        get; 
        set => SetProperty(ref field, value);
    }


    public OperationStates BuildingMutatedSolutionState
    {
        get; 
        set => SetProperty(ref field, value);
    }


    public OperationStates TestingMutantsState
    {
        get; 
        set => SetProperty(ref field, value);
    }
}
