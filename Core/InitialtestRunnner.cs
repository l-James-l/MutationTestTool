using Core.IndustrialEstate;
using Core.Interfaces;
using Models;
using Models.Events;
using Mutator;
using Serilog;
using System.Diagnostics;

namespace Core;

public class InitialTestRunnner : IStartUpProcess
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IMutationSettings _mutationSettings;
    private readonly IWasBuildSuccessfull _wasBuildSuccessfull;
    private readonly IProcessWrapperFactory _processFactory;
    private IMutationRunInitiator _mutationRunManager;

    public InitialTestRunnner(IEventAggregator eventAggregator, IMutationSettings mutationSettings, IWasBuildSuccessfull wasBuildSuccessfull,
        IProcessWrapperFactory processFactory, IMutationRunInitiator mutationRunManager)
    {
        _eventAggregator = eventAggregator;
        _mutationSettings = mutationSettings;
        _wasBuildSuccessfull = wasBuildSuccessfull;
        _processFactory = processFactory;
        _mutationRunManager = mutationRunManager;
    }

    public void StartUp()
    {
        _eventAggregator.GetEvent<InitiateTestRunEvent>().Subscribe(InitialTestRun, ThreadOption.BackgroundThread, keepSubscriberReferenceAlive: true);
    }
    
    /// <summary>
    /// When a mutaiotn test run is started, the first step is running all unit test to ensure they all pass
    /// </summary>
    private void InitialTestRun()
    {
        // TODO replace with status manager check
        if (!_wasBuildSuccessfull.WasLastBuildSuccessful)
        {
            Log.Error("Attempted to start a mutation run without a successful build");
            return;
        }

        Log.Information("Starting initial test run before mutation begins.");

        InitialTestRunInfo testRunInfo = new();
        try
        {
            PerformTestRun(testRunInfo);
        }
        finally
        {
            // Ensure event is published even if the test run failed, and before proceeding to mutation run.
            _eventAggregator.GetEvent<InitialTestRunCompleteEvent>().Publish(testRunInfo);
            if (testRunInfo.WasSuccesful)
            {
                _mutationRunManager.Run(testRunInfo);
            }
        }
    }

    private void PerformTestRun(InitialTestRunInfo testRunInfo)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "dotnet",
            Arguments = $"test {Path.GetFileName(_mutationSettings.SolutionPath)} --no-build",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            WorkingDirectory = Path.GetDirectoryName(_mutationSettings.SolutionPath)
        };
        IProcessWrapper testRun = _processFactory.Create(startInfo);

        int? timeout = _mutationSettings.SolutionProfileData?.GeneralSettings.TestRunTimeout;
        bool processSuccess = testRun.StartAndAwait(timeout);

        foreach (string output in testRun.Output)
        {
            Log.Information(output);
        }

        if (!processSuccess || !testRun.Success)
        {
            Log.Error("Initial test run without mutations has failures. Cannot perform mutation testing.");
        }
        else
        {
            Log.Information("Initial test run successful, starting mutant discovery.");
            testRunInfo.WasSuccesful = true;
            testRunInfo.InitialRunDuration = testRun.Duration;
        }
    }
}
