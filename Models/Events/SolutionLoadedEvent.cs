namespace Models.Events;

/// <summary>
/// Event published when a solution has been loaded, and is ready to be built.
/// </summary>
public class SolutionLoadedEvent: PubSubEvent<bool>
{
}
