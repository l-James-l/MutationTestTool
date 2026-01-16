using Models;
using Models.Events;

namespace Core;

/// <summary>
/// Will track the overall status of the mutation testing process.
/// This class will subscribe to various events and update the status accordingly.
/// Used by the UI to display current status, and by other classes to check if certain actions can be performed.
/// </summary>
public class StatusTracker : IStartUpProcess
{
    private readonly IEventAggregator _eventAggregator;

    public StatusTracker(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    public void StartUp()
    {
        // These subscriptions track the overall status of the mutation testing process.
        _eventAggregator.GetEvent<SolutionPathProvidedEvent>().BackGroundSubscribe(OnSolutionPathProvided);
        _eventAggregator.GetEvent<SolutionLoadedEvent>().BackGroundSubscribe(OnSolutionBuildRequested);
        _eventAggregator.GetEvent<SolutionBuildCompletedEvent>().BackGroundSubscribe(OnSolutionBuildCompleted);
        _eventAggregator.GetEvent<InitiateTestRunEvent>().BackGroundSubscribe(OnUnmutatedTestRunStarted);
        _eventAggregator.GetEvent<InitialTestRunCompleteEvent>().BackGroundSubscribe(OnUnmutatedRunComplete);
        _eventAggregator.GetEvent<MutantDiscoveryCompleteEvent>().BackGroundSubscribe(OnMutantDiscoveryComplete);
        _eventAggregator.GetEvent<TestMutatedSolutionEvent>().BackGroundSubscribe(OnMutatedSolutionBuilt);
        _eventAggregator.GetEvent<MutatedSolutionTestingCompleteEvent>().BackGroundSubscribe(OnMutationTestingComplete);
    }

    private void OnSolutionPathProvided(SolutionPathProvidedPayload payload)
    {
        throw new NotImplementedException();
    }

    private void OnSolutionBuildRequested(bool obj)
    {
        throw new NotImplementedException();
    }

    private void OnSolutionBuildCompleted(bool obj)
    {
        throw new NotImplementedException();
    }

    private void OnUnmutatedTestRunStarted()
    {
        throw new NotImplementedException();
    }

    private void OnUnmutatedRunComplete(InitialTestRunInfo info)
    {
        throw new NotImplementedException();
    }

    private void OnMutantDiscoveryComplete(bool success)
    {
        throw new NotImplementedException();
    }

    private void OnMutatedSolutionBuilt(bool success)
    {
        throw new NotImplementedException();
    }

    private void OnMutationTestingComplete()
    {
        throw new NotImplementedException();
    }
}
