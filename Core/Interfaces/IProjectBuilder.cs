using Models;

namespace Core.Interfaces;

public interface IProjectBuilder
{
    bool InitialBuild(SolutionContainer solution);
}
