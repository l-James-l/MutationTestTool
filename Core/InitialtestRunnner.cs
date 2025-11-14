using Core.Interfaces;
using Core.Startup;
using Models;
using Models.Events;
using Serilog;
using System.Diagnostics;
using System.IO;

namespace Core;

public class InitialtestRunnner : IStartUpProcess
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IMutationSettings _mutationSettings;
    private readonly IWasBuildSuccessfull _wasBuildSuccessfull;

    public InitialtestRunnner(IEventAggregator eventAggregator, IMutationSettings mutationSettings, IWasBuildSuccessfull wasBuildSuccessfull)
    {
        _eventAggregator = eventAggregator;
        _mutationSettings = mutationSettings;
        _wasBuildSuccessfull = wasBuildSuccessfull;
    }

    public void StartUp()
    {
        _eventAggregator.GetEvent<InitiateTestRunEvent>().Subscribe(InitialTestRun);
    }
    
    /// <summary>
    /// When a mutaiotn test run is started, the first step is running all unit test to ensure they all pass
    /// </summary>
    private void InitialTestRun()
    {
        //By checking we have a succesful build, we implicitly know that
        if (!_wasBuildSuccessfull.WasLastBuildSuccessful)
        {
            Log.Error("Attempted to start a mutation run without a successful build");
            return;
        }

        Process testRun = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"test {Path.GetFileName(_mutationSettings.SolutionPath)} --no-build",
                RedirectStandardError = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(_mutationSettings.SolutionPath)
            }
        };

        testRun.Start();
        testRun.WaitForExit();

        while (testRun.StandardOutput.ReadLine() is {} output)
        {
            Log.Information(output);
        }

        if (testRun.ExitCode != 0)
        {
            Log.Error("Initial test run wihout mutations has failures. Cannot perform mutation testing.");
        }
    }
}
