using Models.Attributes;

namespace Models.Enums;

/// <summary>
/// This enum should all the implemented specific mutation types.
/// </summary>
public enum SpecificMutation
{
    [MutationDescription("Replaces an addition operation with a subtraction. + => -")]
    AddToSubtract,

    [MutationDescription("Replaces a subtraction with an addition. - => +")]
    SubtractToAdd
}
