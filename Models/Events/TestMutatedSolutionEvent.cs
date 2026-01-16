namespace Models.Events;

/// <summary>
/// Event published once a solution has been mutated and successfully built.
/// Will initiate the test runs against the mutated solution.
/// </summary>
public class TestMutatedSolutionEvent : PubSubEvent<bool>
{
    
}