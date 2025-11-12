using Core.Interfaces;
using Microsoft.CodeAnalysis;
using Models;
using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Core;

public class ProjectBuilder : IProjectBuilder
{
    private Regex _errorOutPutRegex = new Regex(@"error (\S)*: ", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public bool InitialBuild(SolutionContainer solution)
    {
        // The build process will have exit code 1 if the build failed.
        Log.Information("Performing initial build");
        List<Project> failedBuilds = new();

        TryBuildAllProjects(solution, failedBuilds);
        
        if (failedBuilds.Count > 0)
        {
            return RetryFailedProjectBuilds(failedBuilds);
        }

        return true;
    }

    private bool RetryFailedProjectBuilds(List<Project> failedBuilds)
    {
        foreach (Project projToRety in failedBuilds)
        {
            //Clean the project
            Process cleaningProcess = GenerateCleanProcess(projToRety.FilePath);
            cleaningProcess.Start();
            cleaningProcess.WaitForExit();

            //Restore project dependencies
            Process restoreProcess = GenerateRestoreProcess(projToRety.FilePath);
            restoreProcess.Start();
            restoreProcess.WaitForExit();

            //Retry the build
            Process buildRetyProcess = GenerateBuildProcess(projToRety.FilePath);
            buildRetyProcess.Start();
            buildRetyProcess.WaitForExit();

            if (buildRetyProcess.ExitCode == 0)
            {
                Log.Information($"Retrying build for {projToRety.Name} was successful");
                continue;
            }

            // Second attempt at building the project failed. Return out, dont bother retrying any other failed projects.
            return false;
        }

        return true;
    }

    private void TryBuildAllProjects(SolutionContainer solution, List<Project> failedBuilds)
    {
        foreach (Project proj in solution.AllProjects)
        {
            Log.Information("Building project: " + proj.Name);

            var buildingProcess = GenerateBuildProcess(proj.FilePath);
            buildingProcess.Start();
            buildingProcess.WaitForExit();

            if (buildingProcess.ExitCode == 0)
            {
                Log.Information($"{proj.Name} build succesful");
                continue;
            }
            Log.Information($"{proj.Name} build failed");
            failedBuilds.Add(proj);

            if (buildingProcess.StandardError.ReadToEnd() is string errorsOutput)
            {
                Log.Information($"Errors encountered while trying to build {proj.Name}. Will clean and retry");
                Log.Debug(errorsOutput);
            }

            while (buildingProcess.StandardOutput.ReadLine() is { } output)
            {
                if (_errorOutPutRegex.IsMatch(output))
                {
                    Log.Information(output);
                }
                else
                {
                    Log.Debug(output);
                }
            }
        }
    }

    private Process GenerateBuildProcess(string? path)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build {Path.GetFileName(path)}",
                RedirectStandardError = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(path)
            }
        };
    }

    private Process GenerateCleanProcess(string? path)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"clean {Path.GetFileName(path)}",
                RedirectStandardError = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(path)
            }
        };
    }

    private Process GenerateRestoreProcess(string? path)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"restore {Path.GetFileName(path)}",
                RedirectStandardError = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(path)
            }
        };
    }
}
