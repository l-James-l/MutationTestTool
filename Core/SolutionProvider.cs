using Models;

namespace Core;

public class SolutionProvider : ISolutionProvider
{
    //By making the public property the interface, we can mock the solution in testing.
    public ISolutionContainer SolutionContainer => _solutionContainer ?? throw new InvalidOperationException("Attempted to retrieve a solution before one has been loaded.");
    private ISolutionContainer? _solutionContainer;

    public bool IsAvailable => _solutionContainer != null;

    public void NewSolution(ISolutionContainer solution)
    {
        _solutionContainer = solution;
    }
}
