namespace Models;

public interface ISolutionProvider
{
    public void NewSolution(ISolutionContainer solution);

    public bool IsAvailable { get; }

    ISolutionContainer SolutionContainer { get; }

}
