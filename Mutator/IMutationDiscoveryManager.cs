using Microsoft.CodeAnalysis;
using Models;

namespace Mutator;

public interface IMutationDiscoveryManager
{
    List<DiscoveredMutation> DiscoveredMutations { get; }

    void RediscoverMutations(SyntaxTree origionalTree, SyntaxTree mutatedTree, List<DiscoveredMutation> mutations, DocumentId documentId);

}