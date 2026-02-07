namespace Models.Attributes;

/// <summary>
/// Gives a readable description to the mutation types. This is used for display purposes in the GUI.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class MutationDescriptionAttribute : Attribute
{
    public MutationDescriptionAttribute(string description)
    {
        Description = description;
    }

    public string Description { get; }
}
