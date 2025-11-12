using Models;

namespace Core.Interfaces;

public interface ISolutionProvider
{
    public bool IsAvailable { get; }

    SolutionContainer SolutionContiner { get; }

}
