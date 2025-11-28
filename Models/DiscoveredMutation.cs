using Microsoft.CodeAnalysis;

namespace Models;

/// <summary>
/// Struct to contain data around a single discovered mutation
/// </summary>
public struct DiscoveredMutation
{
    /// <summary>
    /// The mutation identifier 
    /// </summary>
    public SyntaxAnnotation ID { get; set; }

    /// <summary>
    /// Origional umutatetd node
    /// </summary>
    public SyntaxNode OriginalNode { get; set; }

    /// <summary>
    /// What the origional node was mutated into
    /// </summary>
    public SyntaxNode MutatedNode { get; set; }

    /// <summary>
    /// The ID of the document the mutation occured in.
    /// </summary>
    public DocumentId Document { get; set; }

    /// <summary>
    /// The line and position on the line that the mutation occurs
    /// </summary>
    public FileLinePositionSpan LineSpan { get; set;}
}
