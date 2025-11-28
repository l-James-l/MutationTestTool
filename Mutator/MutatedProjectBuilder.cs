using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Models;
using Models.Events;
using Serilog;

namespace Mutator;

public class MutatedProjectBuilder : IStartUpProcess
{
    private readonly IMutationDiscoveryManager _mutationDiscovery;
    private readonly IEventAggregator _eventAggregator;

    public MutatedProjectBuilder(IMutationDiscoveryManager mutationDiscovery, IEventAggregator eventAggregator)
    {
        _mutationDiscovery = mutationDiscovery;
        _eventAggregator = eventAggregator;
    }

    public void StartUp()
    {
        _eventAggregator.GetEvent<BuildMutatedSolutionEvent>().Subscribe(EmitAllChanges);
    }

    public void EmitAllChanges(ISolutionContainer mutatedSolution)
    {
        const int maxRetrys = 5;
        
        foreach (IProjectContainer project in mutatedSolution.SolutionProjects)
        {
            int retryCount = 0;
            bool dontRetry = false;
            while (!EmitMutatedDll(project, out List<Diagnostic> failures) || !dontRetry)
            {
                bool allActioned = FindAndRemoveMutationsCausingFailures(project, failures);
                dontRetry = !allActioned || retryCount++ < maxRetrys;
            }

        }

        RestoreDependencies(mutatedSolution);
    }


    private bool EmitMutatedDll(IProjectContainer mutatedProject, out List<Diagnostic> failures)
    {
        Log.Information($"Attempting to emit dll for {mutatedProject.Name}");

        failures = [];

        Compilation? compilation = mutatedProject.GetCompilation();
        if (compilation == null)
        {
            Log.Error($"Failed to created compilation for mutated project {mutatedProject.Name}.");
            return false;
        }

        EmitResult emitResult = compilation.Emit(mutatedProject.DllFilePath);
        if (!emitResult.Success)
        {
            Log.Error("Emitting dll for {Name} failed. {Diagnostics}.", mutatedProject.Name, emitResult.Diagnostics);
            failures = emitResult.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).ToList();
            return false;
        }

        Log.Information($"Mutated dll succesfully created for {mutatedProject.Name}.");
        return true;
    }

    private void RestoreDependencies(ISolutionContainer mutatedSolution)
    {
        // For every project that has a dependency on another project that has been mutated, we need to replace the dll in that project.
        foreach (IProjectContainer project in mutatedSolution.AllProjects)
        {
            // Get the folder containg the projects build artifacts, which will include dll's of any other projects.
            string projectOutputDirectory = Path.GetDirectoryName(project.DllFilePath) ?? "";

            foreach (IProjectContainer mutatedProject in mutatedSolution.SolutionProjects.Except([project]))
            {
                // Check if the unmutated dll exists in the output directory, and if it does, replace it
                string wouldBeDependency = Path.Combine(projectOutputDirectory, Path.GetFileName(mutatedProject.DllFilePath));
                if (File.Exists(wouldBeDependency))
                {
                    Log.Debug($"Replacing depdency on {mutatedProject.Name} in {project.Name}.");
                    File.Copy(mutatedProject.DllFilePath, wouldBeDependency, true);
                }
            }
        }
    }

    /// <summary>
    /// For each failure in the build diagnostics, we will remove the mutations which was introduced closest to that location.
    /// </summary>
    /// <returns>True if all failures were actioned, False if any failures were unable to be actioned.</returns>
    private bool FindAndRemoveMutationsCausingFailures(IProjectContainer project, List<Diagnostic> failures)
    {
        bool allActioned = true;
        foreach (Diagnostic failure in failures)
        {
            if (!failure.Location.IsInSource || failure.Location.SourceSpan.IsEmpty)
            {
                Log.Debug("failure not located in source code so cannot determine a mutation that caused it. {failure}", failure);
                allActioned = false;
                continue;
            }

            FileLinePositionSpan failureLocation = failure.Location.GetLineSpan();
            if (project.DocumentsByPath.TryGetValue(failureLocation.Path, out DocumentId? document))
            {
                IEnumerable<DiscoveredMutation> mutationsInFile = _mutationDiscovery.DiscoveredMutations.Where(x => x.Document == document);

                //We want to find any mutation where the eror occurs partially or entirly inside it
                //We do this by finding all mutations which 'start' before the error ends,
                //then find all the mutations which 'end' after the error starts
                //Then we take the intersection of these, leaving us with mutations that overlap with the error
                IEnumerable<DiscoveredMutation> mutationStartBeforeErrorEnd = 
                    mutationsInFile.Where(mutant => mutant.LineSpan.StartLinePosition.CompareTo(failureLocation.Span.End) <= 0);
                IEnumerable<DiscoveredMutation> mutationsEndAfterErrorStart = 
                    mutationsInFile.Where(x => x.LineSpan.EndLinePosition.CompareTo(failureLocation.Span.Start) >= 0);
                
                IEnumerable<DiscoveredMutation> mutantsToRemove = mutationStartBeforeErrorEnd.Intersect(mutationsEndAfterErrorStart);

            }
            else
            {
                Log.Debug("No document found that matches that path specified in the build faliure. {failure}", failureLocation.Path);
                allActioned = false;
                continue;
            }
        }

        return allActioned;
    }

    private void RemoveMutants(IEnumerable<DiscoveredMutation> mutantsToRemove)
    {
        foreach (var mutant in mutantsToRemove)
        {
            SyntaxNode syntaxNode = mutant.MutatedNode.SyntaxTree.GetRoot().ReplaceNode(mutant.MutatedNode, mutant.OriginalNode);

        }
    }
}

