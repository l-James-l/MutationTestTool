using LibGit2Sharp;
using Microsoft.CodeAnalysis;
using Models;
using Serilog;

namespace Core;

/// <summary>
/// Used to establish files/lines that have changed, so that we can mutate only those sections.
/// 
/// We always do this from the working directory, and we can compare against either the head, or a specific branch.
/// </summary>
public class GitDiffManager
{
    private readonly ISolutionProvider _solutionProvider;
    private readonly IMutationSettings _settings;

    public GitDiffManager(ISolutionProvider solutionProvider, IMutationSettings settings)
    {
        _solutionProvider = solutionProvider;
        _settings = settings;
    }

    public bool EstablishDiff(string compareBranch = "HEAD") 
    {
        string wouldBeGitPath = Path.Combine(_solutionProvider.SolutionContainer.DirectoryPath, ".git");
        if (!Directory.Exists(wouldBeGitPath))
        {
            Log.Warning("No git repository detected at solution path, skipping diff.");
            return false;
        }
        Log.Information("Git repository detected, establishing diff.");

        Repository repo;
        try
        {
            repo = new Repository(wouldBeGitPath);
        }
        catch (Exception ex) 
        { 
            Log.Error($"Failed to load git repository at path: {wouldBeGitPath}. {ex}"); 
            return false; 
        }

        Patch? patch = TryGetPatch(repo, FindBranch(repo, compareBranch));
        if (patch is null)
        {
            Log.Warning("Couldn't establish a git diff to {branch}.", compareBranch);
            return false;
        }

        foreach (PatchEntryChanges x in patch)
        {
            Log.Debug("Diff in {file}", x.Path);
            x.AddedLines.ForEach(a => Log.Debug("{n}: {c}",a.LineNumber , a.Content.ToString().ReplaceLineEndings("")));
        }
        return true;
    }

    private Branch? FindBranch(Repository repo, string name)
    {
        Branch? branch = repo.Branches.FirstOrDefault(x => x.RemoteName == name || x.RemoteName == $"origin/{name}");
        if (branch is null)
        {
            Log.Warning("Couldn't find branch {branch}", name);
        }
        return branch;
    }

    private Patch? TryGetPatch(Repository repo, Branch? branch)
    {
        if (branch is null)
        {
            return null;
        }
        try
        {
            Patch patch = repo.Diff.Compare<Patch>(branch.Tip.Tree, DiffTargets.WorkingDirectory);
            return patch;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to get git diff. {ex}");
            return null;
        }
    }
}
