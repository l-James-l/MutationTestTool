using Models.Attributes;

namespace Models.Enums;

/// <summary>
/// This enum should cover all mutation categories supported.
/// </summary>
public enum MutationCategory
{
    [MutationDescription("Mutations that replace an arithmetic operator with another. E.g., + => -")]
    Arithmetic,

    [MutationDescription("Mutations that replace a logical operator with another. E.g., && => ||")]
    Logical,

    [MutationDescription("Mutations that replace a relational operator with another. E.g., > => <=")]
    Conditional,

    [MutationDescription("Mutations that replace a bitwise operator with another. E.g., & => |")]
    Bitwise
}
